using System;
using System.Collections.Generic; // For potential future use, e.g., validating children logic
using Aqt.CoreFW.DataGroups; // Namespace chứa Consts và Enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.DataGroups.Entities; // Namespace cụ thể cho Entity

/// <summary>
/// Represents a data group entity, which can be organized hierarchically.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class DataGroup : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the data group. Cannot be changed after creation.
    /// Enforced globally unique by Domain/Application Service.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the data group.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the data group (Active/Inactive).
    /// </summary>
    public virtual DataGroupStatus Status { get; private set; }

    /// <summary>
    /// Display or processing order within its level.
    /// </summary>
    public virtual int Order { get; private set; }

    /// <summary>
    /// Optional description.
    /// </summary>
    [CanBeNull]
    public virtual string? Description { get; private set; }

    /// <summary>
    /// Foreign key to the parent DataGroup (for hierarchical structure). Null for root groups.
    /// </summary>
    public virtual Guid? ParentId { get; private set; } // Chỉ thay đổi qua Domain Service

    /// <summary>
    /// Timestamp of the last synchronization.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? LastSyncDate { get; private set; }

    /// <summary>
    /// ID of the record in the external system during synchronization.
    /// </summary>
    [CanBeNull]
    public virtual Guid? SyncRecordId { get; private set; }

    /// <summary>
    /// Code of the record in the external system during synchronization.
    /// </summary>
    [CanBeNull]
    public virtual string? SyncRecordCode { get; private set; }

    // Navigation property (optional, configure for lazy-loading in EF Core mapping)
    // public virtual DataGroup Parent { get; private set; }

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected DataGroup()
    {
        /* For ORM */
        Code = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
        Name = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DataGroup"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// Use DataGroupManager to create instances.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The unique code.</param>
    /// <param name="name">The name.</param>
    /// <param name="parentId">Optional parent ID.</param> // Manager sẽ validate parentId
    /// <param name="order">The display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">The status.</param>
    /// <param name="lastSyncDate">Optional last synchronization date.</param>
    /// <param name="syncRecordId">Optional synchronization record ID.</param>
    /// <param name="syncRecordCode">Optional synchronization record code.</param>
    internal DataGroup( // Constructor internal để buộc tạo qua DataGroupManager
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        Guid? parentId, // Chấp nhận parentId, manager đã kiểm tra
        int order = 0,
        [CanBeNull] string? description = null,
        DataGroupStatus status = DataGroupStatus.Active,
        [CanBeNull] DateTime? lastSyncDate = null,
        [CanBeNull] Guid? syncRecordId = null,
        [CanBeNull] string? syncRecordCode = null)
        : base(id)
    {
        // Set Code trực tiếp, chỉ một lần và đã được validate bởi DataGroupManager
        SetCodeInternal(code);
        ParentId = parentId; // Gán ParentId đã được kiểm tra bởi Manager

        // Set các thuộc tính khác qua internal setters để validate
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status; // Gán trực tiếp enum
        SetSyncInfoInternal(lastSyncDate, syncRecordId, syncRecordCode);
    }

    // --- Internal setters with validation ---

    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), DataGroupConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DataGroupConsts.MaxNameLength);
        Name = name;
    }

     private void SetOrderInternal(int order)
    {
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), DataGroupConsts.MaxDescriptionLength);
        Description = description;
    }

    private void SetSyncInfoInternal([CanBeNull] DateTime? lastSyncDate, [CanBeNull] Guid? syncRecordId, [CanBeNull] string? syncRecordCode)
    {
        Check.Length(syncRecordCode, nameof(syncRecordCode), DataGroupConsts.MaxSyncRecordCodeLength);
        LastSyncDate = lastSyncDate;
        SyncRecordId = syncRecordId;
        SyncRecordCode = syncRecordCode;
    }

    /// <summary>
    /// Internal method to change the parent ID. Should only be called by DataGroupManager.
    /// </summary>
    internal void SetParentIdInternal(Guid? parentId)
    {
        // DataGroupManager is responsible for validation (no cycles, parent exists)
        ParentId = parentId;
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Changes the name of the data group.
    /// </summary>
    public DataGroup SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

     /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public DataGroup SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public DataGroup SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    /// <summary>
    /// Sets the data group status to Active.
    /// </summary>
    public DataGroup Activate()
    {
        Status = DataGroupStatus.Active;
        return this;
    }

    /// <summary>
    /// Sets the data group status to Inactive.
    /// </summary>
    public DataGroup Deactivate()
    {
        Status = DataGroupStatus.Inactive;
        return this;
    }

     /// <summary>
    /// Updates the synchronization information.
    /// </summary>
    public DataGroup SetSyncInfo([CanBeNull] DateTime? lastSyncDate, [CanBeNull] Guid? syncRecordId, [CanBeNull] string? syncRecordCode)
    {
         SetSyncInfoInternal(lastSyncDate, syncRecordId, syncRecordCode);
         return this;
    }

    // Lưu ý: Không có public SetCode method.
    // Lưu ý: Không có public SetParentId method, việc này phải qua DataGroupManager.
} 