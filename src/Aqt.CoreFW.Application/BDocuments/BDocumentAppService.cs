using Aqt.CoreFW.Application.Contracts.BDocuments; // Service Interface
using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos; // DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // **Added for Lookups**
using Aqt.CoreFW.BDocuments; // Constants
// using Aqt.CoreFW.Domain.Shared.Components.Enums; // **Removed to check where ComponentType is needed**
using Aqt.CoreFW.Domain.BDocuments; // Domain Service and Repository
using Aqt.CoreFW.Domain.BDocuments.Entities; // Entities
using Aqt.CoreFW.Domain.Components; // Component Repository & Entity
using Aqt.CoreFW.Domain.Components.Entities;
using Aqt.CoreFW.Domain.Procedures; // Procedure Repository & Entity
using Aqt.CoreFW.Domain.Procedures.Entities;
using Aqt.CoreFW.Domain.WorkflowStatuses; // Status Repository & Entity
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities;
using Aqt.CoreFW.Helpers;
using Aqt.CoreFW.Localization; // Resource
using Aqt.CoreFW.Permissions; // Permissions
using Aqt.CoreFW.Shared.Services; // Add logging
using EasyAbp.FileManagement.Files; // IFileAppService, IFileManager, File Entity, CreateFileInput
using EasyAbp.FileManagement.Files.Dtos; // Use FileInfoDto, CreateFileInput, CreateFileOutput from FileManagement
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging; // Add logging
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // For JSON handling
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Entities; // For EntityNotFoundException
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using System.Reflection; // **Added for Reflection**
using Aqt.CoreFW.Procedures; // **Using namespace provided by user for Enum**
using Aqt.CoreFW.Application.Contracts.Procedures; // **Added for IProcedureAppService**
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.BDocuments.Dtos; // Assuming ProcedureLookupDto is here or adjust as needed
// using Aqt.CoreFW.Domain.Shared.Enums; // **Commented out - Incorrect namespace**

namespace Aqt.CoreFW.Application.BDocuments;

[Authorize(CoreFWPermissions.BDocuments.Default)]
public class BDocumentAppService : ApplicationService, IBDocumentAppService
{
    private readonly IBDocumentRepository _bDocumentRepository;
    private readonly BDocumentManager _bDocumentManager;
    private readonly IProcedureAppService _procedureAppService;
    private readonly IRepository<WorkflowStatus, Guid> _workflowStatusRepository;
    private readonly IProcedureComponentRepository _componentRepository;
    private readonly IFileAppService _fileAppService;
    private readonly IFileManager _fileManager;
    private readonly IFileRepository _fileRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper _excelExportHelper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<BDocumentAppService> _logger;
    private readonly IClock _clock;

    public BDocumentAppService(
        IBDocumentRepository bDocumentRepository,
        BDocumentManager bDocumentManager,
        IProcedureAppService procedureAppService,
        IRepository<WorkflowStatus, Guid> workflowStatusRepository,
        IProcedureComponentRepository componentRepository,
        IFileAppService fileAppService,
        IFileManager fileManager,
        IFileRepository fileRepository,
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> localizer,
        ILogger<BDocumentAppService> logger,
        IAbpExcelExportHelper excelExportHelper,
        ICurrentUser currentUser,
        IClock clock)
    {
        _bDocumentRepository = bDocumentRepository;
        _bDocumentManager = bDocumentManager;
        _procedureAppService = procedureAppService;
        _workflowStatusRepository = workflowStatusRepository;
        _componentRepository = componentRepository;
        _fileAppService = fileAppService;
        _fileManager = fileManager;
        _fileRepository = fileRepository;
        _guidGenerator = guidGenerator;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;
        _logger = logger;
        _currentUser = currentUser;
        _clock = clock;
        LocalizationResource = typeof(CoreFWResource);
    }

