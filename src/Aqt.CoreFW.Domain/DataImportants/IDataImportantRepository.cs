using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.DataImportants; // Namespace chứa Enum
using Aqt.CoreFW.Domain.DataImportants.Entities; // Namespace chứa Entity
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.DataImportants;

/// <summary>
/// Defines the repository interface for the DataImportant entity.
/// </summary>
public interface IDataImportantRepository : IRepository<DataImportant, Guid> // Kế thừa IRepository cơ bản
{
    /// <summary>
    /// Finds a DataImportant by its code within a specific DataGroup.
    /// </summary>
    /// <param name="code">The code to search for.</param>
    /// <param name="dataGroupId">The ID of the DataGroup.</param>
    /// <param name="includeDetails">Include details like DataGroup navigation property (if defined).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found DataImportant or null.</returns>
    Task<DataImportant?> FindByCodeAsync(
        [NotNull] string code,
        Guid dataGroupId, // Bắt buộc vì Code unique theo Group
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of DataImportants based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="dataGroupId">Filter by DataGroup ID.</param> // Quan trọng để lọc
    /// <param name="sorting">Sorting expression (e.g., "Order ASC, Name ASC").</param>
    /// <param name="maxResultCount">Maximum number of results to return.</param>
    /// <param name="skipCount">Number of results to skip (for pagination).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of DataImportants.</returns>
    Task<List<DataImportant>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataImportantStatus? status = null,
        Guid? dataGroupId = null, // Thường sẽ lọc theo Group này
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of DataImportants based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="dataGroupId">Filter by DataGroup ID.</param> // Quan trọng để đếm
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total count.</returns>
    Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataImportantStatus? status = null,
        Guid? dataGroupId = null, // Thường sẽ lọc theo Group này
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a DataImportant with the given code already exists within the specified DataGroup.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <param name="dataGroupId">The DataGroup ID where uniqueness is checked.</param>
    /// <param name="excludeId">Optional: Exclude this ID from the check (used during update).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the code exists in the group, false otherwise.</returns>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid dataGroupId, // Bắt buộc
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of DataImportants belonging to a specific DataGroup, often used for lookups/dropdowns.
    /// </summary>
    /// <param name="dataGroupId">The ID of the DataGroup to filter by.</param>
    /// <param name="onlyActive">Whether to return only active DataImportants (default: true).</param>
    /// <param name="sorting">Sorting expression (default: "Order ASC, Name ASC").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of DataImportants matching the criteria.</returns>
    Task<List<DataImportant>> GetListByDataGroupIdAsync(
        Guid dataGroupId,
        bool onlyActive = true,
        string? sorting = "Order ASC, Name ASC", // Cung cấp giá trị mặc định hợp lý
        CancellationToken cancellationToken = default);
} 