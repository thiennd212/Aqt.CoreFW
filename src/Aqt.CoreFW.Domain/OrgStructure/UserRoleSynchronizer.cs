using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.OrgStructure.Repositories; // Updated path
using Aqt.CoreFW.Domain.OrgStructure.Services;   // Updated path
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;              // Required for IdentityResult.CheckErrors()
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;                          // Required for UserManager, RoleManager, etc.
using Volo.Abp.Uow;

namespace Aqt.CoreFW.Domain.OrgStructure;

/// <summary>
/// Implementation of the <see cref="IUserRoleSynchronizer"/> domain service.
/// Synchronizes roles in AbpIdentityUserRole based on Position assignments.
/// </summary>
public class UserRoleSynchronizer : DomainService, IUserRoleSynchronizer
{
    // Inject logger if needed for diagnostics
    public ILogger<UserRoleSynchronizer> Logger { get; set; }

    private readonly IPositionRepository _positionRepository;
    private readonly IPositionRoleRepository _positionRoleRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;

    public UserRoleSynchronizer(
            IPositionRepository positionRepository,
            IPositionRoleRepository positionRoleRepository,
            IdentityUserManager userManager,
            IdentityRoleManager roleManager)
    {
        _positionRepository = positionRepository;
        _positionRoleRepository = positionRoleRepository;
        _userManager = userManager;
        _roleManager = roleManager;

        // Initialize logger (optional)
        Logger = NullLogger<UserRoleSynchronizer>.Instance;
    }

    /// <summary>
    /// Synchronizes roles. Recommended to be called within an existing Unit of Work
    /// initiated by the operation that changed positions/roles (like PositionManager methods),
    /// but adding [UnitOfWork] here provides safety if called independently.
    /// </summary>
    [UnitOfWork]
    public virtual async Task SynchronizeUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            // Log this situation? User might have been deleted concurrently.
            Logger.LogWarning($"User with ID {userId} not found during role synchronization.");
            return;
        }

        // 1. Get all Position IDs for the user
        // We only need the IDs to fetch the associated roles efficiently.
        var userPositions = await _positionRepository.GetListByUserIdAsync(userId);
        var positionIds = userPositions.Select(p => p.Id).ToList();

        // 2. Get distinct Role IDs from these Positions
        var roleIdsFromPositions = new HashSet<Guid>(); // Use HashSet for uniqueness
        if (positionIds.Any())
        {
            var distinctRoleIds = await _positionRoleRepository.GetDistinctRoleIdsByPositionIdsAsync(positionIds);
            roleIdsFromPositions.UnionWith(distinctRoleIds);
        }

        // 3. Get Role Names from Role IDs (UserManager works with names)
        var targetRoleNames = new HashSet<string>();
        foreach (var roleId in roleIdsFromPositions)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role != null)
            {
                targetRoleNames.Add(role.Name);
            }
            else
            {
                // Log this potential inconsistency
                Logger.LogWarning($"Role with ID {roleId} assigned to a position for user {userId} was not found in Role Manager.");
            }
        }

        // 4. Get the user's current Role Names
        var currentRoleNames = new HashSet<string>(await _userManager.GetRolesAsync(user));

        // 5. Calculate roles to add and remove
        var roleNamesToAdd = targetRoleNames.Except(currentRoleNames).ToList();
        var roleNamesToRemove = currentRoleNames.Except(targetRoleNames).ToList();

        // 6. Update user roles via UserManager
        IdentityResult result;
        if (roleNamesToRemove.Any())
        {
            Logger.LogInformation($"Removing roles from user {userId}: {string.Join(", ", roleNamesToRemove)}");
            result = await _userManager.RemoveFromRolesAsync(user, roleNamesToRemove);
            // CheckErrors throws an AbpValidationException if there are errors
            result.CheckErrors(); // Pass LocalizationProvider if available for localized errors
        }

        if (roleNamesToAdd.Any())
        {
            Logger.LogInformation($"Adding roles to user {userId}: {string.Join(", ", roleNamesToAdd)}");
            result = await _userManager.AddToRolesAsync(user, roleNamesToAdd);
            result.CheckErrors();
        }

        if (!roleNamesToAdd.Any() && !roleNamesToRemove.Any())
        {
            Logger.LogDebug($"No role changes needed for user {userId}.");
        }
    }
}