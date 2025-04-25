using System;
using Aqt.CoreFW.AttachedDocuments; // Namespace chứa Consts và Enum từ Domain.Shared
using Aqt.CoreFW.Domain.Procedures.Entities; // Namespace chứa Entity Procedure (!! Giả định, cần xác minh !!)
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.AttachedDocuments.Entities; // Namespace cụ thể cho Entity

/// <summary>
/// Represents an attached document item related to an administrative procedure.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// Consider changing to FullAuditedEntity if it's not a true Aggregate Root.
/// </summary>
public class AttachedDocument : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the attached document within a specific Procedure.
    /// Cannot be changed after creation.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the attached document.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the attached document (Active/Inactive).
    /// </summary>
    public virtual AttachedDocumentStatus Status { get; private set; }

    /// <summary>
    /// Display or processing order.
    /// </summary>
    public virtual int Order { get; private set; }

    /// <summary>
    /// Optional description.
    /// </summary>
    [CanBeNull]
    public virtual string? Description { get; private set; }

    /// <summary>
    /// Foreign key to the Procedure this document belongs to. Required.
    /// </summary>
    public virtual Guid ProcedureId { get; private set; } // Thay đổi qua Domain Service

    // Navigation property (optional, configure for lazy-loading in EF Core mapping)
    // public virtual Procedure Procedure { get; private set; }

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected AttachedDocument()
    {
        /* For ORM */
        Code = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
        Name = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
    }

    /// <summary>
    /// Creates a new instance of the <see cref="AttachedDocument"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// Use AttachedDocumentManager to create instances.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The unique code (within the Procedure).</param>
    /// <param name="name">The name.</param>
    /// <param name="procedureId">The ID of the parent Procedure.</param> // Manager sẽ validate procedureId và tính duy nhất của Code
    /// <param name="order">The display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">The status.</param>
    internal AttachedDocument( // Constructor internal để buộc tạo qua AttachedDocumentManager
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        Guid procedureId, // Manager đã kiểm tra Procedure tồn tại và Code là duy nhất trong Procedure này
        int order = 0,
        [CanBeNull] string? description = null,
        AttachedDocumentStatus status = AttachedDocumentStatus.Active)
        : base(id)
    {
        // Set Code trực tiếp, chỉ một lần và đã được validate bởi AttachedDocumentManager
        SetCodeInternal(code);
        ProcedureId = procedureId; // Gán ProcedureId đã được kiểm tra bởi Manager

        // Set các thuộc tính khác qua internal setters để validate
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status; // Gán trực tiếp enum
    }

    // --- Internal setters with validation ---

    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), AttachedDocumentConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), AttachedDocumentConsts.MaxNameLength);
        Name = name;
    }

     private void SetOrderInternal(int order)
    {
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), AttachedDocumentConsts.MaxDescriptionLength);
        Description = description;
    }

    /// <summary>
    /// Internal method to change the Procedure ID. Should only be called by AttachedDocumentManager.
    /// The manager is responsible for validating uniqueness of the code within the new procedure.
    /// </summary>
    internal void SetProcedureIdInternal(Guid procedureId)
    {
        // AttachedDocumentManager is responsible for validation (procedure exists, code uniqueness in new procedure)
        ProcedureId = procedureId;
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Changes the name of the attached document.
    /// </summary>
    public AttachedDocument SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

     /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public AttachedDocument SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public AttachedDocument SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    /// <summary>
    /// Sets the attached document status to Active.
    /// </summary>
    public AttachedDocument Activate()
    {
        Status = AttachedDocumentStatus.Active;
        return this;
    }

    /// <summary>
    /// Sets the attached document status to Inactive.
    /// </summary>
    public AttachedDocument Deactivate()
    {
        Status = AttachedDocumentStatus.Inactive;
        return this;
    }

    // Lưu ý: Không có public SetCode method.
    // Lưu ý: Việc thay đổi ProcedureId phải qua AttachedDocumentManager.
} 