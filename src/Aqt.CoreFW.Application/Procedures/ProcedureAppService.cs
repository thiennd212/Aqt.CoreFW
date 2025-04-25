using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Procedures; // Contracts namespace
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // DTOs namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO namespace
using Aqt.CoreFW.Shared.Services; // IAbpExcelExportHelper namespace (nếu dùng)
using Aqt.CoreFW.Domain.Procedures; // Domain Service and Repository Interface namespace
using Aqt.CoreFW.Domain.Procedures.Entities; // Entity namespace
using Aqt.CoreFW.Localization; // Resource namespace
using Aqt.CoreFW.Permissions; // Permissions namespace
using Aqt.CoreFW.Procedures; // Enum namespace from Domain.Shared
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.ObjectMapping; // For ObjectMapper //ObjectMapper is inherited from AbpApplicationService, no need to inject
using Volo.Abp.Domain.Repositories; // Required for IRepository<>

namespace Aqt.CoreFW.Application.Procedures; // Application Service namespace

[Authorize(CoreFWPermissions.Procedures.Default)] // Default policy for read
public class ProcedureAppService :
    CrudAppService<
        Procedure,                   // Entity
        ProcedureDto,                // DTO Read
        Guid,                          // Primary Key
        GetProceduresInput,           // DTO for GetList input
        CreateUpdateProcedureDto>,   // DTO for Create/Update input
    IProcedureAppService             // Implement the contract interface
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly ProcedureManager _procedureManager;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper? _excelExportHelper; // Optional: For Excel export

    // Constructor injection
    public ProcedureAppService(
        IRepository<Procedure, Guid> repository, // Inject generic repository for base CrudAppService
        IProcedureRepository procedureRepository, // Inject specific repository for custom methods
        ProcedureManager procedureManager,
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper? excelExportHelper = null) // Optional injection
        : base(repository) // Pass generic repo to base
    {
        // Keep both generic and specific repositories if needed
        _procedureRepository = procedureRepository;
        _procedureManager = procedureManager;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        // Set permission policies from Contracts
        GetPolicyName = CoreFWPermissions.Procedures.Default;
        GetListPolicyName = CoreFWPermissions.Procedures.Default;
        CreatePolicyName = CoreFWPermissions.Procedures.Create;
        UpdatePolicyName = CoreFWPermissions.Procedures.Update;
        DeletePolicyName = CoreFWPermissions.Procedures.Delete;
    }

    // --- Overridden CRUD Methods ---

    [Authorize(CoreFWPermissions.Procedures.Create)]
    public override async Task<ProcedureDto> CreateAsync(CreateUpdateProcedureDto input)
    {
        // Use ProcedureManager to create, handling code uniqueness and other business rules
        var entity = await _procedureManager.CreateAsync(
            input.Code,
            input.Name,
            input.Order,
            input.Description,
            input.Status
        );

        // Insert the entity using the repository injected into the base class
        await Repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Procedure, ProcedureDto>(entity);
    }

    [Authorize(CoreFWPermissions.Procedures.Update)]
    public override async Task<ProcedureDto> UpdateAsync(Guid id, CreateUpdateProcedureDto input)
    {
        // Get the entity using the repository from the base class
        var entity = await Repository.GetAsync(id);

        // Code Immutability Check: Prevent changing the Code after creation
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
            // Use localized exception message from CoreFWResource
            throw new UserFriendlyException(_localizer["ProcedureCodeCannotBeChanged"]);
            // Consider creating a specific error code if needed
        }

        // Use manager to handle the update logic and validation
        entity = await _procedureManager.UpdateAsync(
            entity,
            input.Name,
            input.Order,
            input.Description,
            input.Status
        );

        // Update the entity using the repository from the base class
        await Repository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<Procedure, ProcedureDto>(entity);
    }

    // GetAsync: Base implementation is sufficient unless projection/mapping customization is needed.
    // public override Task<ProcedureDto> GetAsync(Guid id) { ... }

    // GetListAsync: Override to use the specific repository for potentially optimized querying
    public override async Task<PagedResultDto<ProcedureDto>> GetListAsync(GetProceduresInput input)
    {
        // Using the specific repository method for filtering
        var totalCount = await _procedureRepository.GetCountAsync(
            filterText: input.Filter,
            status: input.Status
        );

        var entities = await _procedureRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            sorting: input.Sorting ?? $"{nameof(Procedure.Order)} ASC, {nameof(Procedure.Name)} ASC", // Default sort using nameof
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        var dtos = ObjectMapper.Map<List<Procedure>, List<ProcedureDto>>(entities);

        return new PagedResultDto<ProcedureDto>(totalCount, dtos);
    }

    // DeleteAsync: Base implementation handles soft delete if Entity is FullAuditedAggregateRoot.
    // public override Task DeleteAsync(Guid id) { ... }

    // --- Custom AppService Methods ---

    // Lookup method - requires authentication but no specific permission by default
    [Authorize]
    public async Task<ListResultDto<ProcedureLookupDto>> GetLookupAsync()
    {
        // Use the specific repository method optimized for lookups
        var procedures = await _procedureRepository.GetLookupAsync(
            onlyActive: true, // Ensure only active records are returned for lookups
            sorting: $"{nameof(Procedure.Order)} ASC, {nameof(Procedure.Name)} ASC" // Consistent sorting
        );

        var dtos = ObjectMapper.Map<List<Procedure>, List<ProcedureLookupDto>>(procedures);
        return new ListResultDto<ProcedureLookupDto>(dtos);
    }

    // Optional: Implement Excel Export
    [Authorize(CoreFWPermissions.Procedures.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetProceduresInput input)
    {
         if (_excelExportHelper == null)
        {
             throw new UserFriendlyException(_localizer["ExcelExportFunctionalityNotConfigured"]); // Localize message
        }

        // 1. Get filtered list (no pagination for export)
        var entities = await _procedureRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            sorting: input.Sorting ?? $"{nameof(Procedure.Order)} ASC, {nameof(Procedure.Name)} ASC",
            maxResultCount: int.MaxValue, // Get all matching records
            skipCount: 0
        );

        // 2. Map to Excel DTOs (AutoMapper profile handles StatusText localization via MappingAction)
        var excelDtos = ObjectMapper.Map<List<Procedure>, List<ProcedureExcelDto>>(entities);

        // 3. Use helper to generate Excel file
        // Use localized sheet name if possible
        var fileContent = await _excelExportHelper.ExportToExcelAsync(excelDtos, _localizer["Menu:Procedures"]);
        return fileContent;
    }

    // Example for updating sync info (if required via API)
    /*
    [Authorize(CoreFWPermissions.Procedures.Sync)] // Assuming a 'Sync' permission exists
    public async Task UpdateSyncInfoAsync(Guid id, UpdateProcedureSyncInfoDto syncInfo)
    {
        var entity = await Repository.GetAsync(id); // Get entity via base repo

        // Use manager to update sync info
        entity = await _procedureManager.UpdateSyncInfoAsync(
            entity,
            syncInfo.LastSyncedDate,
            syncInfo.SyncRecordId,
            syncInfo.SyncRecordCode
        );

        await Repository.UpdateAsync(entity, autoSave: true); // Save changes via base repo
    }
    */
}
