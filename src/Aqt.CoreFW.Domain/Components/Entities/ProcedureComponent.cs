using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // For Collection initialization
using Aqt.CoreFW.Components; // Namespace chứa Consts và Enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.Components.Entities; // Namespace cụ thể cho Entity

/// <summary>
/// Represents a component of an administrative procedure (e.g., a form or a file template).
/// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
/// </summary>
public class ProcedureComponent : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the component.
    /// Cannot be changed after creation.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the component.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Status of the component (Active/Inactive).
    /// </summary>
    public virtual ComponentStatus Status { get; private set; }

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
    /// Type of the component (Form or File). Determines whether FormDefinition or TempPath is used.
    /// </summary>
    public virtual ComponentType Type { get; private set; }

    /// <summary>
    /// Definition for the form (e.g., JSON structure). Used when Type is Form.
    /// Stored as string, potentially large.
    /// </summary>
    [CanBeNull]
    public virtual string? FormDefinition { get; private set; }

    /// <summary>
    /// Path to the template file. Used when Type is File.
    /// </summary>
    [CanBeNull]
    public virtual string? TempPath { get; private set; }

    /// <summary>
    /// Navigation property for the many-to-many relationship with Procedures.
    /// Represents the links to Procedures this component is part of.
    /// </summary>
    public virtual ICollection<ProcedureComponentLink> ProcedureLinks { get; protected set; } // Sử dụng tên mới ProcedureComponentLink

    /// <summary>
    /// Protected constructor for ORM frameworks.
    /// </summary>
    protected ProcedureComponent()
    {
        /* For ORM */
        Code = string.Empty; // Initialize with valid default
        Name = string.Empty; // Initialize with valid default
        ProcedureLinks = new Collection<ProcedureComponentLink>(); // Khởi tạo collection với tên mới
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ProcedureComponent"/> class.
    /// Ensures required fields are provided and validates initial state.
    /// Use ProcedureComponentManager to create instances.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The unique code.</param> // Manager validates uniqueness
    /// <param name="name">The name.</param>
    /// <param name="order">The display order.</param>
    /// <param name="type">The component type (Form/File).</param>
    /// <param name="formDefinition">Form definition (required if type is Form).</param>
    /// <param name="tempPath">Template path (required if type is File).</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">The status.</param>
    internal ProcedureComponent( // Constructor internal để buộc tạo qua ProcedureComponentManager
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        int order,
        ComponentType type,
        [CanBeNull] string? formDefinition = null,
        [CanBeNull] string? tempPath = null,
        [CanBeNull] string? description = null,
        ComponentStatus status = ComponentStatus.Active)
        : base(id)
    {
        // Set Code directly, only once, validated by ProcedureComponentManager
        SetCodeInternal(code);

        // Set other basic properties via internal setters for validation
        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status; // Assign enum directly

        // Set Type and related content (FormDefinition or TempPath)
        // This method includes validation based on type.
        SetTypeAndContentInternal(type, formDefinition, tempPath);

        ProcedureLinks = new Collection<ProcedureComponentLink>(); // Khởi tạo collection với tên mới
    }

    // --- Internal setters with validation ---

    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), ComponentConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), ComponentConsts.MaxNameLength);
        Name = name;
    }

    private void SetOrderInternal(int order)
    {
        // Add validation for Order if needed (e.g., non-negative)
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), ComponentConsts.MaxDescriptionLength);
        Description = description;
    }

    private void SetFormDefinitionInternal([CanBeNull] string? formDefinition)
    {
        // Basic validation (e.g., not excessively long if there were a theoretical limit)
        // More complex validation (like JSON structure) might happen in Manager or AppService.
        FormDefinition = formDefinition;
    }

    private void SetTempPathInternal([CanBeNull] string? tempPath)
    {
        Check.Length(tempPath, nameof(tempPath), ComponentConsts.MaxTempPathLength);
        // Maybe validate path format?
        TempPath = tempPath;
    }

    /// <summary>
    /// Internal method to set the Type and ensure consistency of FormDefinition/TempPath.
    /// </summary>
    private void SetTypeAndContentInternal(ComponentType type, [CanBeNull] string? formDefinition, [CanBeNull] string? tempPath)
    {
        Type = type;
        SetFormDefinitionInternal(formDefinition);
        SetTempPathInternal(tempPath);
        //if (type == ComponentType.Form)
        //{
        //    // Validate FormDefinition is provided and TempPath is null/empty
        //    Check.NotNullOrWhiteSpace(formDefinition, nameof(formDefinition)); // Must have definition if type is Form
        //    if (!string.IsNullOrWhiteSpace(tempPath))
        //    {
        //        // Consider throwing a specific BusinessException with Error Code
        //        throw new ArgumentException("TempPath must be null or empty when component Type is Form.", nameof(tempPath));
        //    }
        //    SetFormDefinitionInternal(formDefinition);
        //    SetTempPathInternal(null); // Ensure TempPath is cleared
        //}
        //else // Type is File
        //{
        //    // Validate TempPath is provided and FormDefinition is null/empty
        //    Check.NotNullOrWhiteSpace(tempPath, nameof(tempPath)); // Must have path if type is File
        //    if (!string.IsNullOrWhiteSpace(formDefinition))
        //    {
        //        // Consider throwing a specific BusinessException with Error Code
        //        throw new ArgumentException("FormDefinition must be null or empty when component Type is File.", nameof(formDefinition));
        //    }
        //    SetTempPathInternal(tempPath);
        //    SetFormDefinitionInternal(null); // Ensure FormDefinition is cleared
        //}
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Changes the name of the component.
    /// </summary>
    public ProcedureComponent SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

    /// <summary>
    /// Changes the display/processing order.
    /// </summary>
    public ProcedureComponent SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    /// <summary>
    /// Changes the description.
    /// </summary>
    public ProcedureComponent SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    /// <summary>
    /// Changes the type and associated content (FormDefinition or TempPath).
    /// Ensures validation rules based on the new type are met.
    /// </summary>
    public ProcedureComponent SetTypeAndContent(ComponentType type, [CanBeNull] string? formDefinition, [CanBeNull] string? tempPath)
    {
        SetTypeAndContentInternal(type, formDefinition, tempPath);
        return this;
    }


    /// <summary>
    /// Sets the component status to Active.
    /// </summary>
    public ProcedureComponent Activate()
    {
        Status = ComponentStatus.Active;
        return this;
    }

    /// <summary>
    /// Sets the component status to Inactive.
    /// </summary>
    public ProcedureComponent Deactivate()
    {
        Status = ComponentStatus.Inactive;
        return this;
    }

    // Note: No public SetCode method.

    // --- Methods to manage the ProcedureLinks collection ---
    // Called by ProcedureComponentManager
    internal void ClearProcedureLinks()
    {
        ProcedureLinks.Clear();
    }

    internal void AddProcedureLink(ProcedureComponentLink link) // Sử dụng tên mới ProcedureComponentLink
    {
        // Basic check to prevent adding null or duplicate links (based on object reference initially)
        Check.NotNull(link, nameof(link));
        if (!ProcedureLinks.Contains(link)) // Simple duplicate check
        {
            ProcedureLinks.Add(link);
        }
        // More robust duplicate check based on composite key might be needed depending on usage pattern
    }
    internal void RemoveProcedureLink(ProcedureComponentLink link) // Sử dụng tên mới ProcedureComponentLink
    {
        Check.NotNull(link, nameof(link));
        ProcedureLinks.Remove(link);
    }
}