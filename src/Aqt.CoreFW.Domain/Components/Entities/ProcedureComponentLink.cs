using System;
using Aqt.CoreFW.Domain.Procedures.Entities; // Namespace for Procedure entity
using Volo.Abp.Domain.Entities; // Base class for entities

namespace Aqt.CoreFW.Domain.Components.Entities;

/// <summary>
/// Joining entity representing the many-to-many relationship link
/// between Procedure and ProcedureComponent.
/// This entity uses a composite primary key (ProcedureId, ProcedureComponentId).
/// </summary>
public class ProcedureComponentLink : Entity // Using composite key managed by EF Core mapping
{
    /// <summary>
    /// Foreign key to the Procedure. Part of the composite primary key.
    /// </summary>
    public virtual Guid ProcedureId { get; protected set; }

    /// <summary>
    /// Foreign key to the ProcedureComponent. Part of the composite primary key.
    /// </summary>
    public virtual Guid ProcedureComponentId { get; protected set; }

    // Optional: Navigation properties back to the aggregate roots
    // These should typically NOT be included here to avoid Aggregate boundary issues.
    // Load aggregates separately using their IDs if needed.
    // public virtual Procedure Procedure { get; set; }
    // public virtual ProcedureComponent ProcedureComponent { get; set; }

    /// <summary>
    /// Protected constructor for ORM.
    /// </summary>
    protected ProcedureComponentLink() { }

    /// <summary>
    /// Creates a new link between a Procedure and a ProcedureComponent.
    /// Should only be created via the ProcedureComponentManager.
    /// </summary>
    /// <param name="procedureId">ID of the Procedure.</param>
    /// <param name="procedureComponentId">ID of the ProcedureComponent.</param>
    internal ProcedureComponentLink(Guid procedureId, Guid procedureComponentId)
    {
        // Basic validation can be added if needed, e.g., check Guids are not empty.
        ProcedureId = procedureId;
        ProcedureComponentId = procedureComponentId;
    }

    /// <summary>
    /// Gets the composite key values.
    /// Used by ABP/EF Core to identify the entity.
    /// </summary>
    /// <returns>An array containing the key values.</returns>
    public override object[] GetKeys()
    {
        return new object[] { ProcedureId, ProcedureComponentId };
    }
} 