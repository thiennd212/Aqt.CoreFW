using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.DataGroups; // IDataGroupRepository
using Aqt.CoreFW.Domain.DataGroups.Entities; // DataGroup Entity
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext, EfCoreRepository
using Aqt.CoreFW.DataGroups; // DataGroupStatus enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.DataGroups;

public class DataGroupRepository :
    EfCoreRepository<CoreFWDbContext, DataGroup, Guid>,
    IDataGroupRepository
{
    public DataGroupRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<DataGroup?> FindByCodeAsync(string code, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Include details is not strictly necessary for DataGroup itself unless Parent navigation prop is used
        return await dbSet.AsNoTracking()
                          .FirstOrDefaultAsync(dg => dg.Code == code, GetCancellationToken(cancellationToken));
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(dg => dg.Code == code && (!excludedId.HasValue || dg.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataGroup>> GetListAsync(
        string? filterText = null,
        DataGroupStatus? status = null,
        Guid? parentId = null,
        bool? parentIdIsNull = null, // Thêm filter cho ParentId == null
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false, // Option to include parent (if needed later)
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, status, parentId, parentIdIsNull, includeDetails);

        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(DataGroup.Order)} asc, {nameof(DataGroup.Name)} asc" // Default sort
            : sorting);

        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        DataGroupStatus? status = null,
        Guid? parentId = null,
        bool? parentIdIsNull = null, // Thêm filter cho ParentId == null
        CancellationToken cancellationToken = default)
    {
        // Không cần includeDetails khi chỉ đếm
        var query = await GetListQueryInternalAsync(filterText, status, parentId, parentIdIsNull, includeDetails: false);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataGroup>> GetChildrenAsync(Guid parentId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking().Where(dg => dg.ParentId == parentId);
        // Apply include logic if needed
        // if (includeDetails) query = query.Include(...);
        return await query.OrderBy(dg => dg.Order).ThenBy(dg => dg.Name)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(dg => dg.ParentId == id, GetCancellationToken(cancellationToken));
    }

    // --- Hierarchical Methods (Implementation notes) ---

    public async Task<List<Guid>> GetAllDescendantIdsAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        // Recommended: Implement using Recursive CTE (Common Table Expression) with raw SQL.
        // This is the most efficient way for most databases.
        // The exact SQL syntax depends on your database (SQL Server, PostgreSQL, etc.).
        // Example (Conceptual SQL Server CTE):
        /*
        WITH DescendantsCTE AS (
            SELECT Id FROM CoreFWDataGroups WHERE ParentId = @parentId
            UNION ALL
            SELECT dg.Id FROM CoreFWDataGroups dg INNER JOIN DescendantsCTE cte ON dg.ParentId = cte.Id
        )
        SELECT Id FROM DescendantsCTE;
        */
        var dbContext = await GetDbContextAsync();
        var sql = GetRecursiveCteSqlForIds(dbContext, parentId); // Helper to generate SQL based on provider

        // Execute raw SQL and get the list of Guids
        // Note: Use FromSqlRaw or FromSqlInterpolated depending on EF Core version and parameter handling needs
        // Need to handle potential SQL injection if not using parameterized queries correctly.
        var descendantIds = await dbContext.Database
            .SqlQueryRaw<Guid>(sql) // Adjust based on EF Core version/syntax
            .ToListAsync(GetCancellationToken(cancellationToken));

        return descendantIds;

        // // --- Alternative (Less efficient, OK for small/medium datasets): Iterative fetching ---
        // var ids = new List<Guid>();
        // var queue = new Queue<Guid>();
        // queue.Enqueue(parentId);
        // while(queue.Count > 0)
        // {
        //     var currentParentId = queue.Dequeue();
        //     var children = await GetDbSetAsync().Where(x => x.ParentId == currentParentId).Select(x => x.Id).ToListAsync(cancellationToken);
        //     if(children.Any())
        //     {
        //         ids.AddRange(children);
        //         foreach(var childId in children) queue.Enqueue(childId);
        //     }
        // }
        // return ids;
    }

    public async Task<List<DataGroup>> GetAllDescendantsAsync(Guid parentId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        // Similar to GetAllDescendantIdsAsync, using CTE is preferred for efficiency.
        // Get the IDs first using CTE
        var descendantIds = await GetAllDescendantIdsAsync(parentId, cancellationToken);

        if (!descendantIds.Any())
        {
            return new List<DataGroup>();
        }

        // Fetch all descendants by their IDs
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking().Where(dg => descendantIds.Contains(dg.Id));

        // Apply include logic if needed
        // if (includeDetails) query = query.Include(...);

        return await query.ToListAsync(GetCancellationToken(cancellationToken));
    }


    // --- Overloads for Get/Find with includeDetails --- (Implement interface methods)

    // Giữ lại phương thức này để implement interface
    public async Task<DataGroup?> GetAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsQueryable(); // Track entity for potential updates if includeDetails=false

        if (includeDetails)
        {
            query = query.AsNoTracking(); // No tracking if details included (assuming read-only)
            // Apply include logic here, e.g.:
            // query = query.Include(x => x.Parent); // Assuming Parent navigation prop exists
        }

        // Returns nullable as per IRepository<TEntity, TKey>.GetAsync
        var entity = await query.FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));
        return entity;
    }

    // Giữ lại phương thức này để implement interface
    public async Task<DataGroup?> FindAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
         var dbSet = await GetDbSetAsync();
         var query = dbSet.AsNoTracking(); // Use NoTracking for Find

        if (includeDetails)
        {
            // Apply include logic if needed
            // query = query.Include(x => x.Parent); // Example
        }

         return await query.FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));
    }

    // --- Helper Methods ---

    // Private helper to build the base query for GetList/GetCount
    private async Task<IQueryable<DataGroup>> GetListQueryInternalAsync(
        string? filterText = null,
        DataGroupStatus? status = null,
        Guid? parentId = null,
        bool? parentIdIsNull = null,
        bool includeDetails = false)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking() // Start with NoTracking for reads
            .WhereIf(!filterText.IsNullOrWhiteSpace(),
                     dg => dg.Code.Contains(filterText) || dg.Name.Contains(filterText))
            .WhereIf(status.HasValue, dg => dg.Status == status.Value);

        // Handle ParentId filtering:
        if (parentIdIsNull.HasValue && parentIdIsNull.Value)
        {
            query = query.Where(dg => dg.ParentId == null); // Filter for root items
        }
        else if (parentId.HasValue)
        {
            query = query.Where(dg => dg.ParentId == parentId.Value); // Filter by specific parent
        }
        // If both are null, no parent filter is applied (gets all levels mixed)

        // Apply include logic if needed
        // if (includeDetails) query = query.Include(...);

        return query;
    }

    // Helper to generate Recursive CTE SQL based on DB Provider (Conceptual)
    private string GetRecursiveCteSqlForIds(DbContext context, Guid parentId)
    {
        // IMPORTANT: Adapt SQL based on your specific database provider (SQL Server, PostgreSQL, Oracle, etc.)
        // Use parameters to avoid SQL injection!
        // Example shows string interpolation for clarity, but **MUST** use DbParameter in real code.
        var tableName = context.Model.FindEntityType(typeof(DataGroup))?.GetTableName() ?? $"{CoreFWConsts.DbTablePrefix}DataGroups";
        var schemaName = context.Model.FindEntityType(typeof(DataGroup))?.GetSchema() ?? CoreFWConsts.DbSchema;
        var qualifiedTableName = string.IsNullOrEmpty(schemaName) ? $"\"{tableName}\"" : $"\"{schemaName}\".\"{tableName}\"";
        var idColumnName = context.Model.FindEntityType(typeof(DataGroup))?.FindProperty(nameof(DataGroup.Id))?.GetColumnName(StoreObjectIdentifier.Table(tableName, schemaName)) ?? "\"Id\"";
        var parentIdColumnName = context.Model.FindEntityType(typeof(DataGroup))?.FindProperty(nameof(DataGroup.ParentId))?.GetColumnName(StoreObjectIdentifier.Table(tableName, schemaName)) ?? "\"ParentId\"";
        // ---- IMPORTANT: Parameterize this value ----
        // var parentIdParameter = new OracleParameter("parentId", parentId); // Example for Oracle
        // Replace '{parentId}' in SQL with the correct parameter placeholder (e.g., :parentId for Oracle)
        // ----

        //if (context.Database.IsSqlServer())
        //{
        //    // Use DbParameter for parentId! Placeholder: @parentId
        //    return $@"
        //            ;WITH DescendantsCTE AS (
        //                SELECT {idColumnName} FROM {qualifiedTableName} WHERE {parentIdColumnName} = @parentId
        //                UNION ALL
        //                SELECT dg.{idColumnName} FROM {qualifiedTableName} dg INNER JOIN DescendantsCTE cte ON dg.{parentIdColumnName} = cte.{idColumnName}
        //            )
        //            SELECT {idColumnName} FROM DescendantsCTE;";
        //}
        //if (context.Database.IsNpgsql()) // PostgreSQL example
        //{
        //    // Use DbParameter for parentId! Placeholder: $1 (or named parameter)
        //    return $@"
        //            WITH RECURSIVE DescendantsCTE AS (
        //                SELECT {idColumnName} FROM {qualifiedTableName} WHERE {parentIdColumnName} = $1
        //                UNION ALL
        //                SELECT dg.{idColumnName} FROM {qualifiedTableName} dg INNER JOIN DescendantsCTE cte ON dg.{parentIdColumnName} = cte.{idColumnName}
        //            )
        //            SELECT {idColumnName} FROM DescendantsCTE;";
        //}
        if (context.Database.IsOracle()) // Oracle example
        {
            // Use DbParameter for parentId! Placeholder: :parentId

            // IMPORTANT: Oracle CTE syntax might differ slightly based on version.This is a common structure.
            // IMPORTANT: Oracle identifiers might not need explicit quoting depending on creation.Adjust "" if needed.
            return $@"
                    WITH DescendantsCTE ({idColumnName}) AS (
                        SELECT {idColumnName} FROM {qualifiedTableName} WHERE {parentIdColumnName} = :parentId
                        UNION ALL
                        SELECT dg.{idColumnName} FROM {qualifiedTableName} dg INNER JOIN DescendantsCTE cte ON dg.{parentIdColumnName} = cte.{idColumnName}
                    )
                    SELECT {idColumnName} FROM DescendantsCTE";
}
        // Add other database providers if needed

        // Fallback or default logic if needed, or throw exception.
        // This example throws, forcing explicit provider support.
        throw new NotSupportedException($"Recursive CTE SQL generation not implemented for the current database provider: {context.Database.ProviderName}. You must add specific SQL for your provider in {nameof(GetRecursiveCteSqlForIds)}.");
    }
}