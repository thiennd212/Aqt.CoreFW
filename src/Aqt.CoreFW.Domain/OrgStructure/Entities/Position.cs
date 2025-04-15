    using System;
    using System.Collections.Generic; // Required if using ICollection for PositionRoles later
    using Volo.Abp.Domain.Entities.Auditing;
    // using Volo.Abp.Identity; // Uncomment if navigation property AbpIdentityUser is used
    // using Volo.Abp.Organizations; // Uncomment if navigation property OrganizationUnit is used

    namespace Aqt.CoreFW.Domain.OrgStructure.Entities;

    /// <summary>
    /// Represents a specific work assignment (position) for a user within an organization unit, holding a specific job title.
    /// </summary>
    public class Position : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Foreign key to the AbpIdentityUser.
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// Foreign key to the AbpOrganizationUnit.
        /// </summary>
        public virtual Guid OrganizationUnitId { get; protected set; }

        /// <summary>
        /// Foreign key to the JobTitle.
        /// </summary>
        public virtual Guid JobTitleId { get; protected set; }

        /// <summary>
        /// Indicates if this is the user's primary position.
        /// Should only be managed by the PositionManager domain service to ensure uniqueness per user.
        /// </summary>
        public virtual bool IsPrimary { get; internal set; } // internal set prevents direct modification outside domain logic

        // --- Optional Navigation properties ---
        // Uncomment if needed, but be mindful of loading strategies (lazy loading vs explicit includes)
        // public virtual AbpIdentityUser User { get; protected set; }
        // public virtual OrganizationUnit OrganizationUnit { get; protected set; }
        // public virtual JobTitle JobTitle { get; protected set; }

        // --- Optional Collection for PositionRoles (if PositionRole is part of the Position Aggregate) ---
        // If PositionRole is treated as a separate entity (recommended for many-to-many), this collection might not be needed here.
        // public virtual ICollection<PositionRole> PositionRoles { get; protected set; }

        /// <summary>
        /// Protected constructor for ORM.
        /// </summary>
        protected Position()
        {
            // If using the PositionRoles collection:
            // PositionRoles = new HashSet<PositionRole>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Position"/> class.
        /// The IsPrimary flag should be validated and managed by the PositionManager.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="organizationUnitId">The organization unit ID.</param>
        /// <param name="jobTitleId">The job title ID.</param>
        /// <param name="isPrimary">Initial primary status (default false). Domain service should enforce rules.</param>
        public Position(
            Guid id,
            Guid userId,
            Guid organizationUnitId,
            Guid jobTitleId,
            bool isPrimary = false) // Default to false, manager should handle logic
            : base(id)
        {
            // Basic assignment, no complex validation here. Domain service handles constraints.
            UserId = userId;
            OrganizationUnitId = organizationUnitId;
            JobTitleId = jobTitleId;
            IsPrimary = isPrimary; // Manager will verify/correct this

            // If using the PositionRoles collection:
            // PositionRoles = new HashSet<PositionRole>();
        }

        /// <summary>
        /// Internal method to set the primary status.
        /// Should only be called by PositionManager to enforce the single-primary-per-user rule.
        /// </summary>
        /// <param name="isPrimary">The new primary status.</param>
        internal void SetPrimary(bool isPrimary)
        {
            IsPrimary = isPrimary;
        }

        // Add/Remove/Clear methods for PositionRoles would go here if PositionRole is part of this aggregate.
        // Example:
        // public void AddRole(Guid roleId) { ... check existence ... PositionRoles.Add(new PositionRole(this.Id, roleId)); ... }
        // public void RemoveRole(Guid roleId) { ... find and remove from PositionRoles ... }
        // public void ClearRoles() { PositionRoles.Clear(); }
    }