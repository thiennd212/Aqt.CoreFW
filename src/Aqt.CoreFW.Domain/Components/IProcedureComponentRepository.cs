using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Components; // Namespace chứa Enum
using Aqt.CoreFW.Domain.Components.Entities; // Namespace chứa Entity
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.Components;

/// <summary>
/// Defines the repository interface for the ProcedureComponent aggregate root.
/// Extends the basic IRepository and adds custom methods.
/// </summary>
public interface IProcedureComponentRepository : IRepository<ProcedureComponent, Guid> // Kế thừa IRepository cơ bản
{
    /// <summary>
    /// Finds a ProcedureComponent by its unique code, optionally including details.
    /// </summary>
    /// <param name="code">The code to search for.</param>
    /// <param name="includeDetails">Whether to include navigation properties like ProcedureLinks. Defaults to true.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found ProcedureComponent or null.</returns>
    Task<ProcedureComponent?> FindByCodeAsync(
        [NotNull] string code,
        bool includeDetails = true, // Default true to include links when finding by code
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of ProcedureComponents based on filtering and pagination criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="type">Filter by component type.</param>
    /// <param name="procedureId">Optional: Filter by components linked to a specific Procedure.</param>
    /// <param name="sorting">Sorting expression (e.g., "Order ASC, Name DESC").</param>
    /// <param name="maxResultCount">Maximum number of results to return.</param>
    /// <param name="skipCount">Number of results to skip (for pagination).</param>
    /// <param name="includeDetails">Whether to include navigation properties like ProcedureLinks. Defaults to false for lists.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of ProcedureComponents.</returns>
    Task<List<ProcedureComponent>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        ComponentStatus? status = null,
        ComponentType? type = null,
        Guid? procedureId = null, // Added filter by Procedure
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false, // Default false for lists
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of ProcedureComponents based on filtering criteria.
    /// </summary>
    /// <param name="filterText">Text to search in Code or Name.</param>
    /// <param name="code">Filter by exact code.</param>
    /// <param name="name">Filter by name containing text.</param>
    /// <param name="status">Filter by status.</param>
    /// <param name="type">Filter by component type.</param>
    /// <param name="procedureId">Optional: Filter by components linked to a specific Procedure.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total count of matching ProcedureComponents.</returns>
    Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        ComponentStatus? status = null,
        ComponentType? type = null,
         Guid? procedureId = null, // Added filter by Procedure
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a ProcedureComponent with the given code already exists, optionally excluding one ID.
    /// </summary>
    /// <param name="code">The code to check for uniqueness.</param>
    /// <param name="excludeId">Optional: Exclude this Component ID from the check (used during update).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a component with the code exists (excluding the specified ID), false otherwise.</returns>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of Procedure IDs currently linked to a specific ProcedureComponent.
    /// </summary>
    /// <param name="componentId">The ID of the ProcedureComponent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of Guids representing the linked Procedure IDs.</returns>
    Task<List<Guid>> GetLinkedProcedureIdsAsync(
        Guid componentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the joining entities (ProcedureComponentLink) for a specific ProcedureComponent.
    /// This is primarily used internally by the ProcedureComponentManager to manage the links.
    /// </summary>
    /// <param name="componentId">The ID of the ProcedureComponent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of ProcedureComponentLink entities associated with the component.</returns>
    Task<List<ProcedureComponentLink>> GetComponentLinksAsync( // Đổi tên và kiểu trả về
        Guid componentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple ProcedureComponentLink entities efficiently.
    /// This method should be implemented in the concrete repository (e.g., EF Core) for performance.
    /// </summary>
    /// <param name="links">The list of ProcedureComponentLink entities to delete.</param>
    /// <param name="autoSave">Whether to automatically save changes. Defaults to false.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteManyComponentLinksAsync( // Đổi tên
         [NotNull] List<ProcedureComponentLink> links, // Đổi kiểu tham số, NotNull
         bool autoSave = false,
         CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts multiple ProcedureComponentLink entities efficiently.
    /// This method should be implemented in the concrete repository (e.g., EF Core) for performance.
    /// </summary>
    /// <param name="links">The list of ProcedureComponentLink entities to insert.</param>
    /// <param name="autoSave">Whether to automatically save changes. Defaults to false.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertManyComponentLinksAsync( // Đổi tên
         [NotNull] List<ProcedureComponentLink> links, // Đổi kiểu tham số, NotNull
         bool autoSave = false,
         CancellationToken cancellationToken = default);

     /// <summary>
    /// Gets a simplified list of Procedure Components for lookup purposes (e.g., dropdowns).
    /// </summary>
    /// <param name="type">Optional filter by component type.</param>
    /// <param name="onlyActive">Whether to return only active components. Defaults to true.</param>
    /// <param name="sorting">Sorting expression. Defaults to "Order ASC, Name ASC".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of ProcedureComponents suitable for lookup.</returns>
    Task<List<ProcedureComponent>> GetLookupAsync(
        ComponentType? type = null,
        bool onlyActive = true,
        string? sorting = "Order ASC, Name ASC",
        CancellationToken cancellationToken = default);
} 