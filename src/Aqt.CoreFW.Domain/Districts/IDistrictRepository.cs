using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Districts.Entities;
using Aqt.CoreFW.Domain.Shared.Districts; // For DistrictStatus enum
using JetBrains.Annotations; // For [NotNull]
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.Districts;

public interface IDistrictRepository : IRepository<District, Guid>
{
    /// <summary>
    /// Finds a district by its unique code.
    /// Assumes code is globally unique - adjust if uniqueness is per province.
    /// </summary>
    Task<District?> FindByCodeAsync(
        [NotNull] string code,
        CancellationToken cancellationToken = default); // Removed includeDetails as per latest plan

    /// <summary>
    /// Finds a district by its name within a specific province.
    /// </summary>
    Task<District?> FindByNameAsync( // Added FindByNameAsync as per plan
        [NotNull] string name,
        Guid provinceId, // Requires province context
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a district with the given code already exists.
    /// Assumes code is globally unique - adjust if uniqueness is per province.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <param name="excludedId">An optional ID to exclude from the check (used during updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the code exists, false otherwise.</returns>
    Task<bool> CodeExistsAsync( // Added CodeExistsAsync as per plan
        [NotNull] string code,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a district with the given name already exists within the specified province.
    /// </summary>
    Task<bool> NameExistsInProvinceAsync( // Added NameExistsInProvinceAsync as per plan
        [NotNull] string name,
        Guid provinceId,
        Guid? excludedId = null, // Exclude current district when updating
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of districts based on filtering, sorting, and paging parameters.
    /// Includes Province Name by joining or separate query if necessary for display.
    /// </summary>
    Task<List<District>> GetListAsync(
        string? filterText = null,         // Filter by Code or Name
        DistrictStatus? status = null,     // Filter by Status
        Guid? provinceId = null,           // Filter by Province
        string? sorting = null,            // Sorting parameters (e.g., "Name ASC")
        int maxResultCount = int.MaxValue, // Max items to return
        int skipCount = 0,                 // Items to skip (for paging)
        bool includeDetails = false,       // Flag to potentially include Province details
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of districts based on filtering parameters.
    /// </summary>
    Task<long> GetCountAsync(
        string? filterText = null,
        DistrictStatus? status = null, // Corrected parameter name from code to status
        Guid? provinceId = null,
        CancellationToken cancellationToken = default); // Removed code and name filters as they are covered by filterText

    /// <summary>
    /// Checks if a province with the given ID exists.
    /// Needed for validation in AppService.
    /// </summary>
    Task<bool> ProvinceExistsAsync(Guid provinceId, CancellationToken cancellationToken = default); // Kept this method as it's needed by AppService

    /// <summary>
    /// Gets a list of districts by ProvinceId. Useful for lookups or related data loading.
    /// </summary>
    Task<List<District>> GetListByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken = default); // Added as per plan suggestion
}