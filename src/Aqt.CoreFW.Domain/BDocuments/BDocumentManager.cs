using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.BDocuments; // Consts
using Aqt.CoreFW.Domain.BDocuments.Entities;
using Aqt.CoreFW.Domain.Procedures; // Procedure Repositories/Entities
using Aqt.CoreFW.Domain.Procedures.Entities;
using Aqt.CoreFW.Domain.WorkflowStatuses; // WorkflowStatus Repositories/Entities
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities;
using Aqt.CoreFW.Domain.Components; // Component Repositories/Entities
using Aqt.CoreFW.Domain.Components.Entities;
using EasyAbp.FileManagement.Files; // IFileManager
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Microsoft.Extensions.Localization;
using Aqt.CoreFW.Localization; // CoreFWResource
using Microsoft.Extensions.Logging; // ILogger
using Microsoft.Extensions.Logging.Abstractions;
using Aqt.CoreFW.Components; // NullLogger

namespace Aqt.CoreFW.Domain.BDocuments;

/// <summary>
/// Domain service for managing BDocument entities and related business logic.
/// </summary>
public class BDocumentManager : DomainService
{
    private readonly IBDocumentRepository _bDocumentRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IWorkflowStatusRepository _workflowStatusRepository;
    private readonly IProcedureComponentRepository _procedureComponentRepository; // Đổi tên biến cho nhất quán
    private readonly IFileManager _fileManager;
    private readonly IFileRepository _fileRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IStringLocalizer<CoreFWResource> _localizer; // Inject Localizer

    public BDocumentManager(
        IBDocumentRepository bDocumentRepository,
        IProcedureRepository procedureRepository,
        IWorkflowStatusRepository workflowStatusRepository,
        IProcedureComponentRepository procedureComponentRepository, // Đổi tên tham số
        IFileManager fileManager,
        IFileRepository fileRepository,
        IGuidGenerator guidGenerator,
        IUnitOfWorkManager unitOfWorkManager,
        IStringLocalizer<CoreFWResource> localizer) // Inject Localizer
    {
        _bDocumentRepository = bDocumentRepository;
        _procedureRepository = procedureRepository;
        _workflowStatusRepository = workflowStatusRepository;
        _procedureComponentRepository = procedureComponentRepository; // Gán giá trị
        _fileManager = fileManager;
        _fileRepository = fileRepository;
        _guidGenerator = guidGenerator;
        _unitOfWorkManager = unitOfWorkManager;
        _localizer = localizer; // Gán Localizer
    }

    /// <summary>
    /// Creates a new BDocument entity.
    /// </summary>
    public async Task<BDocument> CreateAsync(
        Guid procedureId,
        [NotNull] string maHoSo,
        [NotNull] string tenChuHoSo,
        [CanBeNull] string? soDinhDanhChuHoSo = null,
        [CanBeNull] string? diaChiChuHoSo = null,
        [CanBeNull] string? emailChuHoSo = null,
        [CanBeNull] string? soDienThoaiChuHoSo = null,
        [CanBeNull] string? phamViHoatDong = null, // MỚI
        bool dangKyNhanQuaBuuDien = false, // MỚI
        [CanBeNull] DateTime? ngayNop = null)
    {
        // 1. Validate basic inputs
        Check.NotNullOrWhiteSpace(maHoSo, nameof(maHoSo), BDocumentConsts.MaxMaHoSoLength);
        Check.NotNullOrWhiteSpace(tenChuHoSo, nameof(tenChuHoSo), BDocumentConsts.MaxTenChuHoSoLength);
        // ... validate other lengths ...

        // 2. Validate MaHoSo uniqueness
        await ValidateMaHoSoUniquenessAsync(maHoSo);

        // 3. Validate Procedure existence
        await ValidateProcedureExistsAsync(procedureId);

        // 4. Create the BDocument entity
        var bDocument = new BDocument(
            _guidGenerator.Create(),
            procedureId,
            maHoSo,
            tenChuHoSo,
            soDinhDanhChuHoSo,
            diaChiChuHoSo,
            emailChuHoSo,
            soDienThoaiChuHoSo,
            phamViHoatDong, // MỚI
            dangKyNhanQuaBuuDien, // MỚI
            ngayNop
        );

        // Initial status is null. BDocumentData added via AddOrUpdateComponentDataAsync in AppService.

        return bDocument;
    }

