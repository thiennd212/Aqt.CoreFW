    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Domain.JobTitles.Entities; // Tham chiếu đến Entity vừa tạo
    using Volo.Abp.Domain.Repositories;

    namespace Aqt.CoreFW.Domain.JobTitles;

    /// <summary>
    /// Defines the custom repository interface for the JobTitle entity.
    /// Extends the standard IRepository provided by ABP Framework.
    /// </summary>
    public interface IJobTitleRepository : IRepository<JobTitle, Guid>
    {
        /// <summary>
        /// Finds a job title by its unique code.
        /// </summary>
        /// <param name="code">The job title code to search for.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The JobTitle entity if found; otherwise, null.</returns>
        Task<JobTitle?> FindByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a job title code already exists, optionally excluding a specific ID (useful for updates).
        /// </summary>
        /// <param name="code">The code to check.</param>
        /// <param name="excludedId">The ID of the job title to exclude from the check (usually the one being updated).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>True if the code exists for another entity; otherwise, false.</returns>
        Task<bool> CodeExistsAsync(
            string code,
            Guid? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of job titles based on filtering, sorting, and pagination parameters.
        /// </summary>
        /// <param name="filterText">Text to filter by Code or Name.</param>
        /// <param name="isActive">Filter by active status (null for all).</param>
        /// <param name="sorting">Sorting expression (e.g., "Name ASC", "Code DESC").</param>
        /// <param name="maxResultCount">Maximum number of results to return.</param>
        /// <param name="skipCount">Number of results to skip (for pagination).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of JobTitle entities.</returns>
        Task<List<JobTitle>> GetListAsync(
            string? filterText = null,
            bool? isActive = null, // Thêm bộ lọc trạng thái
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of job titles based on the filtering parameters.
        /// </summary>
        /// <param name="filterText">Text to filter by Code or Name.</param>
        /// <param name="isActive">Filter by active status (null for all).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The total number of matching job titles.</returns>
        Task<long> GetCountAsync(
            string? filterText = null,
            bool? isActive = null, // Thêm bộ lọc trạng thái
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a specific job title is currently assigned to any employees.
        /// (Implementation logic will be added later when the Employee module exists).
        /// </summary>
        /// <param name="jobTitleId">The ID of the job title to check.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>True if the job title is assigned to employees; otherwise, false.</returns>
        Task<bool> HasEmployeesAsync(
            Guid jobTitleId,
            CancellationToken cancellationToken = default);
    }