using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities; // Namespace tới Entity vừa tạo
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.WorkflowStatuses;

/// <summary>
/// Defines the custom repository interface for the <see cref="WorkflowStatus"/> entity.
/// Extends the standard ABP <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
public interface IWorkflowStatusRepository : IRepository<WorkflowStatus, Guid>
{
    /// <summary>
    /// Finds a workflow status by its unique code.
    /// </summary>
    /// <param name="code">The status code to search for.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="WorkflowStatus"/> if found; otherwise, null.</returns>
    Task<WorkflowStatus?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a workflow status by its unique name.
    /// </summary>
    /// <param name="name">The status name to search for.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The <see cref="WorkflowStatus"/> if found; otherwise, null.</returns>
    Task<WorkflowStatus?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a workflow status code already exists, optionally excluding a specific ID.
    /// </summary>
    /// <param name="code">The status code to check.</param>
    /// <param name="excludedId">The ID of the status to exclude from the check (used during updates).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>True if the code exists for another status; otherwise, false.</returns>
    Task<bool> CodeExistsAsync(
        string code,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a workflow status name already exists, optionally excluding a specific ID.
    /// </summary>
    /// <param name="name">The status name to check.</param>
    /// <param name="excludedId">The ID of the status to exclude from the check (used during updates).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>True if the name exists for another status; otherwise, false.</returns>
    Task<bool> NameExistsAsync(
        string name,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of workflow statuses based on filtering, sorting, and pagination parameters.
    /// </summary>
    /// <param name="filterText">A filter to apply to the Code or Name properties.</param>
    /// <param name="isActive">Filter by the active status (null for all).</param>
    /// <param name="sorting">The sorting order (e.g., "Name ASC", "Order DESC").</param>
    /// <param name="maxResultCount">The maximum number of results to return.</param>
    /// <param name="skipCount">The number of results to skip (for pagination).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of <see cref="WorkflowStatus"/> entities.</returns>
    Task<List<WorkflowStatus>> GetListAsync(
        string? filterText = null,
        bool? isActive = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of workflow statuses based on filtering parameters.
    /// </summary>
    /// <param name="filterText">A filter to apply to the Code or Name properties.</param>
    /// <param name="isActive">Filter by the active status (null for all).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The total count of matching <see cref="WorkflowStatus"/> entities.</returns>
    Task<long> GetCountAsync(
        string? filterText = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a specific workflow status is currently associated with any workflow instances (implementation pending).
    /// </summary>
    /// <param name="workflowStatusId">The ID of the workflow status to check.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>True if the status is in use; otherwise, false (currently placeholder).</returns>
    Task<bool> IsInUseAsync(
        Guid workflowStatusId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of workflow statuses by their unique IDs.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<WorkflowStatus>> GetListByIdsAsync(
        List<Guid> ids,
        CancellationToken cancellationToken = default);
}