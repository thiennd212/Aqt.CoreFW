using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.DataCores;
using Aqt.CoreFW.Domain.DataCores.Entities; // Namespace chứa Entity
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.DataCores;

/// <summary>
/// Defines the repository interface for the DataCore entity.
/// </summary>
public interface IDataCoreRepository : IRepository<DataCore, Guid> // Kế thừa IRepository cơ bản
{
    /// <summary>
    /// Finds a DataCore by its unique code (assuming global uniqueness).
    /// </summary>
    /// <param name="code">The code to search for.</param>
    /// <param name="includeDetails">Include details like DataGroup navigation property (if defined and needed).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found DataCore or null.</returns>
    Task<DataCore?> FindByCodeAsync(
        [NotNull] string code,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    // Nếu Code unique theo DataGroupId, bạn sẽ cần một overload như sau:
    // Task<DataCore?> FindByCodeAsync(
    //     [NotNull] string code,
    //     Guid dataGroupId,
    //     bool includeDetails = false,
    //     CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of DataCores based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="dataGroupId">Filter by DataGroup ID.</param>
    /// <param name="sorting">Sorting expression (e.g., "Name ASC").</param>
    /// <param name="maxResultCount">Maximum number of results to return.</param>
    /// <param name="skipCount">Number of results to skip (for pagination).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of DataCores.</returns>
    Task<List<DataCore>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataCoreStatus? status = null,
        Guid? dataGroupId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of DataCores based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="dataGroupId">Filter by DataGroup ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total count.</returns>
    Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataCoreStatus? status = null,
        Guid? dataGroupId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a DataCore with the given code already exists (assuming global uniqueness).
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <param name="excludeId">Optional: Exclude this ID from the check (used during update).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the code exists, false otherwise.</returns>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    // Nếu Code unique theo DataGroupId, bạn sẽ cần một overload như sau:
    // Task<bool> CodeExistsAsync(
    //     [NotNull] string code,
    //     Guid dataGroupId,
    //     Guid? excludeId = null,
    //     CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of DataCores belonging to a specific DataGroup, often used for lookups/dropdowns.
    /// </summary>
    /// <param name="dataGroupId">The ID of the DataGroup to filter by.</param>
    /// <param name="onlyActive">Whether to return only active DataCores (default: true).</param>
    /// <param name="sorting">Sorting expression (default: "Order ASC, Name ASC").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of DataCores matching the criteria.</returns>
    Task<List<DataCore>> GetListByDataGroupIdAsync(
        Guid dataGroupId,
        bool onlyActive = true,
        string? sorting = null, // Mặc định có thể là "Order ASC, Name ASC" trong implementation
        CancellationToken cancellationToken = default);
} 