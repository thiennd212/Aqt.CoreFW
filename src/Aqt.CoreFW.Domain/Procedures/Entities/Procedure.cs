using System;
using Aqt.CoreFW.Procedures; // Namespace chứa Consts và Enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.Procedures.Entities; // Namespace cụ thể cho Entity

/// <summary>
/// Represents an administrative procedure.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class Procedure : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the procedure.
    /// Cannot be changed after creation.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the procedure.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the procedure (Active/Inactive).
    /// </summary>
    public virtual ProcedureStatus Status { get; private set; }

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
    /// Date when this record was last synchronized with an external source.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? LastSyncedDate { get; private set; }

    /// <summary>
    /// ID of the corresponding record in the external synchronization source.
    /// </summary>
    [CanBeNull]
    public virtual Guid? SyncRecordId { get; private set; }

    /// <summary>
    /// Code or identifier of the corresponding record in the external synchronization source.
    /// </summary>
    [CanBeNull]
    public virtual string? SyncRecordCode { get; private set; }

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected Procedure()
    {
        /* For ORM */
        Code = string.Empty; // Initialize with valid default
        Name = string.Empty; // Initialize with valid default
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Procedure"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// Use ProcedureManager to create instances.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The unique code.</param> // Manager validates uniqueness
    /// <param name="name">The name.</param>
    /// <param name="order">The display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">The status.</param>
    internal Procedure( // Constructor internal để buộc tạo qua ProcedureManager
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        int order = 0,
        [CanBeNull] string? description = null,
        ProcedureStatus status = ProcedureStatus.Active)
        : base(id)
    {
        // Set Code directly, only once, validated by ProcedureManager
        SetCodeInternal(code);

        // Set other properties via internal setters for validation
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status; // Assign enum directly

        // Sync fields are initially null
        LastSyncedDate = null;
        SyncRecordId = null;
        SyncRecordCode = null;
    }

    // --- Internal setters with validation ---

    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), ProcedureConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), ProcedureConsts.MaxNameLength);
        Name = name;
    }

     private void SetOrderInternal(int order)
    {
        // Add validation for Order if needed (e.g., non-negative)
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), ProcedureConsts.MaxDescriptionLength);
        Description = description;
    }

    private void SetSyncRecordCodeInternal([CanBeNull] string? syncRecordCode)
    {
        Check.Length(syncRecordCode, nameof(syncRecordCode), ProcedureConsts.MaxSyncRecordCodeLength);
        SyncRecordCode = syncRecordCode;
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Changes the name of the procedure.
    /// </summary>
    public Procedure SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

     /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public Procedure SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public Procedure SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    /// <summary>
    /// Sets the procedure status to Active.
    /// </summary>
    public Procedure Activate()
    {
        Status = ProcedureStatus.Active;
        return this;
    }

    /// <summary>
    /// Sets the procedure status to Inactive.
    /// </summary>
    public Procedure Deactivate()
    {
        Status = ProcedureStatus.Inactive;
        return this;
    }

    /// <summary>
    /// Updates synchronization information.
    /// Typically called by a dedicated synchronization process.
    /// </summary>
    public Procedure UpdateSyncInfo(DateTime? lastSyncedDate, Guid? syncRecordId, [CanBeNull] string? syncRecordCode)
    {
        LastSyncedDate = lastSyncedDate;
        SyncRecordId = syncRecordId;
        SetSyncRecordCodeInternal(syncRecordCode); // Use internal setter for validation
        return this;
    }

    // Note: No public SetCode method.
} 