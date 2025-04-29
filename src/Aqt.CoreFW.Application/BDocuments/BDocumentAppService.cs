using Aqt.CoreFW.Application.Contracts.BDocuments; // Service Interface
using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos; // DTOs
                                                        // using Aqt.CoreFW.Application.Contracts.Files; // Use this if you have a custom FileInfoDto Contract
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // ProcedureDto
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // WorkflowStatusDto
using Aqt.CoreFW.BDocuments; // Constants
using Aqt.CoreFW.Components; // ComponentType Enum
using Aqt.CoreFW.Domain.BDocuments; // Domain Service and Repository
using Aqt.CoreFW.Domain.BDocuments.Entities; // Entities
using Aqt.CoreFW.Domain.Components; // Component Repository & Entity
using Aqt.CoreFW.Domain.Procedures; // Procedure Repository & Entity
using Aqt.CoreFW.Domain.Procedures.Entities;
using Aqt.CoreFW.Domain.WorkflowStatuses; // Status Repository & Entity
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities;
using Aqt.CoreFW.Helpers;
using Aqt.CoreFW.Localization; // Resource
using Aqt.CoreFW.Permissions; // Permissions
using Aqt.CoreFW.Shared.Services; // Add logging
using EasyAbp.FileManagement.Files; // IFileAppService, IFileManager, File Entity, CreateFileInput
using EasyAbp.FileManagement.Files.Dtos; // Use FileInfoDto from FileManagement
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging; // Add logging
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // For JSON handling
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Entities; // For EntityNotFoundException
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Abp.Users; // UnitOfWork

namespace Aqt.CoreFW.Application.BDocuments;

[Authorize(CoreFWPermissions.BDocuments.Default)]
public class BDocumentAppService : ApplicationService, IBDocumentAppService
{
    private readonly IBDocumentRepository _bDocumentRepository;
    private readonly BDocumentManager _bDocumentManager;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IWorkflowStatusRepository _statusRepository;
    private readonly IProcedureComponentRepository _componentRepository;
    private readonly IFileAppService _fileAppService; // File Management AppService
    private readonly IFileManager _fileManager;       // File Management Domain Service
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper _excelExportHelper; // Optional for Excel
                                                               // private readonly IValidationErrorHandler _validationErrorHandler; // Optional: Inject if needed for specific validation handling
    private readonly ICurrentUser _currentUser;

    // Add Logger
    private readonly ILogger<BDocumentAppService> _logger;


    public BDocumentAppService(
        IBDocumentRepository bDocumentRepository,
        BDocumentManager bDocumentManager,
        IProcedureRepository procedureRepository,
        IWorkflowStatusRepository statusRepository,
        IProcedureComponentRepository componentRepository,
        IFileAppService fileAppService,
        IFileManager fileManager, // Inject FileManager
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> localizer,
        // IValidationErrorHandler validationErrorHandler, // Optional
        ILogger<BDocumentAppService> logger, // Inject Logger
        IAbpExcelExportHelper excelExportHelper,
        ICurrentUser currentUser) // Excel helper is optional
    {
        _bDocumentRepository = bDocumentRepository;
        _bDocumentManager = bDocumentManager;
        _procedureRepository = procedureRepository;
        _statusRepository = statusRepository;
        _componentRepository = componentRepository;
        _fileAppService = fileAppService;
        _fileManager = fileManager; // Assign FileManager
        _guidGenerator = guidGenerator;
        _localizer = localizer;
        // _validationErrorHandler = validationErrorHandler; // Optional
        _excelExportHelper = excelExportHelper;
        _logger = logger; // Assign Logger
        _currentUser = currentUser;

        LocalizationResource = typeof(CoreFWResource);
    }

