    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Domain.OrgStructure.Entities; // Reference to the UserProfile entity
    using Volo.Abp.Domain.Repositories;

    namespace Aqt.CoreFW.Domain.OrgStructure.Repositories;

    /// <summary>
    /// Defines the repository interface for the <see cref="UserProfile"/> entity.
    /// </summary>
    public interface IUserProfileRepository : IRepository<UserProfile, Guid>
    {
        /// <summary>
        /// Finds a user profile by the associated AbpIdentityUser ID.
        /// </summary>
        /// <param name="userId">The ID of the AbpIdentityUser.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="UserProfile"/> if found; otherwise, null.</returns>
        Task<UserProfile?> FindByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of user profiles based on filtering, sorting, and pagination parameters.
        /// </summary>
        /// <param name="filterText">A filter to apply (e.g., search by FullName, PhoneNumber, Address).</param>
        /// <param name="sorting">Sorting criteria (e.g., "FullName ASC").</param>
        /// <param name="maxResultCount">Maximum number of results to return.</param>
        /// <param name="skipCount">Number of results to skip (for pagination).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="UserProfile"/> entities.</returns>
        Task<List<UserProfile>> GetListAsync(
            string? filterText = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of user profiles based on filtering parameters.
        /// </summary>
        /// <param name="filterText">A filter to apply (matching the one used in GetListAsync).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The total count of matching user profiles.</returns>
        Task<long> GetCountAsync(
            string? filterText = null,
            CancellationToken cancellationToken = default);

        // Add more specific query methods if needed, e.g., FindByPhoneNumberAsync
    }