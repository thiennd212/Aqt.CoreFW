using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Countries.Entities;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.Countries.Repositories;

/// <summary>
/// Defines the custom repository interface for the <see cref="Country"/> entity.
/// </summary>
public interface ICountryRepository : IRepository<Country, Guid>
{
    /// <summary>
    /// Finds a country by its unique code.
    /// </summary>
    /// <param name="code">The country code.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Country"/> or null if not found.</returns>
    Task<Country?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a country code already exists, optionally excluding a specific country ID (useful for updates).
    /// </summary>
    /// <param name="code">The country code to check.</param>
    /// <param name="excludedId">The ID of the country to exclude from the check.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the code exists, false otherwise.</returns>
    Task<bool> CodeExistsAsync(
        string code,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of countries based on filter criteria, sorting, and pagination.
    /// </summary>
    /// <param name="filterText">A filter text to apply (searches in Code and Name).</param>
    /// <param name="sorting">The sorting order (e.g., "Name asc", "Code desc").</param>
    /// <param name="maxResultCount">The maximum number of results to return.</param>
    /// <param name="skipCount">The number of results to skip (for pagination).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Country"/> entities.</returns>
    Task<List<Country>> GetListAsync(
        string? filterText = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of countries based on filter criteria.
    /// </summary>
    /// <param name="filterText">A filter text to apply (searches in Code and Name).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total count of matching countries.</returns>
    Task<long> GetCountAsync(
        string? filterText = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a country has any associated provinces/cities (Requires Province entity and repository).
    /// </summary>
    /// <param name="countryId">The ID of the country to check.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the country has associated provinces, false otherwise.</returns>
    Task<bool> HasProvincesAsync(
        Guid countryId,
        CancellationToken cancellationToken = default);
} 