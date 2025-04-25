using System;
using System.Threading.Tasks;
using Aqt.CoreFW.AttachedDocuments; // Namespaces cần thiết
using Aqt.CoreFW.Domain.AttachedDocuments.Entities;
using Aqt.CoreFW.Domain.Procedures; // (!! Giả định, cần namespace chứa IProcedureRepository !!)
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Users; // Để inject ICurrentUser nếu cần kiểm tra quyền phức tạp hơn
using Volo.Abp.Localization;
using Aqt.CoreFW.Localization; // Namespace của Resource
using Microsoft.Extensions.Localization;

namespace Aqt.CoreFW.Domain.AttachedDocuments;

/// <summary>
/// Domain service responsible for managing AttachedDocument entities.
/// </summary>
public class AttachedDocumentManager : DomainService
{
    private readonly IAttachedDocumentRepository _attachedDocumentRepository;
    private readonly IProcedureRepository _procedureRepository; // (!! Giả định tên Repository, cần xác minh !!)
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> L; // Inject localizer
    // private readonly ICurrentUser _currentUser; // Inject nếu cần

    public AttachedDocumentManager(
        IAttachedDocumentRepository attachedDocumentRepository,
        IProcedureRepository procedureRepository, // (!! Cần xác minh tên Repository và phương thức AnyAsync bên dưới!!)
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> localizer/*,
        ICurrentUser currentUser*/)
    {
        _attachedDocumentRepository = attachedDocumentRepository;
        _procedureRepository = procedureRepository;
        _guidGenerator = guidGenerator;
        L = localizer; // Gán localizer
        // _currentUser = currentUser;
    }

    /// <summary>
    /// Creates a new AttachedDocument entity after validating business rules.
    /// </summary>
    /// <param name="code">Unique code within the Procedure.</param>
    /// <param name="name">Name.</param>
    /// <param name="procedureId">Parent Procedure ID.</param>
    /// <param name="order">Display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">Status.</param>
    /// <returns>The newly created AttachedDocument entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public async Task<AttachedDocument> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        Guid procedureId,
        int order = 0,
        [CanBeNull] string? description = null,
        AttachedDocumentStatus status = AttachedDocumentStatus.Active)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), AttachedDocumentConsts.MaxCodeLength);
        Check.NotNullOrWhiteSpace(name, nameof(name), AttachedDocumentConsts.MaxNameLength);
        Check.Length(description, nameof(description), AttachedDocumentConsts.MaxDescriptionLength);

        // 1. Validate Procedure existence
        await ValidateProcedureExistsAsync(procedureId);

        // 2. Validate Code uniqueness within the specified Procedure
        await ValidateCodeUniquenessAsync(code, procedureId);

        var attachedDocument = new AttachedDocument(
            _guidGenerator.Create(),
            code,
            name,
            procedureId,
            order,
            description,
            status
        );

        // Application Service sẽ gọi Repository.InsertAsync
        return attachedDocument;
    }

    /// <summary>
    /// Updates an existing AttachedDocument entity after validating business rules.
    /// </summary>
    /// <param name="attachedDocument">The entity to update.</param>
    /// <param name="name">New name.</param>
    /// <param name="procedureId">New parent Procedure ID.</param>
    /// <param name="order">New display order.</param>
    /// <param name="description">New optional description.</param>
    /// <param name="status">New status.</param>
    /// <returns>The updated AttachedDocument entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public async Task<AttachedDocument> UpdateAsync(
        [NotNull] AttachedDocument attachedDocument,
        [NotNull] string name,
        Guid procedureId, // Cho phép thay đổi Procedure
        int order,
        [CanBeNull] string? description,
        AttachedDocumentStatus status)
    {
        Check.NotNull(attachedDocument, nameof(attachedDocument));
        Check.NotNullOrWhiteSpace(name, nameof(name), AttachedDocumentConsts.MaxNameLength);
        Check.Length(description, nameof(description), AttachedDocumentConsts.MaxDescriptionLength);

        // 1. Validate Procedure existence if it's changed AND check Code uniqueness in the NEW procedure
        if (attachedDocument.ProcedureId != procedureId)
        {
            await ValidateProcedureExistsAsync(procedureId);
            // Kiểm tra Code có bị trùng trong Procedure mới không (bỏ qua chính nó vì Code không đổi)
            await ValidateCodeUniquenessAsync(attachedDocument.Code, procedureId, attachedDocument.Id);
            attachedDocument.SetProcedureIdInternal(procedureId); // Change Procedure ID via internal setter
        }

        // Update other properties
        attachedDocument.SetName(name);
        attachedDocument.SetOrder(order);
        attachedDocument.SetDescription(description);

        if (status == AttachedDocumentStatus.Active) attachedDocument.Activate();
        else attachedDocument.Deactivate();

        // Application Service sẽ gọi Repository.UpdateAsync
        return attachedDocument;
    }

    /// <summary>
    /// Changes the Procedure for an existing AttachedDocument.
    /// Ensures the code is unique in the new Procedure.
    /// </summary>
    public async Task ChangeProcedureAsync([NotNull] AttachedDocument attachedDocument, Guid newProcedureId)
    {
         Check.NotNull(attachedDocument, nameof(attachedDocument));

         if (attachedDocument.ProcedureId == newProcedureId)
         {
             return; // No change needed
         }

         await ValidateProcedureExistsAsync(newProcedureId);
         await ValidateCodeUniquenessAsync(attachedDocument.Code, newProcedureId, attachedDocument.Id); // Check uniqueness in new procedure

         attachedDocument.SetProcedureIdInternal(newProcedureId);
         // Repository.UpdateAsync will be called by the Application Service
    }


    /// <summary>
    /// Helper method to validate if a Procedure exists.
    /// </summary>
    private async Task ValidateProcedureExistsAsync(Guid procedureId)
    {
        // (!! Cần phương thức tương ứng trong IProcedureRepository, ví dụ AnyAsync hoặc ExistsAsync !!) 
        // !! Cần xác minh lại logic này khi có IProcedureRepository
        // Tạm thời dùng FindAsync để kiểm tra tồn tại thay vì AnyAsync
        var procedure = await _procedureRepository.FindAsync(procedureId);
        if (procedure == null)
        {
             throw new UserFriendlyException(L["ProcedureNotFoundForAttachedDocument", procedureId]);
            // Hoặc dùng mã lỗi: throw new BusinessException(CoreFWDomainErrorCodes.ProcedureNotFoundForAttachedDocument).WithData("id", procedureId);
        }
    }

    /// <summary>
    /// Helper method to validate code uniqueness within a Procedure.
    /// </summary>
    private async Task ValidateCodeUniquenessAsync([NotNull] string code, Guid procedureId, Guid? excludeId = null)
    {
         if (await _attachedDocumentRepository.CodeExistsAsync(code, procedureId, excludeId))
        {
            throw new UserFriendlyException(L["AttachedDocumentCodeAlreadyExists", code]);
            // Hoặc dùng mã lỗi: throw new BusinessException(CoreFWDomainErrorCodes.AttachedDocumentCodeAlreadyExists).WithData("code", code);
        }
    }
} 