    /// <summary>
    /// Updates main information for an existing BDocument.
    /// </summary>
    public async Task<BDocument> UpdateInfoAsync( // Đổi tên
        [NotNull] BDocument bDocument,
        [NotNull] string tenChuHoSo,
        [CanBeNull] string? soDinhDanhChuHoSo,
        [CanBeNull] string? diaChiChuHoSo,
        [CanBeNull] string? emailChuHoSo,
        [CanBeNull] string? soDienThoaiChuHoSo,
        [CanBeNull] string? phamViHoatDong, // MỚI
        bool dangKyNhanQuaBuuDien) // MỚI
    {
        Check.NotNull(bDocument, nameof(bDocument));
        await ValidateUpdatableStatusAsync(bDocument); // Check if status allows update

        bDocument.UpdateInfo( // Gọi phương thức entity đã đổi tên
            tenChuHoSo,
            soDinhDanhChuHoSo,
            diaChiChuHoSo,
            emailChuHoSo,
            soDienThoaiChuHoSo,
            phamViHoatDong, // MỚI
            dangKyNhanQuaBuuDien // MỚI
        );
        return bDocument;
    }

    /// <summary>
    /// Adds or updates data for a specific component.
    /// </summary>
    public async Task<BDocument> AddOrUpdateComponentDataAsync(
         [NotNull] BDocument bDocument,
         Guid procedureComponentId,
         [CanBeNull] string? formData, // Can be JSON for Declaration Form
         [CanBeNull] Guid? fileId)
    {
        Check.NotNull(bDocument, nameof(bDocument));
        await ValidateUpdatableComponentDataStatusAsync(bDocument); // Check status allowance

        // 1. Validate Component exists and belongs to the Procedure
        var component = await ValidateProcedureComponentAsync(bDocument.ProcedureId, procedureComponentId);

        // 2. Validate data consistency (Form vs File) based on component type
        ValidateComponentDataType(component, formData, fileId);

        // 3. Validate FileId exists if provided
        if (fileId.HasValue)
        {
            await ValidateFileExistsAsync(fileId.Value);
        }

        // 4. Use the entity's method to add/update data
        bDocument.AddOrUpdateData(procedureComponentId, formData, fileId);

        return bDocument;
    }

    /// <summary>
    /// Removes the data associated with a specific component. Optionally deletes the file.
    /// </summary>
    public async Task<BDocument> RemoveComponentDataAsync(
        [NotNull] BDocument bDocument,
        Guid procedureComponentId,
        bool deleteAssociatedFile = false)
    {
        Check.NotNull(bDocument, nameof(bDocument));
        await ValidateUpdatableComponentDataStatusAsync(bDocument); // Check status allowance

        var dataToRemove = bDocument.DocumentData.FirstOrDefault(d => d.ProcedureComponentId == procedureComponentId);

        if (dataToRemove != null)
        {
            Guid? fileIdToDelete = (dataToRemove as BDocumentData)?.FileId; // Cast to get FileId
            bDocument.RemoveData(procedureComponentId); // Remove link from BDocument

            if (deleteAssociatedFile && fileIdToDelete.HasValue)
            {
                await DeleteFileAsync(fileIdToDelete.Value, bDocument.Id, procedureComponentId);
            }
        }
        return bDocument;
    }

