// src/Aqt.CoreFW.Domain/Provinces/Entities/Province.cs
using System;
using Aqt.CoreFW.Domain.Shared.Provinces; // Sử dụng constants và enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.Provinces.Entities;

/// <summary>
/// Represents a province or city entity.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class Province : FullAuditedAggregateRoot<Guid>
{
    // ... (Toàn bộ nội dung của lớp Province như trong kế hoạch) ...
    /// <summary>
    /// Unique code for the province. Cannot be changed after creation.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the province.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the province (Active/Inactive).
    /// </summary>
    public virtual ProvinceStatus Status { get; private set; }

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
    /// Foreign key to the Country entity. Cannot be changed after creation.
    /// </summary>
    public virtual Guid CountryId { get; private set; }
    // Navigation property for Country can be added later if needed,
    // but avoid loading it by default in repositories.
    // public virtual Country Country { get; private set; }

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
    protected Province()
    {
        /* For ORM */
        // Initialize non-nullable string properties to avoid warnings
        Code = string.Empty;
        Name = string.Empty;
        // CountryId will be set by ORM
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Province"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// </summary>
    public Province(
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        Guid countryId, // CountryId is required and set at creation
        int order = 0,
        [CanBeNull] string? description = null,
        ProvinceStatus status = ProvinceStatus.Active, // Default status is Active
        [CanBeNull] DateTime? lastSyncedTime = null,
        [CanBeNull] string? syncId = null,
        [CanBeNull] string? syncCode = null)
        : base(id)
    {
        // Set Code and CountryId directly, validated internally once.
        SetCodeInternal(code); // Validate code format and length
        CountryId = countryId; // Assume countryId is valid (checked in Application layer or Domain Service if needed)

        // Set other properties using internal setters for validation
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status; // Direct assignment for enum
        SetSyncInfoInternal(lastSyncedTime, syncId, syncCode); // Set initial sync info
    }

    // --- Internal setters with validation ---

    // Code is set only once in the constructor
    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), ProvinceConsts.MaxCodeLength);
        // Add other potential code format validations if needed
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), ProvinceConsts.MaxNameLength);
        Name = name;
    }

    private void SetOrderInternal(int order)
    {
        // Potential future validation: Check non-negative?
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), ProvinceConsts.MaxDescriptionLength);
        Description = description;
    }

    private void SetSyncInfoInternal([CanBeNull] DateTime? lastSyncedTime, [CanBeNull] string? syncId, [CanBeNull] string? syncCode)
    {
        Check.Length(syncId, nameof(syncId), ProvinceConsts.MaxSyncIdLength);
        Check.Length(syncCode, nameof(syncCode), ProvinceConsts.MaxSyncCodeLength);

        LastSyncedTime = lastSyncedTime;
        SyncId = syncId;
        SyncCode = syncCode;
    }


    // --- Public methods to change state ---

    /// <summary>
    /// Changes the name of the province.
    /// </summary>
    public Province SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

    /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public Province SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public Province SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    /// <summary>
    /// Sets the province status to Active.
    /// </summary>
    public Province Activate()
    {
        Status = ProvinceStatus.Active;
        return this;
    }

    /// <summary>
    /// Sets the province status to Inactive.
    /// </summary>
    public Province Deactivate()
    {
        Status = ProvinceStatus.Inactive;
        return this;
    }

    /// <summary>
    /// Updates the synchronization information.
    /// </summary>
    public Province SetSyncInfo([CanBeNull] DateTime? lastSyncedTime, [CanBeNull] string? syncId, [CanBeNull] string? syncCode)
    {
        SetSyncInfoInternal(lastSyncedTime, syncId, syncCode);
        return this;
    }
}