    [Authorize(CoreFWPermissions.BDocuments.Create)]
    [UnitOfWork]
    public async Task<BDocumentDto> CreateAsync(CreateUpdateBDocumentDto input)
    {
        _logger.LogInformation("Starting BDocument creation for ProcedureId: {ProcedureId}", input.ProcedureId);
        string code = "HS-" + _clock.Now.ToString("yyyyMMddHHmmssfff");
        _logger.LogDebug("Generated Code: {Code}", code);

        var entity = await _bDocumentManager.CreateAsync(
            input.ProcedureId,
            code,
            input.ApplicantName,
            input.ApplicantIdentityNumber,
            input.ApplicantAddress,
            input.ApplicantEmail,
            input.ApplicantPhoneNumber,
            input.ScopeOfActivity,
            input.ReceiveByPost,
            input.SubmissionDate
        );

        if (input.DocumentData != null)
        {
            foreach (var dataInput in input.DocumentData)
            {
                await _bDocumentManager.AddOrUpdateComponentDataAsync(
                    entity,
                    dataInput.ProcedureComponentId,
                    dataInput.InputData,
                    dataInput.FileId
                 );
                 _logger.LogDebug("Added/Updated data for component {ComponentId} to entity (in memory)", dataInput.ProcedureComponentId);
            }
        }

        await _bDocumentRepository.InsertAsync(entity, autoSave: true);
        _logger.LogInformation("Successfully inserted BDocument with Id: {BDocumentId}", entity.Id);
        return await GetAsync(entity.Id);
    }

    [Authorize(CoreFWPermissions.BDocuments.Update)]
    [UnitOfWork]
    public async Task<BDocumentDto> UpdateAsync(Guid id, CreateUpdateBDocumentDto input)
    {
        _logger.LogInformation("Starting BDocument update for Id: {BDocumentId}", id);
        var entity = await _bDocumentRepository.GetWithDataAsync(id)
            ?? throw new EntityNotFoundException(typeof(BDocument), id);

        await _bDocumentManager.UpdateInfoAsync(
            entity,
            input.ApplicantName,
            input.ApplicantIdentityNumber,
            input.ApplicantAddress,
            input.ApplicantEmail,
            input.ApplicantPhoneNumber,
            input.ScopeOfActivity,
            input.ReceiveByPost
        );
        _logger.LogDebug("Updated BDocument entity info for Id: {BDocumentId}", id);

        input.DocumentData ??= new List<CreateUpdateBDocumentDataDto>();
        var inputComponentIds = input.DocumentData.Select(i => i.ProcedureComponentId).ToList();
        var existingComponentIds = entity.DocumentData.Select(d => d.ProcedureComponentId).ToList();

        var componentsToRemove = existingComponentIds.Except(inputComponentIds).ToList();
        foreach (var componentIdToRemove in componentsToRemove)
        {
            await _bDocumentManager.RemoveComponentDataAsync(entity, componentIdToRemove, true /* Assume file deletion */);
            _logger.LogDebug("Removed data for component {ComponentId} from entity (in memory)", componentIdToRemove);
        }

        foreach (var dataInput in input.DocumentData)
        {
             await _bDocumentManager.AddOrUpdateComponentDataAsync(
                 entity,
                 dataInput.ProcedureComponentId,
                 dataInput.InputData,
                 dataInput.FileId
             );
             _logger.LogDebug("Added/Updated data for component {ComponentId} to entity (in memory)", dataInput.ProcedureComponentId);
        }

        await _bDocumentRepository.UpdateAsync(entity, autoSave: true);
        _logger.LogInformation("Successfully updated BDocument with Id: {BDocumentId}", id);
        return await GetAsync(entity.Id);
    }

    [Authorize(CoreFWPermissions.BDocuments.Default)]
    public async Task<BDocumentDto> GetAsync(Guid id)
    {
        _logger.LogDebug("Fetching BDocument with Id: {BDocumentId}", id);
        var bDocument = await _bDocumentRepository.GetWithDataAsync(id)
            ?? throw new EntityNotFoundException(typeof(BDocument), id);

        var dto = ObjectMapper.Map<BDocument, BDocumentDto>(bDocument);
        _logger.LogDebug("Mapped BDocument to BDocumentDto for Id: {BDocumentId}", id);

        await EnrichDocumentDataListAsync(id, dto.DocumentData);

        return dto;
    }

