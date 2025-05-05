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
using Microsoft.Extensions.Localization; // Thêm IStringLocalizer
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Abp.Timing; // Clock
using Aqt.CoreFW.Localization; // Thêm Resource
using Microsoft.Extensions.Logging; // ILogger
using Microsoft.Extensions.Logging.Abstractions;
using Aqt.CoreFW.Components; // NullLogger

namespace Aqt.CoreFW.Domain.BDocuments;

/// <summary>
/// Domain service for managing BDocument entities and related business logic.
/// Domain service để quản lý các entity BDocument và logic nghiệp vụ liên quan.
/// </summary>
public class BDocumentManager : DomainService
{
    private readonly IBDocumentRepository _bDocumentRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IWorkflowStatusRepository _workflowStatusRepository;
    private readonly IProcedureComponentRepository _componentRepository;
    private readonly IFileManager _fileManager;
    private readonly IFileRepository _fileRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IClock _clock; // Inject IClock

    public BDocumentManager(
        IBDocumentRepository bDocumentRepository,
        IProcedureRepository procedureRepository,
        IWorkflowStatusRepository workflowStatusRepository,
        IProcedureComponentRepository componentRepository,
        IFileManager fileManager,
        IFileRepository fileRepository,
        IGuidGenerator guidGenerator,
        IUnitOfWorkManager unitOfWorkManager,
        IStringLocalizer<CoreFWResource> localizer,
        IClock clock) // Inject IClock
    {
        _bDocumentRepository = bDocumentRepository;
        _procedureRepository = procedureRepository;
        _workflowStatusRepository = workflowStatusRepository;
        _componentRepository = componentRepository;
        _fileManager = fileManager;
        _fileRepository = fileRepository;
        _guidGenerator = guidGenerator;
        _unitOfWorkManager = unitOfWorkManager;
        _localizer = localizer;
        _clock = clock; // Gán IClock
    }

    /// <summary>
    /// Creates a new BDocument entity.
    /// Tạo một entity BDocument mới.
    /// </summary>
    public async Task<BDocument> CreateAsync(
        Guid procedureId,
        [NotNull] string code,
        [NotNull] string applicantName,
        [CanBeNull] string? applicantIdentityNumber = null,
        [CanBeNull] string? applicantAddress = null,
        [CanBeNull] string? applicantEmail = null,
        [CanBeNull] string? applicantPhoneNumber = null,
        [CanBeNull] string? scopeOfActivity = null,
        bool receiveByPost = false,
        [CanBeNull] DateTime? submissionDate = null)
    {
        // 1. Validate basic inputs - Kiểm tra đầu vào cơ bản
        Check.NotNullOrWhiteSpace(code, nameof(code), BDocumentConsts.MaxCodeLength);
        Check.NotNullOrWhiteSpace(applicantName, nameof(applicantName), BDocumentConsts.MaxApplicantNameLength);
        Check.Length(applicantIdentityNumber, nameof(applicantIdentityNumber), BDocumentConsts.MaxApplicantIdentityNumberLength);
        Check.Length(applicantAddress, nameof(applicantAddress), BDocumentConsts.MaxApplicantAddressLength);
        Check.Length(applicantEmail, nameof(applicantEmail), BDocumentConsts.MaxApplicantEmailLength);
        Check.Length(applicantPhoneNumber, nameof(applicantPhoneNumber), BDocumentConsts.MaxApplicantPhoneNumberLength);
        // Không check length cho scopeOfActivity nếu là NCLOB

        // 2. Validate Code uniqueness
        await ValidateCodeUniqueness(code);

        // 3. Validate Procedure existence
        await ValidateProcedureExistsAsync(procedureId);

        // 4. Create the BDocument entity
        var bDocument = new BDocument(
            _guidGenerator.Create(),
            procedureId,
            code,
            applicantName,
            _clock, // Truyền IClock vào đúng vị trí
            applicantIdentityNumber,
            applicantAddress,
            applicantEmail,
            applicantPhoneNumber,
            scopeOfActivity,
            receiveByPost,
            submissionDate
        );

        // Initial status is null. BDocumentData added via AddOrUpdateComponentDataAsync in AppService.
        // Trạng thái ban đầu là null. BDocumentData được thêm thông qua AddOrUpdateComponentDataAsync trong AppService.

        return bDocument;
    }