    [Authorize(CoreFWPermissions.BDocuments.Create)]
    [UnitOfWork]
    public async Task<BDocumentDto> CreateAsync(CreateBDocumentInputDto input)
    {
        _logger.LogInformation("Starting BDocument creation for ProcedureId: {ProcedureId}", input.ProcedureId);

        // 1. Generate document code (Consider a more robust approach if needed)
        // TODO: Implement a configurable and robust code generation strategy
        string maHoSo = "HS-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        _logger.LogDebug("Generated MaHoSo: {MaHoSo}", maHoSo);


        // 2. Create BDocument entity using Manager (without components yet)
        var bDocument = await _bDocumentManager.CreateAsync(
            input.ProcedureId,
            maHoSo,
            input.TenChuHoSo,
            input.SoDinhDanhChuHoSo,
            input.DiaChiChuHoSo,
            input.EmailChuHoSo,
            input.SoDienThoaiChuHoSo,
            input.PhamViHoatDong,
            input.DangKyNhanQuaBuuDien,
            Clock.Now // Use ABP Clock for consistent time
        );
        _logger.LogDebug("Created BDocument entity with Id: {BDocumentId}", bDocument.Id);

        // 3. Add initial component data
        if (input.ComponentData != null && input.ComponentData.Any())
        {
            _logger.LogDebug("Processing {ComponentCount} component data entries.", input.ComponentData.Count);
            // Get all components for the procedure in one go
            var procedureComponents = (await _componentRepository.GetListAsync(procedureId: input.ProcedureId)).ToDictionary(c => c.Id);

            foreach (var componentInput in input.ComponentData)
            {
                if (!procedureComponents.TryGetValue(componentInput.ProcedureComponentId, out var componentDef))
                {
                    _logger.LogWarning("Skipping invalid or non-existent ProcedureComponentId: {ProcedureComponentId}", componentInput.ProcedureComponentId);
                    continue; // Skip invalid components
                }

                _logger.LogDebug("Processing component: {ComponentName} (Id: {ProcedureComponentId})", componentDef.Name, componentInput.ProcedureComponentId);


                // Validate data type consistency (delegated to Manager)
                _bDocumentManager.ValidateComponentDataType(componentDef, componentInput.FormData, componentInput.FileId);

                // TODO: Implement detailed validation for declaration form data if needed
                if (componentDef.Code == BDocumentConsts.DeclarationFormComponentCode && componentDef.Type == ComponentType.Form)
                {
                    // ValidateDeclarationFormData(componentInput.FormData, componentDef.FormDefinition);
                    _logger.LogDebug("Component is a declaration form, potential validation needed.");
                }

                // Validate file existence (if FileId provided) - Manager already does this
                // if (componentInput.FileId.HasValue) { await _bDocumentManager.ValidateFileExistsAsync(componentInput.FileId.Value); }

                // Add data using Manager
                await _bDocumentManager.AddOrUpdateComponentDataAsync(
                    bDocument,
                    componentInput.ProcedureComponentId,
                    componentInput.FormData, // Contains declaration JSON
                    componentInput.FileId // Contains uploaded file ID
                );
                _logger.LogDebug("Added/Updated data for component {ProcedureComponentId}", componentInput.ProcedureComponentId);
            }
        }
        else
        {
            _logger.LogDebug("No initial component data provided.");
        }


        // 4. Save BDocument (UoW handles BDocumentData persistence)
        await _bDocumentRepository.InsertAsync(bDocument, autoSave: true);
        _logger.LogInformation("Successfully inserted BDocument with Id: {BDocumentId}", bDocument.Id);


        // 5. Trigger Workflow (optional, could be event-driven or called explicitly)
        // Example: await _workflowManager.StartWorkflowAsync("BDocumentWorkflow", bDocument.Id);
        _logger.LogDebug("Workflow trigger step (if applicable).");


        // 6. Return detailed DTO (enriched)
        return await GetAsync(bDocument.Id);
    }

    [Authorize(CoreFWPermissions.BDocuments.Update)]
    [UnitOfWork]
    public async Task<BDocumentDto> UpdateAsync(Guid id, UpdateBDocumentInputDto input)
    {
        _logger.LogInformation("Starting BDocument update for Id: {BDocumentId}", id);
        var bDocument = await _bDocumentRepository.GetAsync(id);
        // Check status allowance (delegate to Manager - TODO inside manager)
        // await _bDocumentManager.ValidateUpdatableStatusAsync(bDocument);

        // Call Manager to update main info
        await _bDocumentManager.UpdateInfoAsync(
             bDocument,
             input.TenChuHoSo,
             input.SoDinhDanhChuHoSo,
             input.DiaChiChuHoSo,
             input.EmailChuHoSo,
             input.SoDienThoaiChuHoSo,
             input.PhamViHoatDong,
             input.DangKyNhanQuaBuuDien
         );
        _logger.LogDebug("Updated BDocument entity info for Id: {BDocumentId}", id);


        await _bDocumentRepository.UpdateAsync(bDocument, autoSave: true);
        _logger.LogInformation("Successfully updated BDocument with Id: {BDocumentId}", id);
        return await GetAsync(id);
    }

