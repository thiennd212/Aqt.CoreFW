using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for OrderBy with string
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.WorkflowStatuses; // Repository Interface
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities; // Entity
                                                   // using Aqt.CoreFW.Domain.OtherWorkflowEntities; // TODO: Add using if needed for IsInUseAsync
using Aqt.CoreFW.EntityFrameworkCore; // DbContext
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.WorkflowStatuses;

/// <summary>
/// Implements the custom repository for the WorkflowStatus entity using Entity Framework Core.
/// </summary>
public class WorkflowStatusRepository :
    EfCoreRepository<CoreFWDbContext, WorkflowStatus, Guid>, // Inherit from ABP base repository
    IWorkflowStatusRepository // Implement the custom repository interface
{
    public WorkflowStatusRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc/>
    public async Task<WorkflowStatus?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(ws => ws.Code == code, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<WorkflowStatus?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(ws => ws.Name == name, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Check if any entity has the code, excluding the specified ID if provided
        return await dbSet.AnyAsync(ws => ws.Code == code && (!excludedId.HasValue || ws.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<bool> NameExistsAsync(string name, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Check if any entity has the name, excluding the specified ID if provided
        return await dbSet.AnyAsync(ws => ws.Name == name && (!excludedId.HasValue || ws.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<List<WorkflowStatus>> GetListAsync(
        string? filterText = null,
        bool? isActive = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = ApplyFilter(dbSet, filterText, isActive); // Apply filters

        // Apply sorting, default to Order then Name if not specified
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ? $"{nameof(WorkflowStatus.Order)} asc, {nameof(WorkflowStatus.Name)} asc" : sorting);

        // Apply paging
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<long> GetCountAsync(
        string? filterText = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = ApplyFilter(dbSet, filterText, isActive); // Apply the same filters as GetListAsync

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<bool> IsInUseAsync(Guid workflowStatusId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement the actual check when related workflow entities (e.g., WorkflowDefinition, WorkflowInstance) are available.
        // This check prevents deletion of statuses that are actively part of a defined or running workflow.
        /* Example (requires related DbSets in CoreFWDbContext):
        var dbContext = await GetDbContextAsync();
        // Check if used in any WorkflowDefinition (adjust property names as needed)
        var usedInDefinition = await dbContext.WorkflowDefinitions
                                  .AnyAsync(wd => wd.InitialStatusId == workflowStatusId || wd.Transitions.Any(t => t.FromStatusId == workflowStatusId || t.ToStatusId == workflowStatusId), GetCancellationToken(cancellationToken));
        if (usedInDefinition) return true;

        // Check if used in any active WorkflowInstance (adjust property names as needed)
         var usedInInstance = await dbContext.WorkflowInstances
                                 .AnyAsync(wi => wi.CurrentStatusId == workflowStatusId && !wi.IsCompleted, GetCancellationToken(cancellationToken));
         if (usedInInstance) return true;
        */

        await Task.CompletedTask; // Placeholder for async compilation
        return false; // IMPORTANT: Returning false temporarily until the check is implemented.
    }

    // Helper method to apply common filters consistently
    private IQueryable<WorkflowStatus> ApplyFilter(IQueryable<WorkflowStatus> query, string? filterText, bool? isActive)
    {
        return query
            .WhereIf(!filterText.IsNullOrWhiteSpace(),
                ws => ws.Code.Contains(filterText!) || ws.Name.Contains(filterText!)) // Notice the '!' for non-null assertion
            .WhereIf(isActive.HasValue, ws => ws.IsActive == isActive.Value);
    }

    // Override GetQueryableAsync to ensure related data (if any) is not tracked by default for read operations
    // This can improve performance slightly for GetListAsync, GetCountAsync etc.
    public override async Task<IQueryable<WorkflowStatus>> GetQueryableAsync()
    {
        var queryable = await base.GetQueryableAsync();
        return queryable.AsNoTracking(); // Apply AsNoTracking for read operations by default in this repo
    }

    /// <summary>
    /// Fetches a list of WorkflowStatus entities by their IDs.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<WorkflowStatus>> GetListByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Fetch the list of WorkflowStatus entities by their IDs
        return await dbSet
            .Where(ws => ids.Contains(ws.Id))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}