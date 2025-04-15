    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Domain.OrgStructure.Entities; // Reference to the JobTitle entity
    using Volo.Abp.Domain.Repositories;

    namespace Aqt.CoreFW.Domain.OrgStructure.Repositories;

    /// <summary>
    /// Defines the repository interface for the <see cref="JobTitle"/> entity.
    /// </summary>
    public interface IJobTitleRepository : IRepository<JobTitle, Guid>
    {
        /// <summary>
        /// Finds a job title by its unique code.
        /// </summary>
        /// <param name="code">The job title code.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="JobTitle"/> if found; otherwise, null.</returns>
        Task<JobTitle?> FindByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a job title code already exists, optionally excluding a specific entity ID.
        /// </summary>
        /// <param name="code">The code to check.</param>
        /// <param name="excludedId">The ID of the entity to exclude from the check (used for updates).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the code exists; otherwise, false.</returns>
        Task<bool> CodeExistsAsync(
            string code,
            Guid? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of job titles based on filtering, sorting, and pagination parameters.
        /// </summary>
        /// <param name="filterText">A filter to apply (e.g., search by Name or Code).</param>
        /// <param name="sorting">Sorting criteria (e.g., "Name ASC", "Code DESC").</param>
        /// <param name="maxResultCount">Maximum number of results to return.</param>
        /// <param name="skipCount">Number of results to skip (for pagination).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="JobTitle"/> entities.</returns>
        Task<List<JobTitle>> GetListAsync(
            string? filterText = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of job titles based on filtering parameters.
        /// </summary>
        /// <param name="filterText">A filter to apply (matching the one used in GetListAsync).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The total count of matching job titles.</returns>
        Task<long> GetCountAsync(
            string? filterText = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a specific job title is currently assigned to any position.
        /// </summary>
        /// <param name="jobTitleId">The ID of the job title to check.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the job title is used in any position; otherwise, false.</returns>
        Task<bool> IsUsedByPositionAsync(
            Guid jobTitleId,
            CancellationToken cancellationToken = default);
    }