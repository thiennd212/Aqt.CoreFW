    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Domain.OrgStructure.Entities; // Reference to the Position entity
    using Volo.Abp.Domain.Repositories;

    namespace Aqt.CoreFW.Domain.OrgStructure.Repositories;

    /// <summary>
    /// Defines the repository interface for the <see cref="Position"/> entity.
    /// </summary>
    public interface IPositionRepository : IRepository<Position, Guid>
    {
        /// <summary>
        /// Gets a list of positions assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="includeDetails">If true, includes related OrganizationUnit and JobTitle entities (consider performance implications).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of the user's <see cref="Position"/> entities.</returns>
        Task<List<Position>> GetListByUserIdAsync(
            Guid userId,
            bool includeDetails = false, // Use with caution, prefer specific DTOs/projections
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds the primary position assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The primary <see cref="Position"/> if found; otherwise, null.</returns>
        Task<Position?> FindPrimaryPositionByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of positions assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The number of positions assigned to the user.</returns>
        Task<long> GetCountByUserIdAsync(
             Guid userId,
             CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of positions within a specific organization unit.
        /// </summary>
        /// <param name="organizationUnitId">The ID of the organization unit.</param>
        /// <param name="includeDetails">If true, includes related User and JobTitle entities (consider performance implications).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="Position"/> entities within the OU.</returns>
        Task<List<Position>> GetListByOrganizationUnitIdAsync(
            Guid organizationUnitId,
            bool includeDetails = false, // Use with caution
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of distinct User IDs assigned to positions within a specific organization unit, optionally filtered by job title.
        /// </summary>
        /// <param name="organizationUnitId">The ID of the organization unit.</param>
        /// <param name="jobTitleId">Optional filter by job title ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of unique User IDs.</returns>
        Task<List<Guid>> GetUserIdsInOrganizationUnitAsync(
            Guid organizationUnitId,
            Guid? jobTitleId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a projected list of positions including related entity names for display purposes.
        /// </summary>
        /// <param name="filterText">Filter text (searches User Name, OU Name, Job Title Name).</param>
        /// <param name="userId">Optional filter by user ID.</param>
        /// <param name="organizationUnitId">Optional filter by organization unit ID.</param>
        /// <param name="jobTitleId">Optional filter by job title ID.</param>
        /// <param name="sorting">Sorting criteria (e.g., "UserName ASC", "OrganizationUnitName DESC").</param>
        /// <param name="maxResultCount">Maximum number of results.</param>
        /// <param name="skipCount">Number of results to skip.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="PositionWithDetails"/> objects.</returns>
        Task<List<PositionWithDetails>> GetListWithDetailsAsync(
            string? filterText = null,
            Guid? userId = null,
            Guid? organizationUnitId = null,
            Guid? jobTitleId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of positions matching the detailed filter criteria.
        /// </summary>
        /// <param name="filterText">Filter text (matching GetListWithDetailsAsync).</param>
        /// <param name="userId">Optional filter by user ID.</param>
        /// <param name="organizationUnitId">Optional filter by organization unit ID.</param>
        /// <param name="jobTitleId">Optional filter by job title ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The total count of matching positions.</returns>
         Task<long> GetCountWithDetailsAsync(
            string? filterText = null,
            Guid? userId = null,
            Guid? organizationUnitId = null,
            Guid? jobTitleId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a specific position based on the combination of User, Organization Unit, and Job Title.
        /// Used to check for duplicates before creating a new position.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="organizationUnitId">The organization unit ID.</param>
        /// <param name="jobTitleId">The job title ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="Position"/> if found; otherwise, null.</returns>
        Task<Position?> FindByUserOUJobTitleAsync(
            Guid userId,
            Guid organizationUnitId,
            Guid jobTitleId,
            CancellationToken cancellationToken = default);

         /// <summary>
         /// Checks if a specific organization unit is currently assigned in any position.
         /// Used to prevent deletion of OUs that are in use.
         /// </summary>
         /// <param name="organizationUnitId">The ID of the organization unit.</param>
         /// <param name="cancellationToken">A cancellation token.</param>
         /// <returns>True if the OU is used in any position; otherwise, false.</returns>
        Task<bool> IsOrganizationUnitUsedAsync(
            Guid organizationUnitId,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A helper record (or class) to hold joined data for position listings.
    /// This is often defined near the repository or in Application.Contracts DTOs.
    /// </summary>
    public record PositionWithDetails(
        Guid Id,
        Guid UserId,
        string UserName, // Requires joining with User or retrieving separately
        Guid OrganizationUnitId,
        string OrganizationUnitName, // Requires joining with OrganizationUnit
        Guid JobTitleId,
        string JobTitleName, // Requires joining with JobTitle
        bool IsPrimary,
        DateTime CreationTime
    );