    /// <summary>
    /// Changes the status of a BDocument.
    /// </summary>
    public async Task<BDocument> ChangeStatusAsync(
        [NotNull] BDocument bDocument,
        Guid newTrangThaiHoSoId, // ID mới không nullable
        [CanBeNull] string? reason = null)
    {
        Check.NotNull(bDocument, nameof(bDocument));

        Guid? currentStatusId = bDocument.TrangThaiHoSoId; // Lấy ID hiện tại (nullable)
        if (currentStatusId == newTrangThaiHoSoId) return bDocument; // Không thay đổi

        // 1. Validate new status exists
        var newStatus = await _workflowStatusRepository.FindAsync(newTrangThaiHoSoId)
             ?? throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.WorkflowStatusNotFoundForDocument]);

        // 2. Validate the transition logic
        await ValidateStatusTransitionAsync(bDocument, currentStatusId, newTrangThaiHoSoId);

        // 3. Determine dates to update based on the new status
        DateTime? receptionDate = null;
        DateTime? resultDate = null;
        // Example: Logic to set dates based on newStatus.Code or properties
        // if (newStatus.IsReceptionStatus()) receptionDate = Clock.Now; // Use IClock if needed
        // if (newStatus.IsFinalStatus()) resultDate = Clock.Now;

        // 4. Use the entity's internal method
        bDocument.SetTrangThaiHoSoId(newTrangThaiHoSoId, reason, receptionDate, resultDate);

        return bDocument;
    }

    // --- Public Helper Method for Validation (used by AppService) ---
    public void ValidateComponentDataType(ProcedureComponent component, string? formData, Guid? fileId)
    {
        if (component.Type == ComponentType.Form && fileId.HasValue)
        {
            throw new UserFriendlyException(_localizer["CannotAssociateFileWithFormComponent"]); // TODO: Add Error Code
        }
        if (component.Type == ComponentType.File && !string.IsNullOrEmpty(formData))
        {
            throw new UserFriendlyException(_localizer["CannotSaveFormDataForFileComponent"]); // TODO: Add Error Code
        }
    }


    // --- Private Validation Helpers ---

    private async Task ValidateProcedureExistsAsync(Guid procedureId)
    {
        // Sử dụng AnyAsync để kiểm tra hiệu quả hơn là GetAsync
        if (!await _procedureRepository.AnyAsync(p => p.Id == procedureId))
        {
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.ProcedureNotFoundForDocument]);
        }
    }

    private async Task<ProcedureComponent> ValidateProcedureComponentAsync(Guid procedureId, Guid componentId)
    {
        var component = await _procedureComponentRepository.FindAsync(componentId);
        if (component == null)
        {
            throw new UserFriendlyException(_localizer["ProcedureComponentNotFound"]); // TODO: Add Error Code
        }
        // Assume ComponentRepository has a method to check link
        var isLinked = await _procedureComponentRepository.IsLinkedToProcedureAsync(componentId, procedureId);
        if (!isLinked)
        {
            throw new UserFriendlyException(_localizer["ProcedureComponentDoesNotBelongToProcedure"]); // TODO: Add Error Code
        }
        return component;
    }

    /// <summary>
    /// Validates if a file exists in the repository.
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task ValidateFileExistsAsync(Guid fileId)
    {
        var file = await _fileRepository.FindAsync(fileId);
        if (file == null)
        {
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.FileManagementInteractionFailed, $"File with ID {fileId} not found."]);
        }
    }

    /// <summary>
    /// Deletes a file from the repository.
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="bDocumentId"></param>
    /// <param name="componentId"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task DeleteFileAsync(Guid fileId, Guid bDocumentId, Guid componentId)
    {
        try
        {
            var file = await _fileRepository.GetAsync(fileId);
            if (file != null)
            {
                await _fileManager.DeleteAsync(file); // Sử dụng overload nhận File entity
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete file {FileId} for BDocument {BDocumentId}, Component {ComponentId}.", fileId, bDocumentId, componentId);
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.FileManagementInteractionFailed, ex.Message]);
        }
    }

    private async Task ValidateMaHoSoUniquenessAsync([NotNull] string maHoSo, Guid? excludeId = null)
    {
        if (await _bDocumentRepository.MaHoSoExistsAsync(maHoSo, excludeId))
        {
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.BDocumentCodeAlreadyExists, maHoSo]);
        }
    }

    private async Task ValidateStatusTransitionAsync(BDocument bDocument, Guid? fromStatusId, Guid toStatusId)
    {
        // Fetch statuses if needed for rule checking
        WorkflowStatus? fromStatus = fromStatusId.HasValue ? await _workflowStatusRepository.FindAsync(fromStatusId.Value) : null;
        WorkflowStatus toStatus = await _workflowStatusRepository.GetAsync(toStatusId); // Assuming toStatus must exist

        // TODO: Implement actual workflow transition rules based on fromStatus and toStatus properties/codes.
        // Example: Cannot transition from a 'COMPLETED' status
        // var fromStatusName = fromStatus?.Name ?? "null";
        // if (fromStatus?.Code == "COMPLETED") {
        //     throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.InvalidDocumentStatusTransition, fromStatusName, toStatus.Name]);
        // }
        // Example: Initial transition must be to specific statuses
        // if (fromStatus == null && !new[] { "PENDING", "RECEIVED" }.Contains(toStatus.Code)) {
        //    throw new UserFriendlyException(...)
        // }

        await Task.CompletedTask; // Placeholder
    }

    private async Task ValidateUpdatableStatusAsync(BDocument bDocument)
    {
        if (!bDocument.TrangThaiHoSoId.HasValue) return; // Can update if no status yet? Or require initial status?
        var currentStatus = await _workflowStatusRepository.FindAsync(bDocument.TrangThaiHoSoId.Value);
        // TODO: Check if currentStatus allows updating document info
        // if (currentStatus != null && !currentStatus.AllowUpdate) { // Assuming AllowUpdate property
        //    throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.CannotUpdateDocumentInCurrentStatus, currentStatus.Name]);
        // }
        await Task.CompletedTask; // Placeholder
    }

    private async Task ValidateUpdatableComponentDataStatusAsync(BDocument bDocument)
    {
        if (!bDocument.TrangThaiHoSoId.HasValue) return;
        var currentStatus = await _workflowStatusRepository.FindAsync(bDocument.TrangThaiHoSoId.Value);
        // TODO: Check if currentStatus allows updating component data
        // if (currentStatus != null && !currentStatus.AllowComponentUpdate) { // Assuming AllowComponentUpdate property
        //    throw new UserFriendlyException(...)
        // }
        await Task.CompletedTask; // Placeholder
    }
}