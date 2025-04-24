using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.DataImportants; // IDataImportantRepository
using Aqt.CoreFW.Domain.DataImportants.Entities; // DataImportant Entity
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext, EfCoreRepository
using Aqt.CoreFW.DataImportants; // DataImportantStatus enum
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.DataImportants;

public class DataImportantRepository :
    EfCoreRepository<CoreFWDbContext, DataImportant, Guid>,
    IDataImportantRepository
{
    public DataImportantRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Implementation requires DataGroupId because Code is unique per group
    public async Task<DataImportant?> FindByCodeAsync(
        [NotNull] string code,
        Guid dataGroupId,
        bool includeDetails = false, // Typically false if no navigation props needed
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking(); // Use AsNoTracking for read-only queries

        // If includeDetails is true and navigation properties exist, add .Include() here
        // query = includeDetails ? query.Include(di => di.DataGroup) : query;

        return await query.FirstOrDefaultAsync(
            di => di.DataGroupId == dataGroupId && di.Code == code,
            GetCancellationToken(cancellationToken));
    }

    // Implementation requires DataGroupId
    public async Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid dataGroupId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking()
                         .Where(di => di.DataGroupId == dataGroupId && di.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(di => di.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataImportant>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataImportantStatus? status = null,
        Guid? dataGroupId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, code, name, status, dataGroupId);

        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(DataImportant.Order)} asc, {nameof(DataImportant.Name)} asc" // Default sort
            : sorting);

        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataImportantStatus? status = null,
        Guid? dataGroupId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, code, name, status, dataGroupId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataImportant>> GetListByDataGroupIdAsync(
        Guid dataGroupId,
        bool onlyActive = true,
        string? sorting = null, // Default handled below
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking().Where(di => di.DataGroupId == dataGroupId);

        if (onlyActive)
        {
            query = query.Where(di => di.Status == DataImportantStatus.Active);
        }

        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
             $"{nameof(DataImportant.Order)} asc, {nameof(DataImportant.Name)} asc" // Sensible default for lookups
             : sorting);

        // No pagination for typical lookup lists
        return await query.ToListAsync(GetCancellationToken(cancellationToken));
    }

    // --- Private Helper Method ---

    private async Task<IQueryable<DataImportant>> GetListQueryInternalAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        DataImportantStatus? status = null,
        Guid? dataGroupId = null)
    {
        var dbSet = await GetDbSetAsync();
        return dbSet.AsNoTracking()
            // Combined filter for Code or Name
            .WhereIf(!filterText.IsNullOrWhiteSpace(), di =>
                (di.Code != null && di.Code.Contains(filterText!)) ||
                (di.Name != null && di.Name.Contains(filterText!)))
            // Specific filters
            .WhereIf(!code.IsNullOrWhiteSpace(), di => di.Code == code)
            .WhereIf(!name.IsNullOrWhiteSpace(), di => di.Name != null && di.Name.Contains(name))
            .WhereIf(status.HasValue, di => di.Status == status!.Value)
            .WhereIf(dataGroupId.HasValue, di => di.DataGroupId == dataGroupId!.Value);
    }
} 