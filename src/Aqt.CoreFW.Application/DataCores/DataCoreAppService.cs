using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataCores; // Contracts namespace
using Aqt.CoreFW.Application.Contracts.DataCores.Dtos; // DTOs namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO namespace
using Aqt.CoreFW.Shared.Services; // IAbpExcelExportHelper namespace
using Aqt.CoreFW.Domain.DataCores; // Domain Service and Repository Interface namespace
using Aqt.CoreFW.Domain.DataCores.Entities; // Entity namespace
using Aqt.CoreFW.Domain.DataGroups; // IDataGroupRepository
using Aqt.CoreFW.Domain.DataGroups.Entities;
using Aqt.CoreFW.Localization; // Resource namespace
using Aqt.CoreFW.Permissions; // Permissions namespace
using Aqt.CoreFW.DataCores; // Enum namespace from Domain.Shared
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping; // For ObjectMapper

namespace Aqt.CoreFW.Application.DataCores; // Application Service namespace

[Authorize(CoreFWPermissions.DataCores.Default)] // Default policy for read
public class DataCoreAppService :
    CrudAppService<
        DataCore,                  // Entity
        DataCoreDto,               // DTO Read
        Guid,                      // Primary Key
        GetDataCoresInput,         // DTO for GetList input
        CreateUpdateDataCoreDto>,  // DTO for Create/Update input
    IDataCoreAppService            // Implement the contract interface
{
    private readonly IDataCoreRepository _dataCoreRepository;
    private readonly DataCoreManager _dataCoreManager;
    private readonly IRepository<DataGroup, Guid> _dataGroupRepository; // Basic repo for DataGroup lookup
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper _excelExportHelper; // Optional: For Excel export

    // Constructor injection
    public DataCoreAppService(
        IDataCoreRepository dataCoreRepository, // Use specific repo for advanced queries
        DataCoreManager dataCoreManager,
        IRepository<DataGroup, Guid> dataGroupRepository, // Inject basic repo
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper excelExportHelper) // Inject optional helper
        : base(dataCoreRepository) // Pass specific repo to base
    {
        _dataCoreRepository = dataCoreRepository;
        _dataCoreManager = dataCoreManager;
        _dataGroupRepository = dataGroupRepository;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        // Set permission policies
        GetPolicyName = CoreFWPermissions.DataCores.Default;
        GetListPolicyName = CoreFWPermissions.DataCores.Default;
        CreatePolicyName = CoreFWPermissions.DataCores.Create;
        UpdatePolicyName = CoreFWPermissions.DataCores.Update;
        DeletePolicyName = CoreFWPermissions.DataCores.Delete;
    }

    // --- Overridden CRUD Methods ---

    [Authorize(CoreFWPermissions.DataCores.Create)]
    public override async Task<DataCoreDto> CreateAsync(CreateUpdateDataCoreDto input)
    {
        // Use DataCoreManager to create, handling code uniqueness and DataGroup validation
        var entity = await _dataCoreManager.CreateAsync(
            input.Code,
            input.Name,
            input.DataGroupId, // Manager validates this
            input.Order,
            input.Description,
            input.Status
        );

        await _dataCoreRepository.InsertAsync(entity, autoSave: true);
        return await MapToDtoWithDataGroupInfoAsync(entity); // Map with DataGroup info
    }

    [Authorize(CoreFWPermissions.DataCores.Update)]
    public override async Task<DataCoreDto> UpdateAsync(Guid id, CreateUpdateDataCoreDto input)
    {
        var entity = await _dataCoreRepository.GetAsync(id); // Get entity

        // Check for immutable Code change attempt
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
             // For simplicity, forbid Code change.
             // TODO: Localize this exception message
             throw new UserFriendlyException("Changing the DataCore Code is not allowed.");
        }

        // Use manager to handle potential DataGroup change and other updates
        entity = await _dataCoreManager.UpdateAsync(
            entity,
            input.Name,
            input.DataGroupId, // Manager validates if changed
            input.Order,
            input.Description,
            input.Status
        );

        await _dataCoreRepository.UpdateAsync(entity, autoSave: true);
        return await MapToDtoWithDataGroupInfoAsync(entity); // Map with DataGroup info
    }

    // Override GetAsync to include DataGroup Info
    public override async Task<DataCoreDto> GetAsync(Guid id)
    {
        var entity = await _dataCoreRepository.GetAsync(id);
        return await MapToDtoWithDataGroupInfoAsync(entity);
    }

    // Override GetListAsync to include DataGroup Info and handle filtering
    public override async Task<PagedResultDto<DataCoreDto>> GetListAsync(GetDataCoresInput input)
    {
        // 1. Get count
        var totalCount = await _dataCoreRepository.GetCountAsync(
            filterText: input.Filter,
            code: null, // Use filterText for combined search
            name: null, // Use filterText for combined search
            status: input.Status,
            dataGroupId: input.DataGroupId
        );

        // 2. Get list
        var entities = await _dataCoreRepository.GetListAsync(
            filterText: input.Filter,
            code: null,
            name: null,
            status: input.Status,
            dataGroupId: input.DataGroupId,
            sorting: input.Sorting ?? "Order ASC, Name ASC", // Default sort order
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        // 3. Fetch related DataGroup data efficiently
        var dataGroupIds = entities.Select(e => e.DataGroupId).Distinct().ToList();
        var dataGroups = new Dictionary<Guid, DataGroup>();
        if (dataGroupIds.Any())
        {
             dataGroups = (await _dataGroupRepository.GetListAsync(dg => dataGroupIds.Contains(dg.Id)))
                            .ToDictionary(dg => dg.Id);
        }

        // 4. Map to DTOs and populate DataGroup info
        var dtos = entities.Select(entity =>
        {
            var dto = ObjectMapper.Map<DataCore, DataCoreDto>(entity);
            if (dataGroups.TryGetValue(entity.DataGroupId, out var dataGroup))
            {
                dto.DataGroupName = dataGroup.Name;
                dto.DataGroupCode = dataGroup.Code;
            }
            return dto;
        }).ToList();

        return new PagedResultDto<DataCoreDto>(totalCount, dtos);
    }

    // --- Custom AppService Methods ---

    [Authorize(CoreFWPermissions.DataCores.Default)] // Same policy as GetList
    public async Task<ListResultDto<DataCoreLookupDto>> GetLookupByDataGroupAsync(Guid dataGroupId)
    {
        // Use the specific repository method added in Domain plan
        var dataCores = await _dataCoreRepository.GetListByDataGroupIdAsync(
            dataGroupId: dataGroupId,
            onlyActive: true // Typically lookups only need active items
        );

        var dtos = ObjectMapper.Map<List<DataCore>, List<DataCoreLookupDto>>(dataCores);
        return new ListResultDto<DataCoreLookupDto>(dtos);
    }

    // Optional: Implement Excel Export
    [Authorize(CoreFWPermissions.DataCores.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetDataCoresInput input)
    {
        // 1. Get filtered list (no pagination for export)
        var entities = await _dataCoreRepository.GetListAsync(
            filterText: input.Filter,
            code: null,
            name: null,
            status: input.Status,
            dataGroupId: input.DataGroupId,
            sorting: input.Sorting ?? "Order ASC, Name ASC",
            maxResultCount: int.MaxValue, // Get all matching for export
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

        // 3. Map to Excel DTOs and populate related info
        var excelDtos = entities.Select(entity =>
        {
            var dto = ObjectMapper.Map<DataCore, DataCoreExcelDto>(entity);
            // Manually populate DataGroup info and StatusText here
            dto.StatusText = _localizer[$"Enum:DataCoreStatus.{(int)entity.Status}"];
            if (dataGroups.TryGetValue(entity.DataGroupId, out var dataGroup))
            {
                dto.DataGroupName = dataGroup.Name;
                dto.DataGroupCode = dataGroup.Code;
            }
            return dto;
        }).ToList();

        // 4. Use helper to generate Excel file
        var fileContent = await _excelExportHelper.ExportToExcelAsync(excelDtos, "DataCores");
        return fileContent;
    }

    // --- Helper Methods ---

    /// <summary>
    /// Maps a DataCore entity to DataCoreDto and populates DataGroup info.
    /// </summary>
    private async Task<DataCoreDto> MapToDtoWithDataGroupInfoAsync(DataCore entity)
    {
        var dto = ObjectMapper.Map<DataCore, DataCoreDto>(entity);
        // Fetch DataGroup info
        var dataGroup = await _dataGroupRepository.FindAsync(entity.DataGroupId);
        if (dataGroup != null)
        {
            dto.DataGroupName = dataGroup.Name;
            dto.DataGroupCode = dataGroup.Code;
        }
        return dto;
    }
} 