    public async Task<BDocumentDto> GetAsync(Guid id)
    {
        _logger.LogDebug("Fetching BDocument with Id: {BDocumentId}", id);
        // Ensure DocumentData is loaded using the specific repository method
        var bDocument = await _bDocumentRepository.GetWithDataAsync(id);
        if (bDocument == null)
        {
            _logger.LogWarning("BDocument with Id {BDocumentId} not found.", id);
            throw new EntityNotFoundException(typeof(BDocument), id);
        }

        var dto = ObjectMapper.Map<BDocument, BDocumentDto>(bDocument);
        _logger.LogDebug("Mapped BDocument to BDocumentDto for Id: {BDocumentId}", id);

        // Enrich with Procedure Info
        var procedure = await _procedureRepository.FindAsync(bDocument.ProcedureId);
        if (procedure != null)
        {
            dto.Procedure = ObjectMapper.Map<Procedure, ProcedureDto>(procedure);
            _logger.LogDebug("Enriched with Procedure info: {ProcedureName}", procedure.Name);
        }
        else
        {
            _logger.LogWarning("Procedure not found for BDocument Id: {BDocumentId}, ProcedureId: {ProcedureId}", id, bDocument.ProcedureId);
        }


        // Enrich with WorkflowStatus Info
        if (bDocument.TrangThaiHoSoId.HasValue)
        {
            var status = await _statusRepository.FindAsync(bDocument.TrangThaiHoSoId.Value);
            if (status != null)
            {
                dto.TrangThaiHoSo = ObjectMapper.Map<WorkflowStatus, WorkflowStatusDto>(status);
                _logger.LogDebug("Enriched with WorkflowStatus info: {StatusName}", status.Name);
            }
            else
            {
                _logger.LogWarning("WorkflowStatus not found for BDocument Id: {BDocumentId}, StatusId: {StatusId}", id, bDocument.TrangThaiHoSoId.Value);
            }
        }
        else
        {
            _logger.LogDebug("BDocument Id: {BDocumentId} has no associated WorkflowStatus.", id);
        }


        // Enrich DocumentData with Component and File Info
        if (dto.DocumentData.Any())
        {
            _logger.LogDebug("Enriching {DocumentDataCount} BDocumentData items.", dto.DocumentData.Count);
            var componentIds = dto.DocumentData.Select(d => d.ProcedureComponentId).Distinct().ToList();
            var fileIds = dto.DocumentData.Where(d => d.FileId.HasValue).Select(d => d.FileId!.Value).Distinct().ToList();

            // Fetch components and files efficiently
            var components = (await _componentRepository.GetListByIdsAsync(ids: componentIds)).ToDictionary(c => c.Id);
            _logger.LogDebug("Fetched {ComponentCount} ProcedureComponent definitions.", components.Count);
            var fileInfos = new Dictionary<Guid, FileInfoDto>(); // Use FileInfoDto from FileManagement directly
            if (fileIds.Any())
            {
                // TODO: Ideally use a GetManyAsync from FileManagement if available.
                _logger.LogDebug("Fetching file info for {FileIdCount} file IDs.", fileIds.Count);
                try
                {
                    // Assuming FileAppService has permission checks
                    foreach (var fileId in fileIds)
                    {
                        try
                        {
                            var fileDto = await _fileAppService.GetAsync(fileId);
                            fileInfos[fileId] = fileDto; // Directly use the DTO from FileManagement
                        }
                        catch (EntityNotFoundException)
                        {
                            _logger.LogWarning("File with Id {FileId} not found in FileManagement for BDocument {BDocumentId}", fileId, id);
                        }
                    }
                    _logger.LogDebug("Successfully fetched info for {FileInfoCount} files.", fileInfos.Count);
                }
                catch (Exception ex) // Catch broader exceptions during file fetching loop
                {
                    // Log the error but continue enriching other data if possible
                    _logger.LogError(ex, "Error fetching multiple file infos from FileManagement for BDocument {BDocumentId}. File IDs: {FileIds}", id, string.Join(",", fileIds));
                }
            }

            // Populate details in BDocumentDataDto list
            foreach (var dataDto in dto.DocumentData)
            {
                if (components.TryGetValue(dataDto.ProcedureComponentId, out var component))
                {
                    dataDto.ComponentCode = component.Code;
                    dataDto.ComponentName = component.Name;
                    dataDto.ComponentType = component.Type;
                    // TODO: Fetch IsRequired flag (needs ProcedureComponentLink info access)
                    // dataDto.IsRequired = await GetIsRequiredFlag(bDocument.ProcedureId, dataDto.ProcedureComponentId);
                    _logger.LogTrace("Enriched component info for ProcedureComponentId: {ProcedureComponentId}", dataDto.ProcedureComponentId);
                }
                else
                {
                    _logger.LogWarning("Component definition not found for ProcedureComponentId: {ProcedureComponentId} in BDocument {BDocumentId}", dataDto.ProcedureComponentId, id);
                }


                if (dataDto.FileId.HasValue && fileInfos.TryGetValue(dataDto.FileId.Value, out var fileInfo))
                {
                    dataDto.FileInfo = fileInfo; // Assign the fetched FileInfoDto
                    _logger.LogTrace("Enriched file info for FileId: {FileId}", dataDto.FileId.Value);
                }
                else if (dataDto.FileId.HasValue)
                {
                    // This case happens if the file existed in BDocumentData but was not found in FileManagement
                    _logger.LogWarning("FileInfo for FileId {FileId} was not found or could not be fetched for BDocument {BDocumentId}", dataDto.FileId.Value, id);
                }
            }
        }
        else
        {
            _logger.LogDebug("BDocument Id: {BDocumentId} has no associated BDocumentData.", id);
        }

        return dto;
    }

