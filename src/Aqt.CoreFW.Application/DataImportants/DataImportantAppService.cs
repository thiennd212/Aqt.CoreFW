using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataImportants; // Contracts namespace
using Aqt.CoreFW.Application.Contracts.DataImportants.Dtos; // DTOs namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO namespace
using Aqt.CoreFW.Shared.Services; // IAbpExcelExportHelper namespace (nếu dùng)
using Aqt.CoreFW.Domain.DataImportants; // Domain Service and Repository Interface namespace
using Aqt.CoreFW.Domain.DataImportants.Entities; // Entity namespace
using Aqt.CoreFW.Domain.DataGroups; // IDataGroupRepository
using Aqt.CoreFW.Domain.DataGroups.Entities;
using Aqt.CoreFW.Localization; // Resource namespace
using Aqt.CoreFW.Permissions; // Permissions namespace
using Aqt.CoreFW.DataImportants; // Enum namespace from Domain.Shared
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping; // For ObjectMapper

namespace Aqt.CoreFW.Application.DataImportants; // Application Service namespace

[Authorize(CoreFWPermissions.DataImportants.Default)] // Default policy for read
public class DataImportantAppService :
    CrudAppService<
        DataImportant,                 // Entity
        DataImportantDto,              // DTO Read
        Guid,                          // Primary Key
        GetDataImportantsInput,        // DTO for GetList input
        CreateUpdateDataImportantDto>, // DTO for Create/Update input
    IDataImportantAppService           // Implement the contract interface
{
    private readonly IDataImportantRepository _dataImportantRepository;
    private readonly DataImportantManager _dataImportantManager;
    private readonly IRepository<DataGroup, Guid> _dataGroupRepository; // Basic repo for DataGroup lookup
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper? _excelExportHelper; // Optional: For Excel export

    // Constructor injection
    public DataImportantAppService(
        IDataImportantRepository dataImportantRepository, // Use specific repo
        DataImportantManager dataImportantManager,
        IRepository<DataGroup, Guid> dataGroupRepository,
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper? excelExportHelper = null) // Optional injection
        : base(dataImportantRepository) // Pass specific repo to base
    {
        _dataImportantRepository = dataImportantRepository;
        _dataImportantManager = dataImportantManager;
        _dataGroupRepository = dataGroupRepository;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        // Set permission policies from Contracts
        GetPolicyName = CoreFWPermissions.DataImportants.Default;
        GetListPolicyName = CoreFWPermissions.DataImportants.Default;
        CreatePolicyName = CoreFWPermissions.DataImportants.Create;
        UpdatePolicyName = CoreFWPermissions.DataImportants.Update;
        DeletePolicyName = CoreFWPermissions.DataImportants.Delete;
    }

    // --- Overridden CRUD Methods ---

    [Authorize(CoreFWPermissions.DataImportants.Create)]
    public override async Task<DataImportantDto> CreateAsync(CreateUpdateDataImportantDto input)
    {
        // Use DataImportantManager to create, handling code uniqueness within group and DataGroup validation
        var entity = await _dataImportantManager.CreateAsync(
            input.Code,
            input.Name,
            input.DataGroupId, // Manager validates this and code uniqueness
            input.Order,
            input.Description,
            input.Status
        );

        await _dataImportantRepository.InsertAsync(entity, autoSave: true);
        return await MapToDtoWithDataGroupInfoAsync(entity); // Map with DataGroup info
    }

    [Authorize(CoreFWPermissions.DataImportants.Update)]
    public override async Task<DataImportantDto> UpdateAsync(Guid id, CreateUpdateDataImportantDto input)
    {
        var entity = await _dataImportantRepository.GetAsync(id); // Get entity

        // Code is immutable, check if user tried to change it via DTO
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
            // TODO: Localize this exception message
            throw new UserFriendlyException("Changing the DataImportant Code is not allowed.");
            // Or use a specific BusinessException/ErrorCode
        }

        // Use manager to handle update, including potential DataGroup change and related code uniqueness check
        entity = await _dataImportantManager.UpdateAsync(
            entity,
            input.Name,
            input.DataGroupId, // Manager validates if changed and checks code uniqueness in new group
            input.Order,
            input.Description,
            input.Status
        );

        await _dataImportantRepository.UpdateAsync(entity, autoSave: true);
        return await MapToDtoWithDataGroupInfoAsync(entity); // Map with DataGroup info
    }

    // Override GetAsync to include DataGroup Info
    public override async Task<DataImportantDto> GetAsync(Guid id)
    {
        // Consider using GetAsync with includeDetails if navigation property mapping is needed later
        var entity = await _dataImportantRepository.GetAsync(id);
        return await MapToDtoWithDataGroupInfoAsync(entity);
    }

    // Override GetListAsync to include DataGroup Info and handle filtering
    public override async Task<PagedResultDto<DataImportantDto>> GetListAsync(GetDataImportantsInput input)
    {
        // 1. Get count based on filters
        var totalCount = await _dataImportantRepository.GetCountAsync(
            filterText: input.Filter,
            code: null, // Use filterText
            name: null, // Use filterText
            status: input.Status,
            dataGroupId: input.DataGroupId // Filter by specific group if provided
        );

        // 2. Get list based on filters and pagination/sorting
        var entities = await _dataImportantRepository.GetListAsync(
            filterText: input.Filter,
            code: null,
            name: null,
            status: input.Status,
            dataGroupId: input.DataGroupId,
            sorting: input.Sorting ?? "Order ASC, Name ASC", // Default sort
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        // 3. Fetch related DataGroup data efficiently
        var dataGroupIds = entities.Select(e => e.DataGroupId).Distinct().ToList();
        var dataGroups = new Dictionary<Guid, DataGroup>();
        if (dataGroupIds.Any())
        {
            // Use basic repository for efficient lookup
            dataGroups = (await _dataGroupRepository.GetListAsync(dg => dataGroupIds.Contains(dg.Id)))
                            .ToDictionary(dg => dg.Id);
        }

        // 4. Map to DTOs and populate DataGroup info
        var dtos = entities.Select(entity =>
        {
            var dto = ObjectMapper.Map<DataImportant, DataImportantDto>(entity);
            if (dataGroups.TryGetValue(entity.DataGroupId, out var dataGroup))
            {
                dto.DataGroupName = dataGroup.Name;
                dto.DataGroupCode = dataGroup.Code;
            }
            // Handle case where DataGroup might be deleted but DataImportant still exists?
            // Maybe set default names or log a warning if dataGroup is null.
            else
            {
                 dto.DataGroupName = $"<{_localizer["UnknownDataGroup"]}>"; // Or some indicator
                 dto.DataGroupCode = entity.DataGroupId.ToString();
            }
            return dto;
        }).ToList();

        return new PagedResultDto<DataImportantDto>(totalCount, dtos);
    }

    // --- Custom AppService Methods ---

    [Authorize(CoreFWPermissions.DataImportants.Default)] // Use read permission
    public async Task<ListResultDto<DataImportantLookupDto>> GetLookupByDataGroupAsync(Guid dataGroupId)
    {
        // Use the specific repository method optimized for this lookup
        var dataImportants = await _dataImportantRepository.GetListByDataGroupIdAsync(
            dataGroupId: dataGroupId,
            onlyActive: true // Only active items for lookups
        );

        var dtos = ObjectMapper.Map<List<DataImportant>, List<DataImportantLookupDto>>(dataImportants);
        return new ListResultDto<DataImportantLookupDto>(dtos);
    }

    // Optional: Implement Excel Export
    [Authorize(CoreFWPermissions.DataImportants.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetDataImportantsInput input)
    {
         if (_excelExportHelper == null)
        {
             throw new UserFriendlyException("Excel export functionality is not configured.");
        }
        // 1. Get filtered list (no pagination for export)
        var entities = await _dataImportantRepository.GetListAsync(
            filterText: input.Filter,
            code: null,
            name: null,
            status: input.Status,
            dataGroupId: input.DataGroupId,
            sorting: input.Sorting ?? "Order ASC, Name ASC",
            maxResultCount: int.MaxValue, // Get all for export
            skipCount: 0
        );

        // 2. Fetch related DataGroup data efficiently
        var dataGroupIds = entities.Select(e => e.DataGroupId).Distinct().ToList();
        var dataGroups = new Dictionary<Guid, DataGroup>();
        if (dataGroupIds.Any())
        {
            dataGroups = (await _dataGroupRepository.GetListAsync(dg => dataGroupIds.Contains(dg.Id)))
                            .ToDictionary(dg => dg.Id);
        }

        // 3. Map to Excel DTOs and populate related info + localized status
        var excelDtos = entities.Select(entity =>
        {
            var dto = ObjectMapper.Map<DataImportant, DataImportantExcelDto>(entity);
            // Manually populate DataGroup info and localized StatusText
            dto.StatusText = _localizer[$"Enum:DataImportantStatus.{(int)entity.Status}"];
            if (dataGroups.TryGetValue(entity.DataGroupId, out var dataGroup))
            {
                dto.DataGroupName = dataGroup.Name;
                dto.DataGroupCode = dataGroup.Code;
            }
             else
            {
                 dto.DataGroupName = $"<{_localizer["UnknownDataGroup"]}>";
                 dto.DataGroupCode = entity.DataGroupId.ToString();
            }
            return dto;
        }).ToList();

        // 4. Use helper to generate Excel file
        var fileContent = await _excelExportHelper.ExportToExcelAsync(excelDtos, "DataImportants"); // Sheet name
        return fileContent;
    }

    // --- Helper Methods ---

    /// <summary>
    /// Maps a DataImportant entity to DataImportantDto and populates DataGroup info.
    /// </summary>
    private async Task<DataImportantDto> MapToDtoWithDataGroupInfoAsync(DataImportant entity)
    {
        var dto = ObjectMapper.Map<DataImportant, DataImportantDto>(entity);
        // Fetch DataGroup info using basic repository for efficiency
        var dataGroup = await _dataGroupRepository.FindAsync(entity.DataGroupId);
        if (dataGroup != null)
        {
            dto.DataGroupName = dataGroup.Name;
            dto.DataGroupCode = dataGroup.Code;
        }
        else
        {
             dto.DataGroupName = $"<{_localizer["UnknownDataGroup"]}>";
             dto.DataGroupCode = entity.DataGroupId.ToString();
        }
        return dto;
    }
}