    /// <summary>
    /// Updates main information for an existing BDocument.
    /// Cập nhật thông tin chính cho một BDocument hiện có.
    /// </summary>
    public async Task<BDocument> UpdateInfoAsync(
        [NotNull] BDocument bDocument,
        [NotNull] string applicantName,
        [CanBeNull] string? applicantIdentityNumber,
        [CanBeNull] string? applicantAddress,
        [CanBeNull] string? applicantEmail,
        [CanBeNull] string? applicantPhoneNumber,
        [CanBeNull] string? scopeOfActivity,
        bool receiveByPost)
    {
        Check.NotNull(bDocument, nameof(bDocument));
        await ValidateUpdatableStatusAsync(bDocument); // Check if status allows update - Kiểm tra trạng thái có cho phép cập nhật không

        bDocument.UpdateInfo(
            applicantName,
            applicantIdentityNumber,
            applicantAddress,
            applicantEmail,
            applicantPhoneNumber,
            scopeOfActivity,
            receiveByPost
        );
        return bDocument;
    }

    /// <summary>
    /// Adds or updates data for a specific component.
    /// Thêm hoặc cập nhật dữ liệu cho một thành phần cụ thể.
    /// </summary>
    public async Task<BDocument> AddOrUpdateComponentDataAsync(
         [NotNull] BDocument bDocument,
         Guid procedureComponentId,
         [CanBeNull] string? inputData,
         [CanBeNull] Guid? fileId)
    {
         Check.NotNull(bDocument, nameof(bDocument));
         await ValidateUpdatableComponentDataStatusAsync(bDocument); // Check status allowance - Kiểm tra trạng thái cho phép

         // 1. Validate Component exists and belongs to the Procedure
         // Kiểm tra Component tồn tại và thuộc về Procedure
         var component = await ValidateProcedureComponentAsync(bDocument.ProcedureId, procedureComponentId);

         // 2. Validate data consistency (Form vs File) based on component type
         // Kiểm tra tính nhất quán của dữ liệu (Form vs File) dựa trên loại component
         ValidateComponentDataType(component, inputData, fileId);

         // 3. Validate FileId exists if provided
         if (fileId.HasValue)
         {
             await ValidateFileExistsAsync(fileId.Value);
         }

         // 4. Use the entity's method to add/update data
         // Sử dụng phương thức của entity để thêm/cập nhật dữ liệu
         bDocument.AddOrUpdateData(procedureComponentId, inputData, fileId);

         return bDocument;
    }

     /// <summary>
    /// Removes the data associated with a specific component. Optionally deletes the file.
    /// Xóa dữ liệu liên kết với một thành phần cụ thể. Tùy chọn xóa tệp.
    /// </summary>
    public async Task<BDocument> RemoveComponentDataAsync(
        [NotNull] BDocument bDocument,
        Guid procedureComponentId,
        bool deleteAssociatedFile = false)
    {
        Check.NotNull(bDocument, nameof(bDocument));
        await ValidateUpdatableComponentDataStatusAsync(bDocument); // Check status allowance - Kiểm tra trạng thái cho phép

        var dataToRemove = bDocument.DocumentData.FirstOrDefault(d => d.ProcedureComponentId == procedureComponentId);

        if (dataToRemove != null)
        {
             Guid? fileIdToDelete = dataToRemove.FileId;
             bDocument.RemoveData(procedureComponentId); // Remove link from BDocument - Xóa liên kết khỏi BDocument

             if (deleteAssociatedFile && fileIdToDelete.HasValue)
             {
                 await DeleteFileAsync(fileIdToDelete.Value, bDocument.Id, procedureComponentId);
             }
        }
        return bDocument;
    }