    public async Task<PagedResultDto<BDocumentListDto>> GetListAsync(GetBDocumentsInput input)
    {
        _logger.LogInformation("Getting BDocument list with filter: {Filter}, ProcedureId: {ProcedureId}, StatusId: {StatusId}", input.Filter, input.ProcedureId, input.StatusId);
        var totalCount = await _bDocumentRepository.GetCountAsync(
            filterText: input.Filter,
            procedureId: input.ProcedureId,
            trangThaiHoSoId: input.StatusId,
            ngayNopFrom: input.SubmissionDateFrom,
            ngayNopTo: input.SubmissionDateTo
        // TODO: Add filter for DangKyNhanQuaBuuDien if needed
        );
        _logger.LogDebug("Total count: {TotalCount}", totalCount);

        if (totalCount == 0)
        {
            return new PagedResultDto<BDocumentListDto>(0, new List<BDocumentListDto>());
        }


        var bDocuments = await _bDocumentRepository.GetListAsync(
            filterText: input.Filter,
            procedureId: input.ProcedureId,
            trangThaiHoSoId: input.StatusId,
            ngayNopFrom: input.SubmissionDateFrom,
            ngayNopTo: input.SubmissionDateTo,
            // TODO: Add filter for DangKyNhanQuaBuuDien if needed
            sorting: input.Sorting ?? $"{nameof(BDocument.CreationTime)} DESC",
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            includeDetails: false // Details not needed for list
        );
        _logger.LogDebug("Fetched {DocumentCount} BDocuments for the current page.", bDocuments.Count);

        var dtos = ObjectMapper.Map<List<BDocument>, List<BDocumentListDto>>(bDocuments);

        // Enrich with Procedure and Status names/colors efficiently
        if (dtos.Any())
        {
            var procedureIds = dtos.Select(d => d.ProcedureId).Distinct().ToList();
            var statusIds = dtos.Where(d => d.TrangThaiHoSoId.HasValue).Select(d => d.TrangThaiHoSoId!.Value).Distinct().ToList();

            // Fetch related data in batches
            var procedures = (await _procedureRepository.GetListByIdsAsync(ids: procedureIds)).ToDictionary(p => p.Id);
            var statuses = new Dictionary<Guid, WorkflowStatus>();
            if (statusIds.Any())
            {
                statuses = (await _statusRepository.GetListByIdsAsync(ids: statusIds)).ToDictionary(s => s.Id);
            }
            _logger.LogDebug("Fetched {ProcedureCount} procedures and {StatusCount} statuses for enrichment.", procedures.Count, statuses.Count);

            // Enrich DTOs
            foreach (var dto in dtos)
            {
                if (procedures.TryGetValue(dto.ProcedureId, out var procedure))
                {
                    dto.ProcedureName = procedure.Name;
                }
                if (dto.TrangThaiHoSoId.HasValue && statuses.TryGetValue(dto.TrangThaiHoSoId.Value, out var status))
                {
                    dto.TrangThaiHoSoName = status.Name;
                    dto.TrangThaiHoSoColorCode = status.ColorCode;
                }
                else
                {
                    // Handle null status name (e.g., use localized "Not Set")
                    dto.TrangThaiHoSoName = _localizer["NotSet"] ?? "Not Set";
                }
            }
        }

        return new PagedResultDto<BDocumentListDto>(totalCount, dtos);
    }

    [Authorize(CoreFWPermissions.BDocuments.Delete)]
    [UnitOfWork]
    public virtual async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Attempting to delete BDocument with Id: {BDocumentId}", id);
        // Fetch with data only if file deletion logic is active
        // bool deleteFiles = false; // TODO: Make this configurable
        // var bDocument = deleteFiles ? await _bDocumentRepository.GetWithDataAsync(id) : await _bDocumentRepository.GetAsync(id);
        var bDocument = await _bDocumentRepository.GetAsync(id); // Get without data by default


        if (bDocument == null)
        {
            _logger.LogWarning("BDocument with Id {BDocumentId} not found for deletion.", id);
            // Consider not throwing if delete is meant to be idempotent, or throw as planned
            throw new EntityNotFoundException(typeof(BDocument), id);
        }

        // Check status allowance (delegate to Manager - TODO inside manager)
        // await _bDocumentManager.ValidateDeletableStatusAsync(bDocument);

        // TODO: Implement file deletion logic based on policy
        // if (deleteFiles && bDocument.DocumentData != null)
        // {
        //     _logger.LogInformation("Deleting associated files for BDocument {BDocumentId}", id);
        //     foreach (var data in bDocument.DocumentData.Where(d => d.FileId.HasValue))
        //     {
        //         try {
        //             await _bDocumentManager.DeleteFileAsync(data.FileId.Value, bDocument.Id, data.ProcedureComponentId);
        //         } catch (Exception ex) {
        //              _logger.LogError(ex, "Failed to delete associated file {FileId} during BDocument {BDocumentId} deletion. Continuing deletion of BDocument.", data.FileId.Value, id);
        //              // Decide if failure to delete a file should stop the BDocument deletion
        //         }
        //     }
        // }

