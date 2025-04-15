    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Domain.OrgStructure.Entities;
    using Aqt.CoreFW.Domain.OrgStructure.Repositories; // Updated path for repositories
    using Aqt.CoreFW.Domain.OrgStructure.Services;   // Updated path for services
    using Aqt.CoreFW.Domain.Shared.OrgStructure;     // For Error Codes, Consts
    using JetBrains.Annotations;                      // For NotNull, CanBeNull
    using Microsoft.AspNetCore.Identity;              // Required for IdentityResult.CheckErrors()
    using Volo.Abp;
    using Volo.Abp.Domain.Repositories;
    using Volo.Abp.Domain.Services;
    using Volo.Abp.Identity;                          // Required for IdentityUserManager, IdentityRoleManager
    // using Volo.Abp.ObjectMapping;                // Required for IOrganizationUnitRepository
    using Volo.Abp.Uow;

    namespace Aqt.CoreFW.Domain.OrgStructure;

    /// <summary>
    /// Implementation of the <see cref="IPositionManager"/> domain service.
    /// </summary>
    public class PositionManager : DomainService, IPositionManager
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IPositionRoleRepository _positionRoleRepository;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IOrganizationUnitRepository _organizationUnitRepository; // Use ABP's built-in repo
        private readonly IdentityUserManager _userManager;
        private readonly IdentityRoleManager _roleManager;
        private readonly IUserRoleSynchronizer _userRoleSynchronizer;

        public PositionManager(
            IPositionRepository positionRepository,
            IPositionRoleRepository positionRoleRepository,
            IJobTitleRepository jobTitleRepository,
            IOrganizationUnitRepository organizationUnitRepository, // Inject ABP's OU repo
            IdentityUserManager userManager,
            IdentityRoleManager roleManager,
            IUserRoleSynchronizer userRoleSynchronizer)
        {
            _positionRepository = positionRepository;
            _positionRoleRepository = positionRoleRepository;
            _jobTitleRepository = jobTitleRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRoleSynchronizer = userRoleSynchronizer;
        }

        [UnitOfWork] // Ensures operations are transactional
        public virtual async Task<Position> CreateAsync(
            Guid userId,
            Guid organizationUnitId,
            Guid jobTitleId,
            [NotNull] List<Guid> roleIds, // Ensure list is not null
            bool isPrimary)
        {
            // --- 1. Validate Input Entities ---
            await CheckUserExistsAsync(userId);
            await CheckOrganizationUnitExistsAsync(organizationUnitId);
            await CheckJobTitleExistsAsync(jobTitleId);
            await CheckRolesExistAsync(roleIds);

            // --- 2. Check for Duplicate Position ---
            var existingPosition = await _positionRepository.FindByUserOUJobTitleAsync(userId, organizationUnitId, jobTitleId);
            if (existingPosition != null)
            {
                // Use Domain Error Code defined in Domain.Shared
                throw new BusinessException(CoreFWDomainErrorCodes.PositionAlreadyExists)
                    .WithData("UserId", userId)
                    .WithData("OrganizationUnitId", organizationUnitId)
                    .WithData("JobTitleId", jobTitleId);
            }

            // --- 3. Handle Primary Position Logic ---
            if (isPrimary)
            {
                // Unset primary flag on the existing primary position, if any
                var currentPrimary = await _positionRepository.FindPrimaryPositionByUserIdAsync(userId);
                if (currentPrimary != null)
                {
                    currentPrimary.SetPrimary(false);
                    await _positionRepository.UpdateAsync(currentPrimary, autoSave: true); // Auto-save needed within UoW if not last operation
                }
            }
            else
            {
                // Optional: If this is the user's very first position, automatically make it primary?
                // var userPositionCount = await _positionRepository.GetCountByUserIdAsync(userId);
                // if (userPositionCount == 0) {
                //     isPrimary = true;
                // }
            }

            // --- 4. Create the new Position ---
            // The constructor sets the initial `isPrimary` value.
            // The internal `SetPrimary` method is mainly for updates via `SetPrimaryPositionAsync`.
            var newPosition = new Position(GuidGenerator.Create(), userId, organizationUnitId, jobTitleId, isPrimary);

            await _positionRepository.InsertAsync(newPosition, autoSave: true); // Insert first to get the ID

            // --- 5. Assign Roles ---
            if (roleIds.Any())
            {
                var uniqueRoleIds = roleIds.Distinct().ToList();
                var positionRoles = uniqueRoleIds.Select(roleId => new PositionRole(newPosition.Id, roleId)).ToList();
                await _positionRoleRepository.InsertManyAsync(positionRoles, autoSave: true);
            }

            // --- 6. Trigger Role Synchronization ---
            // Needs to happen after all position and role changes are potentially saved
            await _userRoleSynchronizer.SynchronizeUserRolesAsync(userId);

            return newPosition;
        }

        [UnitOfWork]
        public virtual async Task UpdateRolesAsync(
            [NotNull] Position position,
            [NotNull] List<Guid> newRoleIds)
        {
            Check.NotNull(position, nameof(position));
            Check.NotNull(newRoleIds, nameof(newRoleIds));

            // --- 1. Validate Input Roles ---
            await CheckRolesExistAsync(newRoleIds);

            // --- 2. Remove current roles ---
            // It's often safer/simpler to remove all and add new ones than to calculate diffs
            await _positionRoleRepository.DeleteByPositionIdAsync(position.Id);

            // --- 3. Add new roles ---
            if (newRoleIds.Any())
            {
                var uniqueRoleIds = newRoleIds.Distinct().ToList();
                var positionRoles = uniqueRoleIds.Select(roleId => new PositionRole(position.Id, roleId)).ToList();
                // Use autoSave: true as these might be the last operations before sync
                await _positionRoleRepository.InsertManyAsync(positionRoles, autoSave: true);
            }

            // --- 4. Trigger Role Synchronization ---
            await _userRoleSynchronizer.SynchronizeUserRolesAsync(position.UserId);
        }

        [UnitOfWork]
        public virtual async Task SetPrimaryPositionAsync(Guid userId, Guid positionId)
        {
            // --- 1. Get the position to be set as primary ---
            var positionToSet = await _positionRepository.FindAsync(positionId);
             if (positionToSet == null)
            {
                 throw new BusinessException(CoreFWDomainErrorCodes.PositionNotFound).WithData("PositionId", positionId);
            }
            if (positionToSet.UserId != userId)
            {
                 // Maybe a different error code? Or just use PositionNotFound for security.
                 throw new BusinessException(CoreFWDomainErrorCodes.PositionNotFound).WithData("PositionId", positionId);
                 // Or: throw new UserFriendlyException("Position does not belong to the specified user.");
            }

            // --- 2. Check if already primary ---
            if (positionToSet.IsPrimary)
            {
                return; // Nothing to do
            }

            // --- 3. Unset current primary (if exists) ---
            var currentPrimary = await _positionRepository.FindPrimaryPositionByUserIdAsync(userId);
            if (currentPrimary != null)
            {
                currentPrimary.SetPrimary(false);
                await _positionRepository.UpdateAsync(currentPrimary, autoSave: true);
            }

            // --- 4. Set the new position as primary ---
            positionToSet.SetPrimary(true);
            await _positionRepository.UpdateAsync(positionToSet, autoSave: true);

            // Note: Role synchronization is NOT typically needed when only changing the primary flag.
        }

        [UnitOfWork]
        public virtual async Task DeleteAsync([NotNull] Position position)
        {
            Check.NotNull(position, nameof(position));
            var userId = position.UserId; // Store userId before deleting position

            // --- 1. Delete associated PositionRoles ---
            await _positionRoleRepository.DeleteByPositionIdAsync(position.Id);

            // --- 2. Delete the Position itself ---
            await _positionRepository.DeleteAsync(position, autoSave: true);

            // --- 3. Trigger Role Synchronization for the user ---
            await _userRoleSynchronizer.SynchronizeUserRolesAsync(userId);

            // --- 4. Handle Primary Position logic after deletion ---
            if (position.IsPrimary)
            {
                var remainingPositions = await _positionRepository.GetListByUserIdAsync(userId);
                if (remainingPositions.Any())
                {
                    // Define a strategy: e.g., make the first remaining position primary.
                    var newPrimary = remainingPositions.OrderBy(p => p.CreationTime).First(); // Or OrderBy Name, etc.
                    newPrimary.SetPrimary(true);
                    await _positionRepository.UpdateAsync(newPrimary, autoSave: true);
                }
            }
        }

        // --- Helper methods for validation ---
        private async Task CheckUserExistsAsync(Guid userId)
        {
            // UserManager.FindByIdAsync returns null if not found, GetByIdAsync throws exception
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new BusinessException("CoreFW:USR001", $"User with ID '{userId}' not found."); // Define proper error code
            }
        }

        private async Task CheckOrganizationUnitExistsAsync(Guid ouId)
        {
            // IOrganizationUnitRepository.FindAsync returns null if not found
            var ou = await _organizationUnitRepository.FindAsync(ouId);
            if (ou == null)
            {
                 throw new BusinessException("CoreFW:ORG009", $"Organization Unit with ID '{ouId}' not found."); // Define proper error code
            }
        }

        private async Task CheckJobTitleExistsAsync(Guid jobTitleId)
        {
             var jobTitle = await _jobTitleRepository.FindAsync(jobTitleId);
             if (jobTitle == null)
             {
                  throw new BusinessException("CoreFW:JT001", $"Job Title with ID '{jobTitleId}' not found."); // Define proper error code
             }
        }

         private async Task CheckRolesExistAsync(List<Guid> roleIds)
        {
            foreach (var roleId in roleIds.Distinct())
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role == null)
                {
                     throw new BusinessException("CoreFW:ROL001", $"Role with ID '{roleId}' not found."); // Define proper error code
                }
            }
        }
    }