    /// <summary>
    /// Changes the status of a BDocument.
    /// Thay đổi trạng thái của một BDocument.
    /// </summary>
    public async Task<BDocument> ChangeStatusAsync(
        [NotNull] BDocument bDocument,
        Guid newWorkflowStatusId,
        [CanBeNull] string? reason = null)
    {
        Check.NotNull(bDocument, nameof(bDocument));

        Guid? currentStatusId = bDocument.WorkflowStatusId;
        if (currentStatusId == newWorkflowStatusId) return bDocument; // No change needed - Không cần thay đổi

        // 1. Validate new status exists - Kiểm tra trạng thái mới tồn tại
        var newStatus = await _workflowStatusRepository.FindAsync(newWorkflowStatusId)
             ?? throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.WorkflowStatusNotFoundForDocument]);

        // 2. Validate the transition logic - Kiểm tra logic chuyển đổi trạng thái
        await ValidateStatusTransitionAsync(bDocument, currentStatusId, newWorkflowStatusId);

        // 3. Determine dates to update based on the new status
        // Xác định ngày cần cập nhật dựa trên trạng thái mới
        DateTime? receptionDate = null;
        DateTime? resultDate = null;
        // Example: Logic to set dates based on newStatus.Code or properties
        // Ví dụ: Logic để đặt ngày dựa trên mã hoặc thuộc tính của newStatus
        // if (newStatus.IsReceptionStatus()) receptionDate = Clock.Now;
        // if (newStatus.IsFinalStatus()) resultDate = Clock.Now;

        // 4. Use the entity's internal method - Sử dụng phương thức nội bộ của entity
        bDocument.SetWorkflowStatusId(newWorkflowStatusId, reason, receptionDate, resultDate);

        return bDocument;
    }

    // --- Public Helper Method for Validation (used by AppService) ---
    // Phương thức trợ giúp public cho Validation (được sử dụng bởi AppService)
    public void ValidateComponentDataType(ProcedureComponent component, string? inputData, Guid? fileId) {
         if (component.Type == ComponentType.Form && fileId.HasValue) {
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.CannotAssociateFileWithFormComponent]);
         }
         if (component.Type == ComponentType.File && !string.IsNullOrEmpty(inputData)) {
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.CannotSaveFormDataForFileComponent]);
         }
    }

    // --- Private Validation Helpers ---
    // Các phương thức trợ giúp validation private

    private async Task ValidateProcedureExistsAsync(Guid procedureId) {
         if (!await _procedureRepository.AnyAsync(p => p.Id == procedureId)) {
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.ProcedureNotFoundForDocument]);
         }
    }

    private async Task<ProcedureComponent> ValidateProcedureComponentAsync(Guid procedureId, Guid componentId) {
         var component = await _componentRepository.FindAsync(componentId);
         if (component == null) {
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.ProcedureComponentNotFound]);
         }
         // Assume ComponentRepository has a method to check link
         // Giả định ComponentRepository có phương thức kiểm tra liên kết
         var isLinked = await _componentRepository.IsLinkedToProcedureAsync(componentId, procedureId);
         if (!isLinked) {
              throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.ProcedureComponentNotLinked]);
         }
         return component;
    }

    private async Task ValidateFileExistsAsync(Guid fileId) {
         var file = await _fileRepository.FindAsync(fileId);
         if (file == null) {
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.FileManagementInteractionFailed, $"File with ID {fileId} not found."]);
         }
    }

     private async Task DeleteFileAsync(Guid fileId, Guid bDocumentId, Guid componentId) {
        try {
            var file = await _fileRepository.FindAsync(fileId);
            if (file != null) {
                await _fileManager.DeleteAsync(file);
            }
        } catch(Exception ex) {
            Logger.LogError(ex, $"Failed to delete file {fileId} for BDocument {bDocumentId}, Component {componentId}.");
             throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.FileManagementInteractionFailed, ex.Message]);
        }
     }

    private async Task ValidateCodeUniqueness([NotNull] string code, Guid? excludeId = null)
    {
         if (await _bDocumentRepository.CodeExistsAsync(code, excludeId))
        {
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.BDocumentCodeAlreadyExists, code]);
        }
    }

    private async Task ValidateStatusTransitionAsync(BDocument bDocument, Guid? fromStatusId, Guid toStatusId)
    {
        // Fetch statuses if needed for rule checking - Lấy trạng thái nếu cần để kiểm tra quy tắc
        WorkflowStatus? fromStatus = fromStatusId.HasValue ? await _workflowStatusRepository.FindAsync(fromStatusId.Value) : null;
        WorkflowStatus toStatus = await _workflowStatusRepository.GetAsync(toStatusId); // Assuming toStatus must exist - Giả định toStatus phải tồn tại

        // TODO: Implement actual workflow transition rules based on fromStatus and toStatus properties/codes.
        // Cần triển khai logic chuyển đổi quy trình công việc thực tế dựa trên các thuộc tính/mã của fromStatus và toStatus.
        // Example: Cannot transition from a 'COMPLETED' status
        // Ví dụ: Không thể chuyển từ trạng thái 'COMPLETED'
        // if (fromStatus?.Code == "COMPLETED") {
        //     throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.InvalidDocumentStatusTransition, fromStatus.Name, toStatus.Name]);
        // }
        // Example: Initial transition must be to specific statuses
        // Ví dụ: Chuyển đổi ban đầu phải sang các trạng thái cụ thể
        // if (fromStatus == null && !new[] { "PENDING", "RECEIVED" }.Contains(toStatus.Code)) {
        //    throw new UserFriendlyException(...)
        // }

        await Task.CompletedTask; // Placeholder
    }

     private async Task ValidateUpdatableStatusAsync([NotNull] BDocument bDocument) {
         if (!bDocument.WorkflowStatusId.HasValue) return; // Can update if no status yet? Or require initial status? - Có thể cập nhật nếu chưa có trạng thái? Hoặc yêu cầu trạng thái ban đầu?
         var currentStatus = await _workflowStatusRepository.FindAsync(bDocument.WorkflowStatusId.Value);
         // TODO: Check if currentStatus allows updating document info
         // Cần kiểm tra xem currentStatus có cho phép cập nhật thông tin hồ sơ không
         // Giả sử WorkflowStatus có thuộc tính IsEditingAllowed
         // Assuming WorkflowStatus has an IsEditingAllowed property
         // if (currentStatus != null && !currentStatus.IsEditingAllowed) { // Assuming AllowUpdate property
         //    throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.CannotUpdateDocumentInCurrentStatus, currentStatus.Name]);
         // }
         await Task.CompletedTask; // Placeholder
     }

      private async Task ValidateUpdatableComponentDataStatusAsync([NotNull] BDocument bDocument) {
         if (!bDocument.WorkflowStatusId.HasValue) return;
         var currentStatus = await _workflowStatusRepository.FindAsync(bDocument.WorkflowStatusId.Value);
         // TODO: Check if currentStatus allows updating component data
         // Cần kiểm tra xem currentStatus có cho phép cập nhật dữ liệu thành phần không
         // Giả sử WorkflowStatus có thuộc tính IsComponentDataEditingAllowed
         // Assuming WorkflowStatus has an IsComponentDataEditingAllowed property
         // if (currentStatus != null && !currentStatus.IsComponentDataEditingAllowed) { // Assuming AllowComponentUpdate property
         //    throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.CannotUpdateComponentDataInCurrentStatus, currentStatus.Name]); // Sử dụng mã lỗi mới
         // }
          await Task.CompletedTask; // Placeholder
     }
}