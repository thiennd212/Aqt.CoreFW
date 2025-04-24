using System;
using Aqt.CoreFW.DataCores; // Namespace chứa Consts và Enum từ Domain.Shared
using Aqt.CoreFW.Domain.DataGroups.Entities; // Namespace chứa Entity DataGroup (Cần kiểm tra lại namespace chính xác)
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.DataCores.Entities; // Namespace cụ thể cho Entity

/// <summary>
/// Represents a core data catalog item.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class DataCore : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the data core item. Cannot be changed after creation.
    /// Uniqueness constraint might be global or per DataGroup (confirm requirement).
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the data core item.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the data core item (Active/Inactive).
    /// </summary>
    public virtual DataCoreStatus Status { get; private set; }

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
    protected DataCore()
    {
        /* For ORM */
        Code = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
        Name = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DataCore"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// Use DataCoreManager to create instances.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The unique code.</param>
    /// <param name="name">The name.</param>
    /// <param name="dataGroupId">The ID of the parent DataGroup.</param> // Manager sẽ validate dataGroupId
    /// <param name="order">The display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">The status.</param>
    internal DataCore( // Constructor internal để buộc tạo qua DataCoreManager
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        Guid dataGroupId, // Manager đã kiểm tra
        int order = 0,
        [CanBeNull] string? description = null,
        DataCoreStatus status = DataCoreStatus.Active)
        : base(id)
    {
        // Set Code trực tiếp, chỉ một lần và đã được validate bởi DataCoreManager
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
        Check.NotNullOrWhiteSpace(code, nameof(code), DataCoreConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DataCoreConsts.MaxNameLength);
        Name = name;
    }

     private void SetOrderInternal(int order)
    {
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), DataCoreConsts.MaxDescriptionLength);
        Description = description;
    }

    /// <summary>
    /// Internal method to change the DataGroup ID. Should only be called by DataCoreManager.
    /// </summary>
    internal void SetDataGroupIdInternal(Guid dataGroupId)
    {
        // DataCoreManager is responsible for validation (data group exists)
        DataGroupId = dataGroupId;
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Changes the name of the data core item.
    /// </summary>
    public DataCore SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

     /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public DataCore SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public DataCore SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    /// <summary>
    /// Sets the data core status to Active.
    /// </summary>
    public DataCore Activate()
    {
        Status = DataCoreStatus.Active;
        return this;
    }

    /// <summary>
    /// Sets the data core status to Inactive.
    /// </summary>
    public DataCore Deactivate()
    {
        Status = DataCoreStatus.Inactive;
        return this;
    }

    // Lưu ý: Không có public SetCode method.
    // Lưu ý: Việc thay đổi DataGroupId phải qua DataCoreManager.
} 