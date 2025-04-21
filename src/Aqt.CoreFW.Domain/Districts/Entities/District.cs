using System;
using Aqt.CoreFW.Domain.Shared.Districts; // Constants and enum from Domain.Shared
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.Districts.Entities;

/// <summary>
/// Represents a district entity within a province.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class District : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the district. Cannot be changed after creation.
    /// Note: Uniqueness scope (global or per province) to be enforced by Domain/Application Service.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the district.
    /// Note: Uniqueness scope (global or per province) to be enforced by Domain/Application Service.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the district (Active/Inactive).
    /// </summary>
    public virtual DistrictStatus Status { get; private set; }

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
    /// Foreign key to the Province entity. Cannot be changed after creation.
    /// </summary>
    public virtual Guid ProvinceId { get; private set; }
    // Navigation property for Province can be added later if needed,
    // but avoid loading it by default in repositories.
    // public virtual Province Province { get; private set; }

    /// <summary>
    /// Timestamp of the last synchronization.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? LastSyncedTime { get; private set; }

    /// <summary>
    /// ID of the record in the external system during synchronization.
    /// </summary>
    [CanBeNull]
    public virtual string? SyncId { get; private set; }

    /// <summary>
    /// Code of the record in the external system during synchronization.
    /// </summary>
    [CanBeNull]
    public virtual string? SyncCode { get; private set; }

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected District()
    {
        /* For ORM */
        Code = string.Empty;
        Name = string.Empty;
        // ProvinceId will be set by ORM
    }

    /// <summary>
    /// Creates a new instance of the <see cref="District"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// Constructor marked internal to enforce creation via Application Service/Domain Service if needed,
    /// but public according to the latest plan review where DistrictManager was removed.
    /// </summary>
    public District( // Marked public as per plan (no DistrictManager)
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        Guid provinceId, // ProvinceId is required and set at creation
        int order = 0,
        [CanBeNull] string? description = null,
        DistrictStatus status = DistrictStatus.Active, // Default status is Active
        [CanBeNull] DateTime? lastSyncedTime = null,
        [CanBeNull] string? syncId = null,
        [CanBeNull] string? syncCode = null)
        : base(id)
    {
        // Set Code and ProvinceId directly, validated internally once.
        SetCodeInternal(code); // Validate code format and length
        ProvinceId = provinceId; // Assume provinceId is valid (checked in Application layer)

        // Set other properties using internal setters for validation
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status; // Direct assignment for enum
        SetSyncInfoInternal(lastSyncedTime, syncId, syncCode); // Set initial sync info
    }

    // --- Internal setters with validation ---
    // These methods centralize validation logic for properties.

    // Code is set only once in the constructor
    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), DistrictConsts.MaxCodeLength);
        // Add other potential code format validations if needed
        Code = code;
    }

    // Renamed to SetNameInternal to match pattern and called by public SetName
    internal virtual void SetNameInternal([NotNull] string name) // Changed to internal to be callable by AppService if needed
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.MaxNameLength);
        Name = name;
    }

    // Renamed to SetOrderInternal
    internal virtual void SetOrderInternal(int order) // Changed to internal
    {
        // Potential future validation: Check non-negative?
        Order = order;
    }

    // Renamed to SetDescriptionInternal
    internal virtual void SetDescriptionInternal([CanBeNull] string? description) // Changed to internal
    {
        Check.Length(description, nameof(description), DistrictConsts.MaxDescriptionLength);
        Description = description;
    }

    // Renamed to SetSyncInfoInternal
    internal virtual void SetSyncInfoInternal([CanBeNull] DateTime? lastSyncedTime, [CanBeNull] string? syncId, [CanBeNull] string? syncCode) // Changed to internal
    {
        Check.Length(syncId, nameof(syncId), DistrictConsts.MaxSyncIdLength);
        Check.Length(syncCode, nameof(syncCode), DistrictConsts.MaxSyncCodeLength);

        LastSyncedTime = lastSyncedTime;
        SyncId = syncId;
        SyncCode = syncCode;
    }

    // Internal setter for status, called by Activate/Deactivate or AppService
    internal virtual void SetStatus(DistrictStatus status) // Changed to internal
    {
        Status = status;
    }


    // --- Public methods to change state ---
    // These methods provide controlled ways to modify the entity's state.

    /// <summary>
    /// Changes the name of the district.
    /// </summary>
    public virtual District SetName([NotNull] string name) // Kept public as per plan
    {
        SetNameInternal(name);
        return this;
    }

    /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public virtual District SetOrder(int order) // Kept public as per plan
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public virtual District SetDescription([CanBeNull] string? description) // Kept public as per plan
    {
        SetDescriptionInternal(description);
        return this;
    }

    /// <summary>
    /// Sets the district status to Active.
    /// </summary>
    public virtual District Activate() // Kept public as per plan
    {
        SetStatus(DistrictStatus.Active); // Calls internal setter
        return this;
    }

    /// <summary>
    /// Sets the district status to Inactive.
    /// </summary>
    public virtual District Deactivate() // Kept public as per plan
    {
        SetStatus(DistrictStatus.Inactive); // Calls internal setter
        return this;
    }

    /// <summary>
    /// Updates the synchronization information.
    /// </summary>
    public virtual District SetSyncInfo([CanBeNull] DateTime? lastSyncedTime, [CanBeNull] string? syncId, [CanBeNull] string? syncCode) // Kept public as per plan
    {
        SetSyncInfoInternal(lastSyncedTime, syncId, syncCode);
        return this;
    }

    // Note: We don't add a SetProvinceId method because ProvinceId should be immutable after creation.
}