    [Authorize(CoreFWPermissions.BDocuments.Default)]
    public async Task<PagedResultDto<BDocumentListDto>> GetListAsync(GetBDocumentsInput input)
    {
        _logger.LogInformation("Getting BDocument list with filter: {Filter}, ProcedureId: {ProcedureId}, StatusId: {WorkflowStatusId}", input.Filter, input.ProcedureId, input.WorkflowStatusId);

        var totalCount = await _bDocumentRepository.GetCountAsync(
            filterText: input.Filter,
            procedureId: input.ProcedureId,
            workflowStatusId: input.WorkflowStatusId,
            submissionDateFrom: input.SubmissionDateFrom,
            submissionDateTo: input.SubmissionDateTo
        );
        _logger.LogDebug("Total count: {Count}", totalCount);

        if (totalCount == 0) { return new PagedResultDto<BDocumentListDto>(0, new List<BDocumentListDto>()); }

        var bDocuments = await _bDocumentRepository.GetListAsync(
            filterText: input.Filter,
            procedureId: input.ProcedureId,
            workflowStatusId: input.WorkflowStatusId,
            submissionDateFrom: input.SubmissionDateFrom,
            submissionDateTo: input.SubmissionDateTo,
            sorting: input.Sorting ?? $"{nameof(BDocument.CreationTime)} DESC",
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            includeDetails: true
        );
        _logger.LogDebug("Fetched {Count} BDocuments for the current page.", bDocuments.Count);

        var dtos = ObjectMapper.Map<List<BDocument>, List<BDocumentListDto>>(bDocuments);
        return new PagedResultDto<BDocumentListDto>(totalCount, dtos);
    }

