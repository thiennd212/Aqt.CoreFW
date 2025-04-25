using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AttachedDocuments; // Contracts namespace
using Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos; // DTOs namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO namespace
using Aqt.CoreFW.Shared.Services; // IAbpExcelExportHelper namespace (nếu dùng, giả định)
using Aqt.CoreFW.Domain.AttachedDocuments; // Domain Service and Repository Interface namespace
using Aqt.CoreFW.Domain.AttachedDocuments.Entities; // Entity namespace
using Aqt.CoreFW.Domain.Procedures; // IProcedureRepository (Giả định)
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity (Giả định)
using Aqt.CoreFW.Localization; // Resource namespace
using Aqt.CoreFW.Permissions; // Permissions namespace
using Aqt.CoreFW.AttachedDocuments; // Enum namespace from Domain.Shared
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping; // For ObjectMapper

namespace Aqt.CoreFW.Application.AttachedDocuments; // Application Service namespace

[Authorize(CoreFWPermissions.AttachedDocuments.Default)] // Default policy for read
public class AttachedDocumentAppService :
    CrudAppService<
        AttachedDocument,                 // Entity
        AttachedDocumentDto,              // DTO Read
        Guid,                             // Primary Key
        GetAttachedDocumentsInput,        // DTO for GetList input
        CreateUpdateAttachedDocumentDto>, // DTO for Create/Update input
    IAttachedDocumentAppService           // Implement the contract interface
{
    private readonly IAttachedDocumentRepository _attachedDocumentRepository;
    private readonly AttachedDocumentManager _attachedDocumentManager;
    private readonly IRepository<Procedure, Guid> _procedureRepository; // Basic repo for Procedure lookup (Giả định)
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper? _excelExportHelper; // Optional: For Excel export (Giả định interface này tồn tại)

    // Constructor injection
    public AttachedDocumentAppService(
        IAttachedDocumentRepository attachedDocumentRepository, // Use specific repo
        AttachedDocumentManager attachedDocumentManager,
        IRepository<Procedure, Guid> procedureRepository, // (Giả định)
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper? excelExportHelper = null) // Optional injection (Giả định)
        : base(attachedDocumentRepository) // Pass specific repo to base
    {
        _attachedDocumentRepository = attachedDocumentRepository;
        _attachedDocumentManager = attachedDocumentManager;
        _procedureRepository = procedureRepository;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        // Set permission policies from Contracts
        GetPolicyName = CoreFWPermissions.AttachedDocuments.Default;
        GetListPolicyName = CoreFWPermissions.AttachedDocuments.Default;
        CreatePolicyName = CoreFWPermissions.AttachedDocuments.Create;
        UpdatePolicyName = CoreFWPermissions.AttachedDocuments.Update;
        DeletePolicyName = CoreFWPermissions.AttachedDocuments.Delete;
    }

    // --- Overridden CRUD Methods ---

    [Authorize(CoreFWPermissions.AttachedDocuments.Create)]
    public override async Task<AttachedDocumentDto> CreateAsync(CreateUpdateAttachedDocumentDto input)
    {
        // Use AttachedDocumentManager to create, handling code uniqueness within procedure and Procedure validation
        var entity = await _attachedDocumentManager.CreateAsync(
            input.Code,
            input.Name,
            input.ProcedureId, // Manager validates this and code uniqueness
            input.Order,
            input.Description,
            input.Status
        );

        await _attachedDocumentRepository.InsertAsync(entity, autoSave: true);
        return await MapToDtoWithProcedureInfoAsync(entity); // Map with Procedure info
    }

    [Authorize(CoreFWPermissions.AttachedDocuments.Update)]
    public override async Task<AttachedDocumentDto> UpdateAsync(Guid id, CreateUpdateAttachedDocumentDto input)
    {
        var entity = await _attachedDocumentRepository.GetAsync(id); // Get entity

        // Code is immutable, check if user tried to change it via DTO
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
            // TODO: Localize this exception message - Cần thêm key mới hoặc dùng ErrorCode
            throw new UserFriendlyException("Changing the AttachedDocument Code is not allowed.");
            // Or use a specific BusinessException/ErrorCode
        }

        // Use manager to handle update, including potential Procedure change and related code uniqueness check
        entity = await _attachedDocumentManager.UpdateAsync(
            entity,
            input.Name,
            input.ProcedureId, // Manager validates if changed and checks code uniqueness in new procedure
            input.Order,
            input.Description,
            input.Status
        );

        await _attachedDocumentRepository.UpdateAsync(entity, autoSave: true);
        return await MapToDtoWithProcedureInfoAsync(entity); // Map with Procedure info
    }

    // Override GetAsync to include Procedure Info
    public override async Task<AttachedDocumentDto> GetAsync(Guid id)
    {
        // Consider using GetAsync with includeDetails if navigation property mapping is needed later
        var entity = await _attachedDocumentRepository.GetAsync(id);
        return await MapToDtoWithProcedureInfoAsync(entity);
    }

    // Override GetListAsync to include Procedure Info and handle filtering
    public override async Task<PagedResultDto<AttachedDocumentDto>> GetListAsync(GetAttachedDocumentsInput input)
    {
        // 1. Get count based on filters
        var totalCount = await _attachedDocumentRepository.GetCountAsync(
            filterText: input.Filter,
            code: null, // Use filterText
            name: null, // Use filterText
            status: input.Status,
            procedureId: input.ProcedureId // Filter by specific procedure if provided
        );

        // 2. Get list based on filters and pagination/sorting
        var entities = await _attachedDocumentRepository.GetListAsync(
            filterText: input.Filter,
            code: null,
            name: null,
            status: input.Status,
            procedureId: input.ProcedureId,
            sorting: input.Sorting ?? "Order ASC, Name ASC", // Default sort
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        // 3. Fetch related Procedure data efficiently
        var procedureIds = entities.Select(e => e.ProcedureId).Distinct().ToList();
        var procedures = new Dictionary<Guid, Procedure>(); // (Giả định Procedure entity)
        if (procedureIds.Any())
        {
            // Use basic repository for efficient lookup (Giả định)
            procedures = (await _procedureRepository.GetListAsync(p => procedureIds.Contains(p.Id)))
                            .ToDictionary(p => p.Id);
        }

        // 4. Map to DTOs and populate Procedure info
        var dtos = entities.Select(entity =>
        {
            var dto = ObjectMapper.Map<AttachedDocument, AttachedDocumentDto>(entity);
            if (procedures.TryGetValue(entity.ProcedureId, out var procedure)) // (Giả định)
            {
                dto.ProcedureName = procedure.Name; // (Giả định thuộc tính Name)
                dto.ProcedureCode = procedure.Code; // (Giả định thuộc tính Code)
            }
            // Handle case where Procedure might be deleted but AttachedDocument still exists?
            // Maybe set default names or log a warning if procedure is null.
            else
            {
                 dto.ProcedureName = $"<{_localizer["UnknownProcedure"]}>"; // Sử dụng key mới
                 dto.ProcedureCode = entity.ProcedureId.ToString();
            }
            return dto;
        }).ToList();

        return new PagedResultDto<AttachedDocumentDto>(totalCount, dtos);
    }

    // --- Custom AppService Methods ---

    [Authorize] // Use read permission (default)
    public async Task<ListResultDto<AttachedDocumentLookupDto>> GetLookupByProcedureAsync(Guid procedureId)
    {
        // Use the specific repository method optimized for this lookup
        var attachedDocuments = await _attachedDocumentRepository.GetListByProcedureIdAsync(
            procedureId: procedureId,
            onlyActive: true // Only active items for lookups
        );

        var dtos = ObjectMapper.Map<List<AttachedDocument>, List<AttachedDocumentLookupDto>>(attachedDocuments);
        return new ListResultDto<AttachedDocumentLookupDto>(dtos);
    }

    // Optional: Implement Excel Export
    [Authorize(CoreFWPermissions.AttachedDocuments.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetAttachedDocumentsInput input)
    {
         if (_excelExportHelper == null)
        {
             // TODO: Localize this message
             throw new UserFriendlyException("Excel export functionality is not configured.");
        }
        // 1. Get filtered list (no pagination for export)
        var entities = await _attachedDocumentRepository.GetListAsync(
            filterText: input.Filter,
            code: null,
            name: null,
            status: input.Status,
            procedureId: input.ProcedureId,
            sorting: input.Sorting ?? "Order ASC, Name ASC",
            maxResultCount: int.MaxValue, // Get all for export
            skipCount: 0
        );

        // 2. Fetch related Procedure data efficiently
        var procedureIds = entities.Select(e => e.ProcedureId).Distinct().ToList();
        var procedures = new Dictionary<Guid, Procedure>(); // (Giả định)
        if (procedureIds.Any())
        {
            procedures = (await _procedureRepository.GetListAsync(p => procedureIds.Contains(p.Id))) // (Giả định)
                            .ToDictionary(p => p.Id);
        }

        // 3. Map to Excel DTOs and populate related info + localized status
        var excelDtos = entities.Select(entity =>
        {
            var dto = ObjectMapper.Map<AttachedDocument, AttachedDocumentExcelDto>(entity);
            // Manually populate Procedure info and localized StatusText
            dto.StatusText = _localizer[$"Enum:AttachedDocumentStatus.{(int)entity.Status}"];
            if (procedures.TryGetValue(entity.ProcedureId, out var procedure)) // (Giả định)
            {
                dto.ProcedureName = procedure.Name; // (Giả định)
                dto.ProcedureCode = procedure.Code; // (Giả định)
            }
             else
            {
                 dto.ProcedureName = $"<{_localizer["UnknownProcedure"]}>"; // Sử dụng key mới
                 dto.ProcedureCode = entity.ProcedureId.ToString();
            }
            return dto;
        }).ToList();

        // 4. Use helper to generate Excel file
        var fileContent = await _excelExportHelper.ExportToExcelAsync(excelDtos, "AttachedDocuments"); // Sheet name (Giả định phương thức helper)
        return fileContent;
    }

    // --- Helper Methods ---

    /// <summary>
    /// Maps an AttachedDocument entity to AttachedDocumentDto and populates Procedure info.
    /// </summary>
    private async Task<AttachedDocumentDto> MapToDtoWithProcedureInfoAsync(AttachedDocument entity)
    {
        var dto = ObjectMapper.Map<AttachedDocument, AttachedDocumentDto>(entity);
        // Fetch Procedure info using basic repository for efficiency (Giả định)
        var procedure = await _procedureRepository.FindAsync(entity.ProcedureId);
        if (procedure != null)
        {
            dto.ProcedureName = procedure.Name; // (Giả định)
            dto.ProcedureCode = procedure.Code; // (Giả định)
        }
        else
        {
             dto.ProcedureName = $"<{_localizer["UnknownProcedure"]}>"; // Sử dụng key mới
             dto.ProcedureCode = entity.ProcedureId.ToString();
        }
        return dto;
    }
} 