    using System;
    using Volo.Abp.Domain.Entities;

    namespace Aqt.CoreFW.Domain.OrgStructure.Entities;

    /// <summary>
    /// Represents the link between a Position and an AbpIdentityRole,
    /// indicating which roles are granted for a specific position.
    /// This entity uses a composite primary key (PositionId, RoleId).
    /// </summary>
    public class PositionRole : Entity // Inherits from Entity, not AggregateRoot, as it's a join entity
    {
        /// <summary>
        /// Part of the composite primary key. Foreign key to the Position.
        /// </summary>
        public virtual Guid PositionId { get; protected set; }

        /// <summary>
        /// Part of the composite primary key. Foreign key to the AbpIdentityRole.
        /// </summary>
        public virtual Guid RoleId { get; protected set; }

        /// <summary>
        /// Protected constructor for ORM/persistence frameworks.
        /// </summary>
        protected PositionRole() { }

        /// <summary>
        /// Creates a new instance of the <see cref="PositionRole"/> class.
        /// </summary>
        /// <param name="positionId">The ID of the position.</param>
        /// <param name="roleId">The ID of the role.</param>
        public PositionRole(Guid positionId, Guid roleId)
        {
            PositionId = positionId;
            RoleId = roleId;
        }

        /// <summary>
        /// Returns the composite primary key values.
        /// Required by ABP when using composite keys.
        /// </summary>
        /// <returns>An array containing the primary key values (PositionId, RoleId).</returns>
        public override object[] GetKeys()
        {
            return new object[] { PositionId, RoleId };
        }
    }