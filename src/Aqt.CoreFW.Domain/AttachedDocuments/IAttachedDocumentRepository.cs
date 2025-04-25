using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.AttachedDocuments; // Namespace chứa Enum
using Aqt.CoreFW.Domain.AttachedDocuments.Entities; // Namespace chứa Entity
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.AttachedDocuments;

/// <summary>
/// Defines the repository interface for the AttachedDocument entity.
/// </summary>
public interface IAttachedDocumentRepository : IRepository<AttachedDocument, Guid> // Kế thừa IRepository cơ bản
{
    /// <summary>
    /// Finds an AttachedDocument by its code within a specific Procedure.
    /// </summary>
    /// <param name="code">The code to search for.</param>
    /// <param name="procedureId">The ID of the Procedure.</param>
    /// <param name="includeDetails">Include details like Procedure navigation property (if defined).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found AttachedDocument or null.</returns>
    Task<AttachedDocument?> FindByCodeAsync(
        [NotNull] string code,
        Guid procedureId, // Bắt buộc vì Code unique theo Procedure
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of AttachedDocuments based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="procedureId">Filter by Procedure ID.</param> // Quan trọng để lọc
    /// <param name="sorting">Sorting expression (e.g., "Order ASC, Name ASC").</param>
    /// <param name="maxResultCount">Maximum number of results to return.</param>
    /// <param name="skipCount">Number of results to skip (for pagination).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of AttachedDocuments.</returns>
    Task<List<AttachedDocument>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        AttachedDocumentStatus? status = null,
        Guid? procedureId = null, // Thường sẽ lọc theo Procedure này
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of AttachedDocuments based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="procedureId">Filter by Procedure ID.</param> // Quan trọng để đếm
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total count.</returns>
    Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        AttachedDocumentStatus? status = null,
        Guid? procedureId = null, // Thường sẽ lọc theo Procedure này
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an AttachedDocument with the given code already exists within the specified Procedure.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <param name="procedureId">The Procedure ID where uniqueness is checked.</param>
    /// <param name="excludeId">Optional: Exclude this ID from the check (used during update).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the code exists in the procedure, false otherwise.</returns>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid procedureId, // Bắt buộc
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of AttachedDocuments belonging to a specific Procedure, often used for lookups/dropdowns.
    /// </summary>
    /// <param name="procedureId">The ID of the Procedure to filter by.</param>
    /// <param name="onlyActive">Whether to return only active AttachedDocuments (default: true).</param>
    /// <param name="sorting">Sorting expression (default: "Order ASC, Name ASC").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of AttachedDocuments matching the criteria.</returns>
    Task<List<AttachedDocument>> GetListByProcedureIdAsync(
        Guid procedureId,
        bool onlyActive = true,
        string? sorting = "Order ASC, Name ASC", // Cung cấp giá trị mặc định hợp lý
        CancellationToken cancellationToken = default);
} 