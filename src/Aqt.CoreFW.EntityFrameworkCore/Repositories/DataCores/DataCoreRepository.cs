using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.DataCores; // IDataCoreRepository
using Aqt.CoreFW.Domain.DataCores.Entities; // DataCore Entity
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext, EfCoreRepository
using Aqt.CoreFW.DataCores; // DataCoreStatus enum
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.DataCores;

public class DataCoreRepository :
    EfCoreRepository<CoreFWDbContext, DataCore, Guid>,
    IDataCoreRepository
{
    public DataCoreRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Finds a DataCore by its unique code (assuming global uniqueness)
    public async Task<DataCore?> FindByCodeAsync(string code, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // AsNoTracking is good practice for read-only queries
        return await dbSet.AsNoTracking()
                          .FirstOrDefaultAsync(dc => dc.Code == code, GetCancellationToken(cancellationToken));
    }

    // Checks if a DataCore with the given code already exists (assuming global uniqueness)
    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking();

        // Assuming global uniqueness
        query = query.Where(dc => dc.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(dc => dc.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    // Gets a list of DataCores based on filtering criteria
    public async Task<List<DataCore>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataCoreStatus? status = null,
        Guid? dataGroupId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, code, name, status, dataGroupId);

        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(DataCore.Order)} asc, {nameof(DataCore.Name)} asc" // Default sort
            : sorting);

        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    // Gets the total count of DataCores based on filtering criteria
    public async Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataCoreStatus? status = null,
        Guid? dataGroupId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, code, name, status, dataGroupId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    // Gets a list of DataCores belonging to a specific DataGroup (for lookups)
    public async Task<List<DataCore>> GetListByDataGroupIdAsync(
        Guid dataGroupId,
        bool onlyActive = true,
        string? sorting = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking().Where(dc => dc.DataGroupId == dataGroupId);

        if (onlyActive)
        {
            query = query.Where(dc => dc.Status == DataCoreStatus.Active);
        }

        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
             $"{nameof(DataCore.Order)} asc, {nameof(DataCore.Name)} asc" // Default sort for lookups
             : sorting);

        return await query.ToListAsync(GetCancellationToken(cancellationToken));
    }

    // --- Private Helper Methods ---

    // Helper method to build the common query for GetListAsync and GetCountAsync
    private async Task<IQueryable<DataCore>> GetListQueryInternalAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataCoreStatus? status = null,
        Guid? dataGroupId = null)
    {
        var dbSet = await GetDbSetAsync();
        return dbSet.AsNoTracking()
            .WhereIf(!filterText.IsNullOrWhiteSpace(), dc =>
                dc.Code.Contains(filterText!) || // Simplified filter
                dc.Name.Contains(filterText!))  // Simplified filter
            .WhereIf(!code.IsNullOrWhiteSpace(), dc => dc.Code == code)
            .WhereIf(!name.IsNullOrWhiteSpace(), dc => dc.Name.Contains(name!))
            .WhereIf(status.HasValue, dc => dc.Status == status!.Value)
            .WhereIf(dataGroupId.HasValue, dc => dc.DataGroupId == dataGroupId!.Value);
    }
} 