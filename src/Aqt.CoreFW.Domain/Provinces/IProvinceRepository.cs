// src/Aqt.CoreFW.Domain/Provinces/IProvinceRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Provinces.Entities;
using Aqt.CoreFW.Domain.Shared.Provinces;
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.Provinces;

public interface IProvinceRepository : IRepository<Province, Guid>
{
    /// <summary>
    /// Finds a province by its unique code.
    /// </summary>
    Task<Province?> FindByCodeAsync(
        [NotNull] string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a province by its name within a specific country.
    /// </summary>
    Task<Province?> FindByNameAsync(
        [NotNull] string name,
        Guid countryId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a province with the given code already exists.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <param name="excludedId">An optional ID to exclude from the check (used during updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the code exists, false otherwise.</returns>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    // Optional: Check if name exists within the same country.
    // Task<bool> NameExistsInCountryAsync(
    //     [NotNull] string name,
    //     Guid countryId,
    //     Guid? excludedId = null,
    //     CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of provinces based on filtering, sorting, and paging parameters.
    /// </summary>
    Task<List<Province>> GetListAsync(
        string? filterText = null,
        ProvinceStatus? status = null,
        Guid? countryId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of provinces based on filtering parameters.
    /// </summary>
    Task<long> GetCountAsync(
        string? filterText = null,
        ProvinceStatus? status = null,
        Guid? countryId = null,
        CancellationToken cancellationToken = default);
}