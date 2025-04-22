using System;
using System.Collections.Generic;
using System.IO; // Added for MemoryStream
using System.Linq;
using System.Linq.Dynamic.Core; // Required for dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataGroups; // Contracts namespace
using Aqt.CoreFW.Application.Contracts.DataGroups.Dtos; // DTOs namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO
using Aqt.CoreFW.Shared.Services; // IAbpExcelExportHelper namespace
using Aqt.CoreFW.Domain.DataGroups; // Domain Service and Repository Interface namespace
using Aqt.CoreFW.Domain.DataGroups.Entities; // Entity namespace
using Aqt.CoreFW.Localization; // Resource namespace
using Aqt.CoreFW.Permissions; // Permissions namespace
using Aqt.CoreFW.DataGroups; // Enum namespace from Domain.Shared
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping; // For ObjectMapper
using Volo.Abp.Threading; // Added for ICancellationTokenProvider

namespace Aqt.CoreFW.Application.DataGroups; // Application Service namespace

[Authorize(CoreFWPermissions.DataGroups.Default)] // Default policy for read
public class DataGroupAppService :
    CrudAppService<
        DataGroup,                  // Entity
        DataGroupDto,               // DTO Read
        Guid,                       // Primary Key
        GetDataGroupsInput,         // DTO for GetList input
        CreateUpdateDataGroupDto>,  // DTO for Create/Update input
    IDataGroupAppService            // Implement the contract interface
{
    private readonly IDataGroupRepository _dataGroupRepository;
    private readonly DataGroupManager _dataGroupManager;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper _excelExportHelper;
    private readonly ICancellationTokenProvider _cancellationTokenProvider;

    // Constructor injection
    public DataGroupAppService(
        IRepository<DataGroup, Guid> repository, // Base repository
        IDataGroupRepository dataGroupRepository, // Specific repository
        DataGroupManager dataGroupManager,
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper excelExportHelper,
        ICancellationTokenProvider cancellationTokenProvider)
        : base(repository)
    {
        _dataGroupRepository = dataGroupRepository;
        _dataGroupManager = dataGroupManager;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;
        _cancellationTokenProvider = cancellationTokenProvider;

        // Set permission policies
        GetPolicyName = CoreFWPermissions.DataGroups.Default;
        GetListPolicyName = CoreFWPermissions.DataGroups.Default;
        CreatePolicyName = CoreFWPermissions.DataGroups.Create;
        UpdatePolicyName = CoreFWPermissions.DataGroups.Update;
        DeletePolicyName = CoreFWPermissions.DataGroups.Delete;
    }

    // --- Overridden CRUD Methods ---

    [Authorize(CoreFWPermissions.DataGroups.Create)]
    public override async Task<DataGroupDto> CreateAsync(CreateUpdateDataGroupDto input)
    {
        // Use DataGroupManager to create, handling code uniqueness and parent validation
        var entity = await _dataGroupManager.CreateAsync(
            input.Code,
            input.Name,
            input.ParentId, // Manager validates this
            input.Order,
            input.Description,
            input.Status
            // Sync fields not managed here
        );

        await Repository.InsertAsync(entity, autoSave: true);
        return await MapEntityToDtoWithParentInfo(entity); // Map with parent info
    }

    [Authorize(CoreFWPermissions.DataGroups.Update)]
    public override async Task<DataGroupDto> UpdateAsync(Guid id, CreateUpdateDataGroupDto input)
    {
        var entity = await _dataGroupRepository.GetAsync(id); // Get entity

        // Check for immutable Code change attempt
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
             throw new UserFriendlyException(_localizer["DataGroupCodeCannotBeChanged"]); // TODO: Add localization
        }

        // Use manager to handle parent change if necessary (validates cycles)
        if (entity.ParentId != input.ParentId)
        {
            await _dataGroupManager.ChangeParentAsync(entity, input.ParentId);
        }

        // Use manager to update other properties
        entity = _dataGroupManager.UpdateAsync(
            entity,
            input.Name,
            input.Order,
            input.Description,
            input.Status,
            entity.LastSyncDate, // Keep existing sync info
            entity.SyncRecordId,
            entity.SyncRecordCode
        );

        await Repository.UpdateAsync(entity, autoSave: true);
        return await MapEntityToDtoWithParentInfo(entity); // Map with parent info
    }

    [Authorize(CoreFWPermissions.DataGroups.Delete)]
    public override async Task DeleteAsync(Guid id)
    {
        var entity = await _dataGroupRepository.GetAsync(id);
        // Use manager to validate deletion conditions (e.g., no children)
        await _dataGroupManager.ValidateBeforeDeleteAsync(entity);
        await base.DeleteAsync(id); // Soft delete
    }

    // Override GetAsync to include Parent Info
    public override async Task<DataGroupDto> GetAsync(Guid id)
    {
        // var entity = await _dataGroupRepository.GetAsync(id, includeDetails: true); // Assuming includeDetails fetches Parent if mapped
        // Using base repository GetAsync first, then fetch parent separately to avoid potential EF Core issues with includeDetails in specific scenarios
        var entity = await Repository.GetAsync(id);
        return await MapEntityToDtoWithParentInfo(entity);
    }

    // Override GetListAsync to include Parent Info and handle specific filters
    public override async Task<PagedResultDto<DataGroupDto>> GetListAsync(GetDataGroupsInput input)
    {
        // 1. Get count
        var totalCount = await _dataGroupRepository.GetCountAsync(
            filterText: input.Filter,
            status: input.Status,
            parentId: input.ParentId,
            parentIdIsNull: input.ParentIdIsNull,
            cancellationToken: _cancellationTokenProvider.Token
        );

        // 2. Get paginated list
        var entities = await _dataGroupRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            parentId: input.ParentId,
            parentIdIsNull: input.ParentIdIsNull,
            sorting: input.Sorting ?? $"{nameof(DataGroup.Order)},{nameof(DataGroup.Name)}", // Default sort
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            includeDetails: false, // Avoid includeDetails here, fetch parents separately
            cancellationToken: _cancellationTokenProvider.Token
        );

        // 3. Map entities to DTOs and populate parent info efficiently
        var dtoList = await MapListToDtoWithParentInfoAsync(entities);

        return new PagedResultDto<DataGroupDto>(totalCount, dtoList);
    }


    // --- Custom Methods from Interface ---

    [AllowAnonymous] // Or specific read permission
    public async Task<ListResultDto<DataGroupLookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var query = queryable
            .Where(dg => dg.Status == DataGroupStatus.Active)
            .OrderBy(dg => dg.Order).ThenBy(dg => dg.Name);

        var entities = await AsyncExecuter.ToListAsync(query);
        var dtos = ObjectMapper.Map<List<DataGroup>, List<DataGroupLookupDto>>(entities);
        return new ListResultDto<DataGroupLookupDto>(dtos);
    }

    public async Task<ListResultDto<DataGroupTreeNodeDto>> GetAsTreeAsync(bool onlyActive = true)
    {
        // 1. Fetch all relevant data groups (consider filtering)
        var queryable = await Repository.GetQueryableAsync();
        if(onlyActive)
        {
            queryable = queryable.Where(dg => dg.Status == DataGroupStatus.Active);
        }
        // Order might be important for building the tree visually
        queryable = queryable.OrderBy(dg => dg.ParentId).ThenBy(dg => dg.Order).ThenBy(dg => dg.Name);
        var allGroups = await AsyncExecuter.ToListAsync(queryable);

        // 2. Build the tree structure
        var treeNodes = BuildTree(allGroups, null); // Start from root (ParentId = null)

        return new ListResultDto<DataGroupTreeNodeDto>(treeNodes);
    }

    [Authorize(CoreFWPermissions.DataGroups.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetDataGroupsInput input)
    {
        // 1. Get all matching entities (no paging)
        var entities = await _dataGroupRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            parentId: input.ParentId,
            parentIdIsNull: input.ParentIdIsNull,
            sorting: input.Sorting ?? $"{nameof(DataGroup.Order)},{nameof(DataGroup.Name)}",
            maxResultCount: int.MaxValue, // Get all matching records
            skipCount: 0,
            includeDetails: false, // Fetch parents separately
            cancellationToken: _cancellationTokenProvider.Token
        );

        if (!entities.Any())
        {
            // Throwing exception might not be user-friendly for empty exports.
            // Consider returning an empty file or a specific message.
            // For now, returning an empty stream as an example.
            // Or use a more specific exception: throw new UserFriendlyException(_localizer["NoDataFoundToExport"]);
            return new RemoteStreamContent(new MemoryStream()); // Return empty content with a stream
        }

        // 2. Map entities to Excel DTOs, populating Parent and StatusText
        var parentIds = entities.Where(e => e.ParentId.HasValue).Select(e => e.ParentId.Value).Distinct().ToList();
        var parents = new Dictionary<Guid, DataGroup>();
        if (parentIds.Any())
        {
             parents = (await _dataGroupRepository.GetListAsync(e => parentIds.Contains(e.Id), cancellationToken: _cancellationTokenProvider.Token)) // Use injected provider
                           .ToDictionary(p => p.Id);
        }


        var excelDtos = new List<DataGroupExcelDto>();
        foreach (var entity in entities)
        {
            var dto = ObjectMapper.Map<DataGroup, DataGroupExcelDto>(entity);
            // Populate StatusText
            dto.StatusText = _localizer[$"Enum:DataGroupStatus.{(int)entity.Status}"];
            // Populate Parent Info
            if (entity.ParentId.HasValue && parents.TryGetValue(entity.ParentId.Value, out var parent))
            {
                dto.ParentCode = parent.Code;
                dto.ParentName = parent.Name;
            }
            excelDtos.Add(dto);
        }

        // 3. Create Excel file using the helper
        return await _excelExportHelper.ExportToExcelAsync(
            items: excelDtos,
            filePrefix: "DataGroups", // File name prefix
            sheetName: _localizer["DataGroups"] ?? "DataGroupsData" // Localized sheet name
        );
    }

    // --- Helper Methods ---

    private List<DataGroupTreeNodeDto> BuildTree(List<DataGroup> allGroups, Guid? parentId)
    {
        return allGroups
            .Where(g => g.ParentId == parentId)
            .OrderBy(g => g.Order).ThenBy(g => g.Name)
            .Select(g => {
                var node = ObjectMapper.Map<DataGroup, DataGroupTreeNodeDto>(g);
                node.Children = BuildTree(allGroups, g.Id); // Recursively build children
                return node;
            })
            .ToList();
    }

    // Helper to map a single entity to DTO including parent info
    private async Task<DataGroupDto> MapEntityToDtoWithParentInfo(DataGroup entity)
    {
         var dto = ObjectMapper.Map<DataGroup, DataGroupDto>(entity);
         if (entity.ParentId.HasValue)
         {
             // Fetch parent - consider caching or optimizing if called frequently
             var parent = await _dataGroupRepository.FindAsync(entity.ParentId.Value);
             if (parent != null)
             {
                 dto.ParentCode = parent.Code;
                 dto.ParentName = parent.Name;
             }
         }
         return dto;
    }

     // Helper to map a list of entities to DTOs including parent info efficiently
    private async Task<List<DataGroupDto>> MapListToDtoWithParentInfoAsync(List<DataGroup> entities)
    {
        var dtos = ObjectMapper.Map<List<DataGroup>, List<DataGroupDto>>(entities);
        var parentIds = entities.Where(e => e.ParentId.HasValue).Select(e => e.ParentId.Value).Distinct().ToList();

        if (parentIds.Any())
        {
            // Fetch all required parents in one go
            var parents = (await _dataGroupRepository.GetListAsync(p => parentIds.Contains(p.Id)))
                           .ToDictionary(p => p.Id);

            // Populate parent info in DTOs
            foreach (var dto in dtos.Where(d => d.ParentId.HasValue))
            {
                if (parents.TryGetValue(dto.ParentId.Value, out var parent))
                {
                    dto.ParentCode = parent.Code;
                    dto.ParentName = parent.Name;
                }
            }
        }
        return dtos;
    }
} 