    [Authorize(CoreFWPermissions.BDocuments.Delete)]
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Attempting to delete BDocument with Id: {BDocumentId}", id);
        var entity = await _bDocumentRepository.GetWithDataAsync(id);
        if (entity != null)
        {
            foreach (var data in entity.DocumentData.ToList())
            {
                if (data.FileId.HasValue)
                {
                    try
                    {
                        var fileToDelete = await _fileRepository.FindAsync(data.FileId.Value);
                        if (fileToDelete != null)
                        {
                             await _fileManager.DeleteAsync(fileToDelete);
                             _logger.LogDebug("Deleted associated file {FileId} for BDocument {BDocumentId}", data.FileId.Value, id);
                        }
                        else { _logger.LogWarning("Could not find file entity with Id {FileId} to delete.", data.FileId.Value); }
                    }
                    catch (Exception ex)
                    {
                         _logger.LogError(ex, "Failed to delete associated file {FileId} while deleting BDocument {BDocumentId}", data.FileId.Value, id);
                    }
                }
            }
        }
        await _bDocumentRepository.DeleteAsync(id, autoSave: true);
        _logger.LogInformation("Successfully soft-deleted BDocument with Id: {BDocumentId}", id);
    }

    [Authorize(CoreFWPermissions.BDocuments.Update)]
    [UnitOfWork]
    public async Task<BDocumentDataDto> AddOrUpdateDataAsync(Guid bDocumentId, AddOrUpdateBDocumentDataInputDto input)
    {
        var bDocument = await _bDocumentRepository.GetWithDataAsync(bDocumentId)
            ?? throw new EntityNotFoundException(typeof(BDocument), bDocumentId);

        await _bDocumentManager.AddOrUpdateComponentDataAsync(
            bDocument,
            input.ProcedureComponentId,
            input.InputData,
            input.FileId
        );

        var updatedDataEntry = bDocument.DocumentData.FirstOrDefault(d => d.ProcedureComponentId == input.ProcedureComponentId)
            ?? throw new AbpException($"Failed to find BDocumentData for component {input.ProcedureComponentId} after update/add in BDocument {bDocumentId}.");

        _logger.LogInformation("Successfully added/updated data for component {ComponentId} in BDocument {BDocumentId}", input.ProcedureComponentId, bDocumentId);

        return await EnrichBDocumentDataDtoAsync(updatedDataEntry);
    }

    [Authorize(CoreFWPermissions.BDocuments.Update)]
    [UnitOfWork]
    public async Task RemoveDataAsync(Guid bDocumentId, Guid bDocumentDataId, bool deleteAssociatedFile = false)
    {
        var bDocument = await _bDocumentRepository.GetWithDataAsync(bDocumentId)
           ?? throw new EntityNotFoundException(typeof(BDocument), bDocumentId);

        var dataToRemove = bDocument.DocumentData.FirstOrDefault(d => d.Id == bDocumentDataId);

        if (dataToRemove == null)
        {
            _logger.LogWarning("BDocumentData with Id {BDocumentDataId} not found in BDocument {BDocumentId}. Cannot remove.", bDocumentDataId, bDocumentId);
            return;
        }

        var procedureComponentId = dataToRemove.ProcedureComponentId;

        await _bDocumentManager.RemoveComponentDataAsync(
            bDocument,
            procedureComponentId,
            deleteAssociatedFile
        );
        _logger.LogInformation("Successfully removed data (BDocumentDataId: {BDocumentDataId}, ComponentId: {ComponentId}) from BDocument {BDocumentId}. Delete file: {DeleteFile}",
            bDocumentDataId, procedureComponentId, bDocumentId, deleteAssociatedFile);
    }

    [Authorize(CoreFWPermissions.BDocuments.Update)]
    [UnitOfWork]
    public async Task<BDocumentDto> ChangeStatusAsync(Guid id, ChangeBDocumentStatusInputDto input)
    {
        _logger.LogInformation("Changing status for BDocument Id: {BDocumentId} to StatusId: {NewStatusId}", id, input.NewStatusId);

        var bDocument = await _bDocumentRepository.GetAsync(id)
            ?? throw new EntityNotFoundException(typeof(BDocument), id);

        await _bDocumentManager.ChangeStatusAsync(
            bDocument,
            input.NewStatusId,
            input.Reason
        );

        await _bDocumentRepository.UpdateAsync(bDocument, autoSave: true);
        _logger.LogInformation("Successfully changed status for BDocument Id: {BDocumentId}", id);
        return await GetAsync(id);
    }

    [Authorize(CoreFWPermissions.BDocuments.Update)]
    [UnitOfWork]
    public async Task<FileInfoDto> GenerateDeclarationFileAsync(Guid id)
    {
        _logger.LogInformation("Starting declaration file generation for BDocument Id: {BDocumentId}", id);
        var bDocument = await _bDocumentRepository.GetWithDataAsync(id)
            ?? throw new EntityNotFoundException(typeof(BDocument), id);

        _logger.LogDebug("Looking for declaration component for ProcedureId: {ProcedureId}", bDocument.ProcedureId);

        var procedureComponentsList = await _componentRepository.GetListByProcedureAsync(bDocument.ProcedureId);
        var procedureComponents = procedureComponentsList.ToDictionary(c => c.Id);

        var declarationComponentEntry = procedureComponents.Values
                                      .FirstOrDefault(c => c.Code == BDocumentConsts.DeclarationFormComponentCode && c.Type == CoreFW.Components.ComponentType.Form);

        if (declarationComponentEntry == null)
        {
             _logger.LogError("Declaration form component definition not found for ProcedureId {ProcedureId} (or query failed)", bDocument.ProcedureId);
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.DeclarationFormComponentNotFound]);
        }
         _logger.LogDebug("Found declaration component: {Name} (Id: {Id})", declarationComponentEntry.Name, declarationComponentEntry.Id);


        var declarationData = bDocument.DocumentData.FirstOrDefault(d => d.ProcedureComponentId == declarationComponentEntry.Id);
        if (declarationData == null || string.IsNullOrWhiteSpace(declarationData.InputData))
        {
             _logger.LogError("Required declaration form data is missing for BDocument {Id}, Component {CompName}", id, declarationComponentEntry.Name);
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.RequiredComponentDataMissing, declarationComponentEntry.Name]);
        }
        _logger.LogDebug("Found declaration form data (JSON length: {Length})", declarationData.InputData.Length);

        Dictionary<string, object?>? formDataObject = null; // Initialize to null
        // **Temporarily commenting out the catch block due to missing CoreFWDomainErrorCodes.InvalidFormDataForDeclaration**
        /*
        try
        {
            formDataObject = JsonSerializer.Deserialize<Dictionary<string, object?>>(declarationData.InputData);
            if (formDataObject == null) throw new JsonException("Deserialized form data dictionary is null.");
            Logger.LogDebug("Successfully deserialized form data JSON.");
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, "Failed to deserialize declaration form data JSON for BDocument {Id}. JSON: {JsonData}", id, declarationData.InputData);

            // Check if error code exists before using, otherwise use default string
            string errorCode = CoreFWDomainErrorCodes.InvalidFormDataForDeclaration; // This line causes compile error if const doesn't exist
            var fieldInfo = typeof(CoreFWDomainErrorCodes).GetField(nameof(CoreFWDomainErrorCodes.InvalidFormDataForDeclaration), BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
            {
                Logger.LogWarning("Domain error code '{ErrorCodeName}' not found in CoreFWDomainErrorCodes. Using default string.", nameof(CoreFWDomainErrorCodes.InvalidFormDataForDeclaration));
                errorCode = "Error.InvalidFormData";
            }

            throw new UserFriendlyException(_localizer[errorCode, ex.Message], innerException: ex);
        }
        */
         // **Need to handle the case where deserialization fails if catch block is commented out**
         // For now, proceed assuming deserialization worked (will throw later if formDataObject is null)
         // A better approach after fixing the error code is to keep the try-catch.
         try {
             formDataObject = JsonSerializer.Deserialize<Dictionary<string, object?>>(declarationData.InputData);
              if (formDataObject == null) throw new JsonException("Deserialized form data dictionary is null.");
         } catch (Exception ex) {
              Logger.LogError(ex, "Failed to deserialize declaration form data JSON for BDocument {Id}. JSON: {JsonData}", id, declarationData.InputData);
               // **Corrected UserFriendlyException call using .Value**
               throw new UserFriendlyException(L["Error.InvalidFormData"].Value, innerException: ex);
         }

        _logger.LogDebug("Getting template content (Placeholder logic)...");
         string templatePath = declarationComponentEntry.TempPath ?? string.Empty;
         if (string.IsNullOrWhiteSpace(templatePath))
         {
             _logger.LogError("Template path not configured for declaration component {ComponentId}", declarationComponentEntry.Id);
             throw new UserFriendlyException("Template path not configured for declaration generation.");
         }
        byte[] generatedFileBytes;
        string generatedFileName = $"ToKhai_{bDocument.Code}.pdf";
        string mimeType = AbpFileHelper.GetMimeType(generatedFileName);
        _logger.LogDebug("Rendering template for generated file: {Name}, MimeType: {Type}", generatedFileName, mimeType);
        try
        {
            var simulationContent = $"Simulated PDF content for {bDocument.Code}\nData:\n{JsonSerializer.Serialize(formDataObject, new JsonSerializerOptions { WriteIndented = true })}";
            generatedFileBytes = System.Text.Encoding.UTF8.GetBytes(simulationContent);
            _logger.LogInformation("Successfully rendered template (Simulation). Byte length: {Length}", generatedFileBytes.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed during declaration file generation (simulation) for BDocument {Id}", id);
             // **Check if error code exists before using, otherwise use default string**
            string errorCodeGeneration = CoreFWDomainErrorCodes.DeclarationFileGenerationFailed;
            var fieldInfoGeneration = typeof(CoreFWDomainErrorCodes).GetField(nameof(CoreFWDomainErrorCodes.DeclarationFileGenerationFailed), BindingFlags.Public | BindingFlags.Static);
            if (fieldInfoGeneration == null)
            {
                _logger.LogWarning("Domain error code '{ErrorCodeName}' not found in CoreFWDomainErrorCodes. Using default string.", nameof(CoreFWDomainErrorCodes.DeclarationFileGenerationFailed));
                errorCodeGeneration = "Error.FileGenerationFailed";
            }
            throw new UserFriendlyException(_localizer[errorCodeGeneration, ex.Message], innerException: ex);
        }

        CreateFileOutput createdFileResult;
        try
        {
            _logger.LogDebug("Uploading generated file to FileManagement...");
            var createFileInput = new CreateFileInput
            {
                FileContainerName = BDocumentConsts.FileContainerName,
                FileName = generatedFileName,
                MimeType = mimeType,
                FileType = FileType.RegularFile,
                ParentId = null,
                OwnerUserId = _currentUser.GetId(),
                Content = generatedFileBytes
            };
             createdFileResult = await _fileAppService.CreateAsync(createFileInput);
            _logger.LogInformation("Successfully uploaded generated file. New FileId: {Id}, FileName: {Name}", createdFileResult.FileInfo.Id, createdFileResult.FileInfo.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload generated declaration file to FileManagement for BDocument {Id}", id);
            // **Check if error code exists before using, otherwise use default string**
            string errorCodeUpload = CoreFWDomainErrorCodes.FileManagementInteractionFailed;
             var fieldInfoUpload = typeof(CoreFWDomainErrorCodes).GetField(nameof(CoreFWDomainErrorCodes.FileManagementInteractionFailed), BindingFlags.Public | BindingFlags.Static);
            if (fieldInfoUpload == null)
            {
                _logger.LogWarning("Domain error code '{ErrorCodeName}' not found in CoreFWDomainErrorCodes. Using default string.", nameof(CoreFWDomainErrorCodes.FileManagementInteractionFailed));
                errorCodeUpload = "Error.FileManagementFailed";
            }
            throw new UserFriendlyException(_localizer[errorCodeUpload, ex.Message], innerException: ex);
        }

        await _bDocumentManager.AddOrUpdateComponentDataAsync(
            bDocument,
            declarationData.ProcedureComponentId,
            declarationData.InputData,
            createdFileResult.FileInfo.Id
        );
        _logger.LogDebug("Updated BDocumentData with new FileId {Id} for Component {CompId}", createdFileResult.FileInfo.Id, declarationData.ProcedureComponentId);

        return createdFileResult.FileInfo;
    }

    [Authorize(CoreFWPermissions.BDocuments.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetBDocumentsInput input)
    {
        _logger.LogInformation("Exporting BDocuments to Excel with filter: {Filter}, ProcedureId: {ProcedureId}, StatusId: {WorkflowStatusId}", input.Filter, input.ProcedureId, input.WorkflowStatusId);
        if (_excelExportHelper == null)
        {
             _logger.LogError("IAbpExcelExportHelper is not registered or injected correctly.");
             throw new UserFriendlyException("Excel export functionality is not available.");
        }

        var bDocuments = await _bDocumentRepository.GetListAsync(
            filterText: input.Filter,
            procedureId: input.ProcedureId,
            workflowStatusId: input.WorkflowStatusId,
            submissionDateFrom: input.SubmissionDateFrom,
            submissionDateTo: input.SubmissionDateTo,
            sorting: input.Sorting ?? $"{nameof(BDocument.CreationTime)} DESC",
            maxResultCount: int.MaxValue,
            skipCount: 0,
            includeDetails: true
        );
        _logger.LogDebug("Fetched {Count} BDocuments for Excel export.", bDocuments.Count);

        var excelDtos = ObjectMapper.Map<List<BDocument>, List<BDocumentExcelDto>>(bDocuments);

        var fileContent = await _excelExportHelper.ExportToExcelAsync(excelDtos, "Hồ sơ");
        _logger.LogInformation("Generated Excel file content for BDocuments.");
        return fileContent;
    }

    [Authorize(CoreFWPermissions.BDocuments.Default)]
    public async Task<ListResultDto<ProcedureLookupDto>> GetProcedureLookupAsync()
    {
         Logger.LogDebug("Fetching Procedure lookup via ProcedureAppService.");
         // Call the injected ProcedureAppService
         // Assuming GetLookupAsync exists and handles necessary filtering
         return await _procedureAppService.GetLookupAsync();
    }

    [Authorize(CoreFWPermissions.BDocuments.Default)]
    public async Task<ListResultDto<LookupDto<Guid>>> GetWorkflowStatusLookupAsync()
    {
         _logger.LogDebug("Fetching Workflow Status lookup.");
        var statuses = await _workflowStatusRepository.GetListAsync(ws => ws.IsActive);
        var lookupDtos = ObjectMapper.Map<List<WorkflowStatus>, List<LookupDto<Guid>>>(statuses.OrderBy(ws => ws.Order).ToList());
         _logger.LogDebug("Returning {Count} workflow statuses in lookup.", lookupDtos.Count);
        return new ListResultDto<LookupDto<Guid>>(lookupDtos);
    }

    [Authorize(CoreFWPermissions.BDocuments.Default)]
    public async Task<ListResultDto<ProcedureComponentLookupDto>> GetComponentListForProcedureAsync(Guid procedureId)
    {
        _logger.LogDebug("Fetching Component lookup for ProcedureId: {ProcedureId}", procedureId);
        var components = await _componentRepository.GetListByProcedureAsync(procedureId);
        var lookupDtos = ObjectMapper.Map<List<ProcedureComponent>, List<ProcedureComponentLookupDto>>(components.OrderBy(c => c.Order).ToList());
        _logger.LogDebug("Returning {Count} components in lookup for Procedure {ProcedureId}.", lookupDtos.Count, procedureId);
        return new ListResultDto<ProcedureComponentLookupDto>(lookupDtos);
    }

    private async Task EnrichDocumentDataListAsync(Guid bDocumentId, List<BDocumentDataDto> dataDtos)
    {
        if (!dataDtos.Any()) return;

        _logger.LogDebug("Enriching {Count} BDocumentData items for BDocument {BDocumentId}.", dataDtos.Count, bDocumentId);
        var componentIds = dataDtos.Select(d => d.ProcedureComponentId).Distinct().ToList();
        var components = (await _componentRepository.GetListAsync(x => componentIds.Contains(x.Id)))
            .ToDictionary(c => c.Id);
        _logger.LogDebug("Fetched {Count} ProcedureComponent definitions.", components.Count);

        foreach (var dataDto in dataDtos)
        {
            if (components.TryGetValue(dataDto.ProcedureComponentId, out var component))
            {
                dataDto.ComponentCode = component.Code;
                dataDto.ComponentName = component.Name;
                dataDto.ComponentType = component.Type;
                _logger.LogTrace("Enriched component info for ProcedureComponentId: {Id}", dataDto.ProcedureComponentId);
            }
            else { _logger.LogWarning("Component definition not found for ProcedureComponentId: {Id} in BDocument {DocId}", dataDto.ProcedureComponentId, bDocumentId); }

            if (dataDto.FileId.HasValue)
            {
                _logger.LogTrace("Fetching FileInfo for FileId: {FileId}", dataDto.FileId.Value);
                try
                {
                    dataDto.FileInfo = await _fileAppService.GetAsync(dataDto.FileId.Value);
                    _logger.LogTrace("Successfully fetched FileInfo for FileId: {FileId}", dataDto.FileId.Value);
                }
                catch (EntityNotFoundException)
                {
                    _logger.LogWarning("File with Id {FileId} not found in FileManagement for BDocumentData {BDocumentDataId}", dataDto.FileId.Value, dataDto.Id);
                     dataDto.FileInfo = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching FileInfo from FileAppService for FileId {FileId} in BDocumentData {BDocumentDataId}", dataDto.FileId.Value, dataDto.Id);
                     dataDto.FileInfo = null;
                }
            } else {
                 dataDto.FileInfo = null;
            }
        }
    }

    private async Task<BDocumentDataDto> EnrichBDocumentDataDtoAsync(BDocumentData data)
    {
        var dto = ObjectMapper.Map<BDocumentData, BDocumentDataDto>(data);
        await EnrichDocumentDataListAsync(data.BDocumentId, new List<BDocumentDataDto> { dto });
        return dto;
    }
}