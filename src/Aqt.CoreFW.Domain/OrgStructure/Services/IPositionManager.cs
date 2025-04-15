    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Domain.OrgStructure.Entities; // Reference to the Position entity
    using Volo.Abp.Domain.Services;

    namespace Aqt.CoreFW.Domain.OrgStructure.Services;

    /// <summary>
    /// Defines the domain service interface for managing Positions.
    /// Encapsulates business logic related to position creation, role assignment,
    /// primary position management, and deletion, ensuring aggregate consistency.
    /// </summary>
    public interface IPositionManager : IDomainService // Inherit from IDomainService for DI registration
    {
        /// <summary>
        /// Creates a new position for a user, assigning roles and handling primary status logic.
        /// Ensures that the user, OU, job title, and roles exist.
        /// Checks for duplicate positions (same user, OU, job title).
        /// Manages the single primary position per user rule.
        /// Triggers role synchronization for the user after creation.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="organizationUnitId">The ID of the organization unit.</param>
        /// <param name="jobTitleId">The ID of the job title.</param>
        /// <param name="roleIds">A list of Role IDs to assign to this position.</param>
        /// <param name="isPrimary">Whether this new position should be set as the primary one.</param>
        /// <returns>The newly created <see cref="Position"/> entity.</returns>
        /// <exception cref="Volo.Abp.BusinessException">Thrown if validation fails (e.g., entities not found, position already exists).</exception>
        Task<Position> CreateAsync(
            Guid userId,
            Guid organizationUnitId,
            Guid jobTitleId,
            List<Guid> roleIds,
            bool isPrimary);

        /// <summary>
        /// Updates the roles assigned to an existing position.
        /// Removes existing roles and assigns the new ones.
        /// Ensures the provided role IDs exist.
        /// Triggers role synchronization for the user after update.
        /// </summary>
        /// <param name="position">The position entity to update roles for.</param>
        /// <param name="newRoleIds">The new list of Role IDs to be assigned.</param>
        /// <exception cref="Volo.Abp.BusinessException">Thrown if role validation fails.</exception>
        Task UpdateRolesAsync(
            Position position, // Pass the entity to avoid fetching again if already loaded
            List<Guid> newRoleIds);

        // Consider adding an overload: Task UpdateRolesAsync(Guid positionId, List<Guid> newRoleIds);

        /// <summary>
        /// Sets a specific position as the primary position for the user.
        /// Automatically unsets the primary flag on any other position the user might have.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="positionId">The ID of the position to set as primary.</param>
        /// <exception cref="Volo.Abp.BusinessException">Thrown if the position does not belong to the user or other validation fails.</exception>
        Task SetPrimaryPositionAsync(
            Guid userId,
            Guid positionId);

        /// <summary>
        /// Deletes a position and its associated roles.
        /// Triggers role synchronization for the user.
        /// Handles logic if the deleted position was the primary one (e.g., assigning a new primary if possible).
        /// </summary>
        /// <param name="position">The position entity to delete.</param>
        Task DeleteAsync(Position position); // Pass the entity to avoid fetching again

        // Consider adding an overload: Task DeleteAsync(Guid positionId);

        // Potential future methods:
        // Task ChangeOrganizationUnitAsync(Position position, Guid newOrganizationUnitId);
        // Task ChangeJobTitleAsync(Position position, Guid newJobTitleId);
    }