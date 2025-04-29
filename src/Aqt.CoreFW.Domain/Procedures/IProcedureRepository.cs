using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Procedures; // Namespace chứa Enum
using Aqt.CoreFW.Domain.Procedures.Entities; // Namespace chứa Entity
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.Procedures;

/// <summary>
/// Defines the repository interface for the Procedure entity.
/// </summary>
public interface IProcedureRepository : IRepository<Procedure, Guid> // Kế thừa IRepository cơ bản
{
    /// <summary>
    /// Finds a Procedure by its unique code.
    /// </summary>
    /// <param name="code">The code to search for.</param>
    /// <param name="includeDetails">Include details (if any navigation properties are defined).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found Procedure or null.</returns>
    Task<Procedure?> FindByCodeAsync(
        [NotNull] string code,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of Procedures based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="sorting">Sorting expression (e.g., "Order ASC, Name ASC").</param>
    /// <param name="maxResultCount">Maximum number of results to return.</param>
    /// <param name="skipCount">Number of results to skip (for pagination).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of Procedures.</returns>
    Task<List<Procedure>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        ProcedureStatus? status = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of Procedures based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total count.</returns>
    Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        ProcedureStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a Procedure with the given code already exists.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <param name="excludeId">Optional: Exclude this ID from the check (used during update).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the code exists, false otherwise.</returns>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// GetLookupAsync
    /// </summary>
    Task<List<Procedure>> GetLookupAsync(
        bool onlyActive = true,
        string? sorting = "Order ASC, Name ASC",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of Procedures by their IDs.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Procedure>> GetListByIdsAsync(
        List<Guid> ids,
        CancellationToken cancellationToken = default);
} 