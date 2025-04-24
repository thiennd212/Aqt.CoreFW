using System;
using Aqt.CoreFW.DataImportants; // Namespace chứa Consts và Enum từ Domain.Shared
using Aqt.CoreFW.Domain.DataGroups.Entities; // Namespace chứa Entity DataGroup
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.DataImportants.Entities; // Namespace cụ thể cho Entity

/// <summary>
/// Represents an important data catalog item.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class DataImportant : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the data important item within a specific DataGroup.
    /// Cannot be changed after creation.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the data important item.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the data important item (Active/Inactive).
    /// </summary>
    public virtual DataImportantStatus Status { get; private set; }

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
    /// Foreign key to the DataGroup this item belongs to. Required.
    /// </summary>
    public virtual Guid DataGroupId { get; private set; } // Thay đổi qua Domain Service

    // Navigation property (optional, configure for lazy-loading in EF Core mapping)
    // public virtual DataGroup DataGroup { get; private set; }

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected DataImportant()
    {
        /* For ORM */
        Code = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
        Name = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DataImportant"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// Use DataImportantManager to create instances.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The unique code (within the DataGroup).</param>
    /// <param name="name">The name.</param>
    /// <param name="dataGroupId">The ID of the parent DataGroup.</param> // Manager sẽ validate dataGroupId và tính duy nhất của Code
    /// <param name="order">The display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">The status.</param>
    internal DataImportant( // Constructor internal để buộc tạo qua DataImportantManager
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        Guid dataGroupId, // Manager đã kiểm tra DataGroup tồn tại và Code là duy nhất trong Group này
        int order = 0,
        [CanBeNull] string? description = null,
        DataImportantStatus status = DataImportantStatus.Active)
        : base(id)
    {
        // Set Code trực tiếp, chỉ một lần và đã được validate bởi DataImportantManager
        SetCodeInternal(code);
        DataGroupId = dataGroupId; // Gán DataGroupId đã được kiểm tra bởi Manager

        // Set các thuộc tính khác qua internal setters để validate
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status; // Gán trực tiếp enum
    }

    // --- Internal setters with validation ---

    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), DataImportantConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DataImportantConsts.MaxNameLength);
        Name = name;
    }

     private void SetOrderInternal(int order)
    {
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), DataImportantConsts.MaxDescriptionLength);
        Description = description;
    }

    /// <summary>
    /// Internal method to change the DataGroup ID. Should only be called by DataImportantManager.
    /// The manager is responsible for validating uniqueness of the code within the new group.
    /// </summary>
    internal void SetDataGroupIdInternal(Guid dataGroupId)
    {
        // DataImportantManager is responsible for validation (data group exists, code uniqueness in new group)
        DataGroupId = dataGroupId;
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Changes the name of the data important item.
    /// </summary>
    public DataImportant SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

     /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public DataImportant SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public DataImportant SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this; // Sửa lại lỗi cú pháp từ ví dụ gốc
    }

    /// <summary>
    /// Sets the data important status to Active.
    /// </summary>
    public DataImportant Activate()
    {
        Status = DataImportantStatus.Active;
        return this;
    }

    /// <summary>
    /// Sets the data important status to Inactive.
    /// </summary>
    public DataImportant Deactivate()
    {
        Status = DataImportantStatus.Inactive;
        return this;
    }

    // Lưu ý: Không có public SetCode method.
    // Lưu ý: Việc thay đổi DataGroupId phải qua DataImportantManager.
} 