        // Soft delete BDocument (EF Core might handle cascade delete for BDocumentData if configured)
        await _bDocumentRepository.DeleteAsync(id, autoSave: true);
        _logger.LogInformation("Successfully soft-deleted BDocument with Id: {BDocumentId}", id);
    }

    [Authorize(CoreFWPermissions.BDocuments.ManageComponents)]
    [UnitOfWork]
    public async Task<BDocumentDataDto> AddOrUpdateDataAsync(Guid id, AddOrUpdateBDocumentDataInputDto input)
    {
        _logger.LogInformation("Adding/Updating component data for BDocument Id: {BDocumentId}, ProcedureComponentId: {ProcedureComponentId}", id, input.ProcedureComponentId);
        var bDocument = await _bDocumentRepository.GetAsync(id); // No need for full data here
        if (bDocument == null) throw new EntityNotFoundException(typeof(BDocument), id);

        // Check status allowance (delegate to Manager - TODO inside manager)
        // await _bDocumentManager.ValidateUpdatableComponentDataStatusAsync(bDocument);

        // TODO: Validate declaration form JSON if applicable before passing to manager
        // var component = await _componentRepository.GetAsync(input.ProcedureComponentId);
        // if(component.Code == BDocumentConsts.DeclarationFormComponentCode && component.Type == ComponentType.Form) {
        //    ValidateDeclarationFormData(input.FormData, component.FormDefinition);
        // }

        // Use the manager to handle the logic
        await _bDocumentManager.AddOrUpdateComponentDataAsync(
            bDocument,
            input.ProcedureComponentId,
            input.FormData,
            input.FileId
        );

        // Save changes to the BDocument aggregate
        await _bDocumentRepository.UpdateAsync(bDocument, autoSave: true);
        _logger.LogInformation("Updated BDocument aggregate after AddOrUpdateData for BDocument Id: {BDocumentId}", id);


        // Fetch the specific updated/added BDocumentData record for the response
        // Need to requery as the manager modifies the collection within the aggregate
        var updatedDocument = await _bDocumentRepository.GetWithDataAsync(id); // Requery with data
        var updatedData = updatedDocument?.DocumentData
                            .FirstOrDefault(d => d.ProcedureComponentId == input.ProcedureComponentId);


        if (updatedData == null)
        {
            _logger.LogError("Failed to retrieve updated BDocumentData for BDocument {BDocumentId}, ProcedureComponentId {ProcedureComponentId} after update.", id, input.ProcedureComponentId);
            // Throw a more specific exception or handle appropriately
            throw new UserFriendlyException("Failed to retrieve updated document data.");
        }

        // Enrich the single BDocumentDataDto for the response
        return await EnrichBDocumentDataDtoAsync(updatedData);
    }

    [Authorize(CoreFWPermissions.BDocuments.ManageComponents)]
    [UnitOfWork]
    public async Task RemoveDataAsync(Guid id, Guid bDocumentDataId, bool deleteFile = false)
    {
        _logger.LogInformation("Removing BDocumentData Id: {BDocumentDataId} from BDocument Id: {BDocumentId}. Delete file: {DeleteFile}", bDocumentDataId, id, deleteFile);
        var bDocument = await _bDocumentRepository.GetWithDataAsync(id); // Need data to find the correct component ID
        if (bDocument == null) throw new EntityNotFoundException(typeof(BDocument), id);

        // Check status allowance (delegate to Manager - TODO inside manager)
        // await _bDocumentManager.ValidateUpdatableComponentDataStatusAsync(bDocument);

        var dataToRemove = bDocument.DocumentData.FirstOrDefault(d => d.Id == bDocumentDataId);
        if (dataToRemove == null)
        {
            _logger.LogWarning("BDocumentData with Id {BDocumentDataId} not found in BDocument {BDocumentId}. No action taken.", bDocumentDataId, id);
            return; // Or throw? Depends on expected behavior.
        }

        // Use Manager to handle removal and potential file deletion
        await _bDocumentManager.RemoveComponentDataAsync(
            bDocument,
            dataToRemove.ProcedureComponentId, // Pass ProcedureComponentId to manager
            deleteFile
        );

        await _bDocumentRepository.UpdateAsync(bDocument, autoSave: true);
        _logger.LogInformation("Successfully removed BDocumentData Id: {BDocumentDataId} from BDocument Id: {BDocumentId}", bDocumentDataId, id);
    }


    // Typically status changes are driven by specific actions or workflows, might need dedicated permissions
    [Authorize(CoreFWPermissions.BDocuments.Update)] // Or a more specific permission like ManageWorkflow
    [UnitOfWork]
    public async Task<BDocumentDto> ChangeStatusAsync(Guid id, ChangeBDocumentStatusInputDto input)
    {
        _logger.LogInformation("Changing status for BDocument Id: {BDocumentId} to StatusId: {NewStatusId}", id, input.NewStatusId);
        var bDocument = await _bDocumentRepository.GetAsync(id);
        if (bDocument == null) throw new EntityNotFoundException(typeof(BDocument), id);
        // Check specific permission for status change if needed

        // Use manager to handle status change logic and validation
        await _bDocumentManager.ChangeStatusAsync(
            bDocument,
            input.NewStatusId,
            input.Reason
        );

        await _bDocumentRepository.UpdateAsync(bDocument, autoSave: true);
        _logger.LogInformation("Successfully changed status for BDocument Id: {BDocumentId}", id);
        return await GetAsync(id); // Return enriched DTO
    }


    [Authorize(CoreFWPermissions.BDocuments.GenerateDeclarationFile)]
    [UnitOfWork]
    public async Task<FileInfoDto> GenerateDeclarationFileAsync(Guid id)
    {
        _logger.LogInformation("Starting declaration file generation for BDocument Id: {BDocumentId}", id);
        var bDocument = await _bDocumentRepository.GetWithDataAsync(id); // Need data
        if (bDocument == null) throw new EntityNotFoundException(typeof(BDocument), id);
        // TODO: Check status allowance for generation?

        // 1. Find Declaration Component and Data
        _logger.LogDebug("Looking for declaration component for ProcedureId: {ProcedureId}", bDocument.ProcedureId);
        // Fetch components efficiently
        var procedureComponents = (await _componentRepository.GetListAsync(procedureId: bDocument.ProcedureId))
                                      .ToDictionary(c => c.Id);

        var declarationComponentEntry = procedureComponents.Values
                                      .FirstOrDefault(c => c.Code == BDocumentConsts.DeclarationFormComponentCode && c.Type == ComponentType.Form);

        if (declarationComponentEntry == null)
        {
            _logger.LogError("Declaration form component definition (Code: {DeclarationCode}) not found for ProcedureId {ProcedureId}", BDocumentConsts.DeclarationFormComponentCode, bDocument.ProcedureId);
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.DeclarationFormComponentNotFound]);
        }
        _logger.LogDebug("Found declaration component: {ComponentName} (Id: {ComponentId})", declarationComponentEntry.Name, declarationComponentEntry.Id);


        var declarationData = bDocument.DocumentData.FirstOrDefault(d => d.ProcedureComponentId == declarationComponentEntry.Id);
        if (declarationData == null || string.IsNullOrWhiteSpace(declarationData.DuLieuNhap))
        {
            _logger.LogError("Declaration form data is missing or empty for BDocument {BDocumentId}, Component {ComponentId}", id, declarationComponentEntry.Id);
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.RequiredComponentDataMissing, declarationComponentEntry.Name]);
        }
        _logger.LogDebug("Found declaration form data (JSON length: {JsonLength})", declarationData.DuLieuNhap.Length);


        // 2. Deserialize JSON (Consider using a specific DTO type if form structure is known/stable)
        Dictionary<string, object?>? formDataObject; // Use Dictionary<string, object?> for flexibility
        try
        {
            // Use relaxed JsonSerializerOptions if needed
            formDataObject = JsonSerializer.Deserialize<Dictionary<string, object?>>(declarationData.DuLieuNhap);
            if (formDataObject == null) throw new JsonException("Deserialized form data dictionary is null.");
            _logger.LogDebug("Successfully deserialized form data JSON.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize declaration form data JSON for BDocument {Id}. JSON: {JsonData}", id, declarationData.DuLieuNhap);
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.InvalidFormDataForDeclaration, ex.Message]);
        }

        // 3. Get Template Content (Placeholder/Example)
        _logger.LogDebug("Getting template content (Placeholder logic)...");
        // Example: var templateBytes = await _fileStorage.GetBytesAsync("templates/declaration_template.docx");
        // var templateContent = Convert.ToBase64String(templateBytes); // Or use bytes directly
        string templatePath = declarationComponentEntry.TempPath ?? string.Empty;
        if (string.IsNullOrWhiteSpace(templatePath))
        {
            _logger.LogError("Template path (TempPath) is not defined for declaration component {ComponentId}", declarationComponentEntry.Id);
            throw new UserFriendlyException("Template path not configured for declaration form."); // Needs localization
        }
        // TODO: Implement actual template fetching logic based on templatePath


        // 4. Render Template (Placeholder/Example)
        byte[] generatedFileBytes;
        string generatedFileName = $"ToKhai_{bDocument.MaHoSo}.pdf"; // Example name, extension depends on renderer
        string mimeType = AbpFileHelper.GetMimeType(generatedFileName); // Use helper for MIME type
        _logger.LogDebug("Rendering template for generated file: {FileName}, MimeType: {MimeType}", generatedFileName, mimeType);
        try
        {
            // TODO: Replace with actual template rendering service call
            // Example: generatedFileBytes = await _templateRenderer.RenderAsync(templateBytes, formDataObject);
            // Simulation:
            var simulationContent = $"Simulated PDF content for {bDocument.MaHoSo}\nData:\n{JsonSerializer.Serialize(formDataObject, new JsonSerializerOptions { WriteIndented = true })}";
            generatedFileBytes = System.Text.Encoding.UTF8.GetBytes(simulationContent);
            _logger.LogInformation("Successfully rendered template (Simulation). Byte length: {ByteLength}", generatedFileBytes.Length);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render declaration file template for BDocument {Id}", id);
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.DeclarationFileGenerationFailed, ex.Message]);
        }

        // 5. Upload Generated File using FileManagement Module
        FileInfoDto createdFileDto; // Use the DTO from FileManagement
        try
        {
            _logger.LogDebug("Uploading generated file to FileManagement...");
            // Use IFileAppService for easier upload with byte[]
            //var createFileInput = new CreateFileInput
            //{
            //    FileContainerName = "bdocuments", // TODO: Make container name configurable
            //    FileName = generatedFileName,
            //    Bytes = generatedFileBytes,
            //    FileType = FileType.RegularFile // Or appropriate type
            //                                    // Optionally set ParentId if organizing files within FileManagement
            //};
            var createFileInput = new CreateFileInput
            {
                FileContainerName = "bdocuments",
                FileName = generatedFileName,
                MimeType = mimeType,
                FileType = FileType.RegularFile,
                ParentId = null,
                OwnerUserId = _currentUser.GetId(),
                Content = generatedFileBytes
            };
            // The CreateAsync returns the DTO directly
            var dataFile = await _fileAppService.CreateAsync(createFileInput);
            createdFileDto = dataFile.FileInfo;
            _logger.LogInformation("Successfully uploaded generated file. New FileId: {FileId}, FileName: {FileName}", createdFileDto.Id, createdFileDto.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload generated declaration file using FileAppService for BDocument {Id}", id);
            // Consider specific exception handling for FileManagement errors
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.FileManagementInteractionFailed, ex.Message]);
        }


        // 6. Update BDocumentData with new FileId using the Manager
        // This ensures consistency and potential domain logic within the manager is applied
        await _bDocumentManager.AddOrUpdateComponentDataAsync(
            bDocument,
            declarationData.ProcedureComponentId,
            declarationData.DuLieuNhap, // Keep original JSON data? Or clear it? Depends on requirements.
            createdFileDto.Id); // Use the ID from the created file DTO

        // Optionally clear DuLieuNhap after successful generation:
        // await _bDocumentManager.AddOrUpdateComponentDataAsync(bDocument, declarationData.ProcedureComponentId, null, createdFileDto.Id);

        await _bDocumentRepository.UpdateAsync(bDocument, autoSave: true); // Save the changes to BDocument
        _logger.LogDebug("Updated BDocumentData with new FileId {FileId} for Component {ComponentId}", createdFileDto.Id, declarationData.ProcedureComponentId);

        // 7. Return File Info DTO from FileManagement
        return createdFileDto;
    }


    [Authorize(CoreFWPermissions.BDocuments.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetBDocumentsInput input)
    {
        _logger.LogInformation("Exporting BDocuments to Excel with filter: {Filter}, ProcedureId: {ProcedureId}, StatusId: {StatusId}", input.Filter, input.ProcedureId, input.StatusId);
        if (_excelExportHelper == null)
        {
            _logger.LogError("Excel export service (IAbpExcelExportHelper) is not configured/injected.");
            throw new UserFriendlyException("Excel export service is not configured."); // Needs localization
        }

        // Get all matching documents (no pagination for export)
        var bDocuments = await _bDocumentRepository.GetListAsync(
            filterText: input.Filter,
            procedureId: input.ProcedureId,
            trangThaiHoSoId: input.StatusId,
            ngayNopFrom: input.SubmissionDateFrom,
            ngayNopTo: input.SubmissionDateTo,
            // TODO: Add filter for DangKyNhanQuaBuuDien if needed
            sorting: input.Sorting ?? $"{nameof(BDocument.CreationTime)} DESC",
            maxResultCount: int.MaxValue, // Get all matching records
            skipCount: 0,
            includeDetails: false // Details usually not needed for export list
        );
        _logger.LogDebug("Fetched {DocumentCount} BDocuments for Excel export.", bDocuments.Count);


        var excelDtos = new List<BDocumentExcelDto>();
        if (bDocuments.Any())
        {
            var procedureIds = bDocuments.Select(d => d.ProcedureId).Distinct().ToList();
            var statusIds = bDocuments.Where(d => d.TrangThaiHoSoId.HasValue).Select(d => d.TrangThaiHoSoId!.Value).Distinct().ToList();

            // Fetch related data efficiently
            var procedures = (await _procedureRepository.GetListByIdsAsync(ids: procedureIds)).ToDictionary(p => p.Id);
            var statuses = new Dictionary<Guid, WorkflowStatus>();
            if (statusIds.Any()) statuses = (await _statusRepository.GetListByIdsAsync(ids: statusIds)).ToDictionary(s => s.Id);
            _logger.LogDebug("Fetched {ProcedureCount} procedures and {StatusCount} statuses for Excel enrichment.", procedures.Count, statuses.Count);


            // Map using AutoMapper first
            excelDtos = ObjectMapper.Map<List<BDocument>, List<BDocumentExcelDto>>(bDocuments);

            // Enrich names and format boolean in the loop
            foreach (var dto in excelDtos)
            {
                // Find the original entity to get IDs (Mapping might lose context)
                //var bDoc = bDocuments.Find(b => b.Id == dto.Id); // Assuming ExcelDto inherits from EntityDto<Guid> or has Id

                if (procedures.TryGetValue(dto.ProcedureId, out var p)) dto.ProcedureName = p.Name;
                if (dto.TrangThaiHoSoId.HasValue && statuses.TryGetValue(dto.TrangThaiHoSoId.Value, out var s)) dto.StatusName = s.Name;
                else dto.StatusName = _localizer["NotSet"] ?? "Not Set"; // Handle null status

                // Boolean formatting already handled by AutoMapper Profile in this plan
                // dto.DangKyNhanQuaBuuDien = bDoc.DangKyNhanQuaBuuDien ? (_localizer["Yes"] ?? "Yes") : (_localizer["No"] ?? "No");
            }
        }
        else
        {
            _logger.LogInformation("No BDocuments found matching the criteria for Excel export.");
        }

        // Use the helper to generate the Excel file content
        var fileContent = await _excelExportHelper.ExportToExcelAsync(excelDtos, "BDocuments"); // Sheet name
        _logger.LogInformation("Generated Excel file content for BDocuments.");
        return fileContent;
    }

    // --- Private Helper Methods ---

    /// <summary>
    /// Enriches a single BDocumentData entity into its DTO representation.
    /// </summary>
    private async Task<BDocumentDataDto> EnrichBDocumentDataDtoAsync(BDocumentData data)
    {
        var dto = ObjectMapper.Map<BDocumentData, BDocumentDataDto>(data);
        _logger.LogTrace("Mapping BDocumentData Id: {BDocumentDataId} to DTO.", data.Id);

        // Enrich Component Info
        var component = await _componentRepository.FindAsync(data.ProcedureComponentId);
        if (component != null)
        {
            dto.ComponentCode = component.Code;
            dto.ComponentName = component.Name;
            dto.ComponentType = component.Type;
            // TODO: Fetch IsRequired from link table if needed
            // dto.IsRequired = await GetIsRequiredFlag(data.BDocumentId, data.ProcedureComponentId);
            _logger.LogTrace("Enriched component info for DTO (Code: {ComponentCode})", dto.ComponentCode);
        }
        else
        {
            _logger.LogWarning("Component definition not found for ProcedureComponentId: {ProcedureComponentId} while enriching BDocumentDataDto Id: {BDocumentDataId}", data.ProcedureComponentId, data.Id);
        }

        // Enrich File Info
        if (data.FileId.HasValue)
        {
            _logger.LogTrace("Fetching FileInfo for FileId: {FileId}", data.FileId.Value);
            try
            {
                // Use FileAppService to get the DTO
                var fileDtoFm = await _fileAppService.GetAsync(data.FileId.Value);
                // Assign directly if BDocumentDataDto uses FileManagement's DTO
                dto.FileInfo = fileDtoFm;
                // Or map if using a custom contract DTO:
                // dto.FileInfo = ObjectMapper.Map<EasyAbp.FileManagement.Files.Dtos.FileInfoDto, YourContract.FileInfoDto>(fileDtoFm);
                _logger.LogTrace("Successfully fetched FileInfo for FileId: {FileId}", data.FileId.Value);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogWarning("File with Id {FileId} not found in FileManagement for BDocumentData {BDocumentDataId}", data.FileId.Value, data.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching FileInfo from FileAppService for FileId {FileId} in BDocumentData {BDocumentDataId}", data.FileId.Value, data.Id);
                // Decide if this error should prevent returning the DTO or just log
            }
        }
        return dto;
    }

    // TODO: Implement if needed for GetAsync enrichment
    // private async Task<bool> GetIsRequiredFlag(Guid procedureId, Guid procedureComponentId) { ... }


    // TODO: Implement detailed validation logic if needed
    // private void ValidateDeclarationFormData(string? jsonData, string? formDefinitionJson) {
    //    if (string.IsNullOrWhiteSpace(jsonData)) {
    //        // Throw validation exception or handle as needed
    //        _logger.LogWarning("Declaration form data (JSON) is null or whitespace.");
    //        // throw new AbpValidationException("Declaration form data cannot be empty."); // Example
    //        return; // Or proceed depending on requirements
    //    }
    //    if (string.IsNullOrWhiteSpace(formDefinitionJson)) {
    //         _logger.LogWarning("FormDefinition JSON is missing, cannot perform detailed validation.");
    //         return; // Cannot validate without definition
    //    }
    //    _logger.LogDebug("Performing detailed validation for declaration form data against definition.");
    //    // Example Logic:
    //    // 1. Deserialize formDefinitionJson into a structure (e.g., List<FormFieldDefinition>)
    //    // 2. Deserialize jsonData into Dictionary<string, JsonElement?> or similar
    //    // 3. Iterate through field definitions:
    //    //    - Check 'required' fields exist in jsonData
    //    //    - Check data types (string, number, boolean, date) match definition
    //    //    - Check string lengths, number ranges, etc. based on definition properties
    //    //    - Use _validationErrorHandler or throw AbpValidationException on errors
    // }

    // Placeholder/Example for template rendering logic
    // private async Task<byte[]> RenderTemplateAsync(byte[] templateBytes, Dictionary<string, object?> data) {
    //    _logger.LogInformation("Rendering document from template...");
    //    // Use a library like NPOI (for DOCX), iTextSharp/QuestPDF (for PDF), etc.
    //    // Example: var renderedBytes = await _documentGenerator.GenerateFromTemplateAsync(templateBytes, data);
    //    // return renderedBytes;
    //    await Task.Delay(100); // Simulate work
    //    return System.Text.Encoding.UTF8.GetBytes($"Rendered content with data: {JsonSerializer.Serialize(data)}");
    // }
    // Placeholder/Example for fetching template bytes
    // private async Task<byte[]> GetTemplateBytesAsync(string templatePath) {
    //     _logger.LogDebug("Fetching template bytes from path: {TemplatePath}", templatePath);
    //     // Implement logic to read template file from storage (e.g., blob storage, local file system)
    //     // Example: return await _blobContainer.ReadBytesAsync(templatePath);
    //     await Task.Delay(50); // Simulate work
    //     return System.Text.Encoding.UTF8.GetBytes("Fake template content");
    // }
}