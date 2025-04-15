    using System;
    using System.Threading.Tasks;
    using Volo.Abp.Domain.Services;

    namespace Aqt.CoreFW.Domain.OrgStructure.Services;

    /// <summary>
    /// Defines the domain service interface responsible for synchronizing
    /// the roles assigned to a user in the core Identity module (AbpIdentityUserRole)
    /// based on the roles granted through all their assigned Positions.
    /// </summary>
    public interface IUserRoleSynchronizer : IDomainService // Inherit from IDomainService for DI registration
    {
        /// <summary>
        /// Synchronizes the AbpIdentityUserRoles for a given user based on all roles
        /// assigned to their current positions.
        /// This involves:
        /// 1. Getting all positions for the user.
        /// 2. Getting all distinct roles associated with these positions.
        /// 3. Getting the user's current roles from AbpIdentityUserRole.
        /// 4. Calculating roles to add and roles to remove.
        /// 5. Updating the user's roles using UserManager.
        /// This method should typically be called after a Position is created, deleted,
        /// or its assigned roles are modified.
        /// </summary>
        /// <param name="userId">The ID of the user whose roles need synchronization.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SynchronizeUserRolesAsync(Guid userId);
    }