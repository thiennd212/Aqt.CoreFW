using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Components; // Contracts namespace
using Aqt.CoreFW.Application.Contracts.Components.Dtos; // DTOs namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO namespace
using Aqt.CoreFW.Shared.Services; // IAbpExcelExportHelper namespace (giả định) - Bạn cần tạo interface và implementation này nếu chưa có
using Aqt.CoreFW.Domain.Components; // Domain Service and Repository Interface namespace
using Aqt.CoreFW.Domain.Components.Entities; // Entity namespace
using Aqt.CoreFW.Localization; // Resource namespace
using Aqt.CoreFW.Permissions; // Permissions namespace
using Aqt.CoreFW.Components; // Enum namespace from Domain.Shared
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Entities; // For EntityNotFoundException
using Volo.Abp.ObjectMapping; // For ObjectMapper (though base class provides it)

namespace Aqt.CoreFW.Application.Components; // Application Service namespace

[Authorize(CoreFWPermissions.Components.Default)] // Default policy for read
public class ProcedureComponentAppService :
    CrudAppService<
        ProcedureComponent,             // Entity
        ProcedureComponentDto,          // DTO Read
        Guid,                           // Primary Key
        GetProcedureComponentsInput,    // DTO for GetList input
        CreateUpdateProcedureComponentDto>, // DTO for Create/Update input
    IProcedureComponentAppService       // Implement the contract interface
{
    private readonly IProcedureComponentRepository _componentRepository;
    private readonly ProcedureComponentManager _componentManager;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper? _excelExportHelper; // Optional: For Excel export

    // Constructor injection
    public ProcedureComponentAppService(
        IProcedureComponentRepository componentRepository, // Use specific repo
        ProcedureComponentManager componentManager,
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper? excelExportHelper = null) // Optional injection
        : base(componentRepository) // Pass specific repo to base
    {
        _componentRepository = componentRepository;
        _componentManager = componentManager;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        // Set permission policies from Contracts
        GetPolicyName = CoreFWPermissions.Components.Default;
        GetListPolicyName = CoreFWPermissions.Components.Default;
        CreatePolicyName = CoreFWPermissions.Components.Create;
        UpdatePolicyName = CoreFWPermissions.Components.Update;
        DeletePolicyName = CoreFWPermissions.Components.Delete;
        // Note: ManageProcedureLinks permission check might be needed within UpdateAsync
    }

    // --- Overridden CRUD Methods ---

    [Authorize(CoreFWPermissions.Components.Create)]
    public override async Task<ProcedureComponentDto> CreateAsync(CreateUpdateProcedureComponentDto input)
    {
        // 1. Use Manager to create the core entity (validates code, type, content)
        var entity = await _componentManager.CreateAsync(
            input.Code,
            input.Name,
            input.Order,
            input.Type,
            input.FormDefinition,
            input.TempPath,
            input.Description,
            input.Status
        );

        // 2. Insert the core entity
        await _componentRepository.InsertAsync(entity, autoSave: true); // AutoSave for initial insert

        // 3. Update the links using the manager (requires entity.Id)
        // Ensure ProcedureIds is not null; default is new List<Guid>() in DTO.
        await _componentManager.UpdateProcedureLinksAsync(entity.Id, input.ProcedureIds ?? new List<Guid>());
        // Save changes after link update if not using AutoSave in UpdateProcedureLinksAsync or rely on UoW completion.
        // await UnitOfWorkManager.Current.SaveChangesAsync(); // If needed and autoSave=false in manager

        // 4. Map the final entity (potentially re-fetched with links if needed) to DTO
        // Mapping requires ProcedureLinks to be populated for ProcedureIds in DTO.
        // Fetch again or assume manager/UoW handles it. Let's assume mapping works after UoW completion.
        return ObjectMapper.Map<ProcedureComponent, ProcedureComponentDto>(entity);
    }

    [Authorize(CoreFWPermissions.Components.Update)]
    public override async Task<ProcedureComponentDto> UpdateAsync(Guid id, CreateUpdateProcedureComponentDto input)
    {
        // Optional: Check ManageProcedureLinks permission if ProcedureIds are being modified
        if (input.ProcedureIds != null) // Assuming null means "don't change links" (adjust logic if needed)
        {
            // This requires the PermissionChecker to be injected
            // await AuthorizationService.CheckAsync(CoreFWPermissions.Components.ManageProcedureLinks);
        }

        // 1. Get the existing entity
        var entity = await _componentRepository.GetAsync(id);

        // 2. Check immutable Code (optional but good practice)
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
            // !! LƯU Ý: Cần định nghĩa localization key "ComponentCodeCannotBeChanged" !!
            throw new UserFriendlyException(_localizer["ComponentCodeCannotBeChanged"]);
        }

        // 3. Use Manager to update the core entity properties
        entity = await _componentManager.UpdateAsync(
            entity,
            input.Name,
            input.Order,
            input.Type,
            input.FormDefinition,
            input.TempPath,
            input.Description,
            input.Status
        );

        // 4. Update the entity in the repository
        await _componentRepository.UpdateAsync(entity, autoSave: true); // AutoSave for property updates

        // 5. Update the links using the manager
        // Ensure ProcedureIds is not null; default is new List<Guid>() in DTO.
        await _componentManager.UpdateProcedureLinksAsync(entity.Id, input.ProcedureIds ?? new List<Guid>());
        // Save changes after link update if needed.
        // await UnitOfWorkManager.Current.SaveChangesAsync();

        // 6. Map the final entity to DTO
        return ObjectMapper.Map<ProcedureComponent, ProcedureComponentDto>(entity);
    }

    // Override GetAsync to ensure ProcedureIds are loaded in the DTO
    public override async Task<ProcedureComponentDto> GetAsync(Guid id)
    {
        // 1. Get the entity, ensuring details (links) are included.
        // GetAsync của Repository cơ bản thường không load navigation properties.
        // Cần đảm bảo IProcedureComponentRepository implementation (ở tầng EF Core)
        // load ProcedureLinks khi gọi GetAsync, hoặc sử dụng phương thức GetWithDetailsAsync riêng.
        // Hoặc, load thủ công như dưới đây:
        var entity = await _componentRepository.GetAsync(id);
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(ProcedureComponent), id);
        }
        // Manually load links if GetAsync doesn't include them by default
        var linkedIds = await _componentRepository.GetLinkedProcedureIdsAsync(id);

        var dto = ObjectMapper.Map<ProcedureComponent, ProcedureComponentDto>(entity);
        dto.ProcedureIds = linkedIds; // Populate the IDs

        return dto;
    }


    // Override GetListAsync to use repository filtering including ProcedureId
    public override async Task<PagedResultDto<ProcedureComponentDto>> GetListAsync(GetProcedureComponentsInput input)
    {
        var totalCount = await _componentRepository.GetCountAsync(
            filterText: input.Filter,
            code: null, // Assuming filterText searches Code and Name
            name: null, // Assuming filterText searches Code and Name
            status: input.Status,
            type: input.Type,
            procedureId: input.ProcedureId // Pass ProcedureId filter
        );

        var entities = await _componentRepository.GetListAsync(
            filterText: input.Filter,
             code: null,
            name: null,
            status: input.Status,
            type: input.Type,
            procedureId: input.ProcedureId, // Pass ProcedureId filter
            sorting: input.Sorting ?? "Order ASC, Name ASC", // Default sort
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            includeDetails: false // Do NOT include details (links) for list view performance
        );

        // Map to DTO. ProcedureIds will likely be empty due to includeDetails=false.
        var dtos = ObjectMapper.Map<List<ProcedureComponent>, List<ProcedureComponentDto>>(entities);

        return new PagedResultDto<ProcedureComponentDto>(totalCount, dtos);
    }

    // --- Custom AppService Methods ---

    [Authorize] // Allow any authenticated user to get lookups
    public async Task<ListResultDto<ProcedureComponentLookupDto>> GetLookupAsync(ComponentType? type = null)
    {
        // Use the specific repository method for lookups
        var components = await _componentRepository.GetLookupAsync(
            type: type,
            onlyActive: true // Typically only want active items for lookup
                             // Sorting is handled by the repository method default
        );

        var dtos = ObjectMapper.Map<List<ProcedureComponent>, List<ProcedureComponentLookupDto>>(components);
        return new ListResultDto<ProcedureComponentLookupDto>(dtos);
    }

    // Optional: Implement Excel Export
    [Authorize(CoreFWPermissions.Components.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetProcedureComponentsInput input)
    {
        if (_excelExportHelper == null)
        {
            throw new NotImplementedException("Excel export service (IAbpExcelExportHelper) is not registered.");
        }

        // 1. Get filtered list (no pagination for export)
        var entities = await _componentRepository.GetListAsync(
            filterText: input.Filter,
             code: null,
            name: null,
            status: input.Status,
            type: input.Type,
            procedureId: input.ProcedureId,
            sorting: input.Sorting ?? "Order ASC, Name ASC",
            maxResultCount: int.MaxValue, // Get all matching records
            skipCount: 0,
            includeDetails: false // No details needed for standard Excel export
        );

        // 2. Map to Excel DTOs using AutoMapper (which uses ComponentToExcelMappingAction)
        var excelDtos = ObjectMapper.Map<List<ProcedureComponent>, List<ProcedureComponentExcelDto>>(entities);

        // 3. Use helper to generate Excel file
        // Ensure the helper service name and method match your implementation.
        // !! LƯU Ý: Bạn cần tạo interface và implementation cho IAbpExcelExportHelper !!
        var fileContent = await _excelExportHelper.ExportToExcelAsync(excelDtos, "ProcedureComponents"); // Sheet name
        return fileContent;
    }

    public async Task<ListResultDto<ProcedureComponentDto>> GetListByProcedureAsync(Guid procedureId, ComponentType? type = null)
    {
        var components = await _componentRepository.GetListByProcedureAsync(procedureId, type);
        var dtos = ObjectMapper.Map<List<ProcedureComponent>, List<ProcedureComponentDto>>(components);
        return new ListResultDto<ProcedureComponentDto>(dtos);
    }
}