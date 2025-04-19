using System;
using Aqt.CoreFW.Domain.Shared.WorkflowStatuses; // Sử dụng constants
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.WorkflowStatuses.Entities;

/// <summary>
/// Represents a status within a workflow process.
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class WorkflowStatus : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the status.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Display name of the status.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Optional description for the status.
    /// </summary>
    [CanBeNull]
    public virtual string? Description { get; private set; }

    /// <summary>
    /// Order for displaying or processing the status.
    /// </summary>
    public virtual int Order { get; private set; }

    /// <summary>
    /// Optional color code (e.g., #RRGGBB) for UI representation.
    /// </summary>
    [CanBeNull]
    public virtual string? ColorCode { get; private set; }

    /// <summary>
    /// Indicates if the status is currently active and usable.
    /// </summary>
    public virtual bool IsActive { get; private set; }

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected WorkflowStatus()
    {
        /* For ORM */
        // Initialize non-nullable string properties to avoid warnings
        Code = string.Empty;
        Name = string.Empty;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WorkflowStatus"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// </summary>
    /// <param name="id">The unique identifier for the entity.</param>
    /// <param name="code">The mandatory status code.</param>
    /// <param name="name">The mandatory status name.</param>
    /// <param name="order">The display/processing order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="colorCode">Optional color code.</param>
    /// <param name="isActive">Initial active state (defaults to true).</param>
    public WorkflowStatus(
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        int order,
        [CanBeNull] string? description = null,
        [CanBeNull] string? colorCode = null,
        bool isActive = true) : base(id)
    {
        // Set properties using internal setters for validation
        SetCodeInternal(code);
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        SetColorCodeInternal(colorCode);
        IsActive = isActive; // Direct assignment is fine for boolean
    }

    // Internal setters with validation, called by constructor and public methods
    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), WorkflowStatusConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), WorkflowStatusConsts.MaxNameLength);
        Name = name;
    }

    private void SetOrderInternal(int order)
    {
        // Potential future validation: Check.Range(order, nameof(order), 0, int.MaxValue);
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), WorkflowStatusConsts.MaxDescriptionLength);
        Description = description;
    }

    private void SetColorCodeInternal([CanBeNull] string? colorCode)
    {
        Check.Length(colorCode, nameof(colorCode), WorkflowStatusConsts.MaxColorCodeLength);
        // Potential future validation: Regex check for #RRGGBB format
        ColorCode = colorCode;
    }


    // Public methods to change state, ensuring validation logic is applied
    // These methods now call the internal setters.

    public WorkflowStatus SetCode([NotNull] string code)
    {
        SetCodeInternal(code);
        return this;
    }

    public WorkflowStatus SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

    public WorkflowStatus SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    public WorkflowStatus SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    public WorkflowStatus SetColorCode([CanBeNull] string? colorCode)
    {
        SetColorCodeInternal(colorCode);
        return this;
    }

    public WorkflowStatus Activate()
    {
        IsActive = true;
        return this;
    }

    public WorkflowStatus Deactivate()
    {
        IsActive = false;
        return this;
    }
}