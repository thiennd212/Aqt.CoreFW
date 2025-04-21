using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Communes; // ICommuneRepository
using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Domain.Shared.Communes;
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.Communes;

public class CommuneRepository :
    EfCoreRepository<CoreFWDbContext, Commune, Guid>,
    ICommuneRepository // Implement the specific interface
{
    public CommuneRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Commune?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Use AsNoTracking for read-only queries
        return await dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code, GetCancellationToken(cancellationToken));
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(c => c.Code == code && (!excludedId.HasValue || c.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    public async Task<List<Commune>> GetListAsync(
        string? filterText = null,
        CommuneStatus? status = null,
        Guid? provinceId = null,
        Guid? districtId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false, // Not used for Includes here, names are fetched in AppService
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(filterText, status, provinceId, districtId, includeDetails);

        // Default sorting if none provided
        var finalSorting = sorting.IsNullOrWhiteSpace()
            ? $"{nameof(Commune.Order)} asc, {nameof(Commune.Name)} asc" // Default sort
            : sorting;

        // Apply sorting
        query = query.OrderBy(finalSorting);


        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }


    public async Task<long> GetCountAsync(
        string? filterText = null,
        CommuneStatus? status = null,
        Guid? provinceId = null,
        Guid? districtId = null,
        CancellationToken cancellationToken = default)
    {
        // Reuse the query logic without Includes
        var query = await GetListQueryAsync(filterText, status, provinceId, districtId, includeDetails: false);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Commune>> GetListByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AsNoTracking()
                          .Where(c => c.ProvinceId == provinceId)
                          .OrderBy(c => c.Order).ThenBy(c => c.Name) // Consistent sorting
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Commune>> GetListByDistrictIdAsync(Guid districtId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AsNoTracking()
                          .Where(c => c.DistrictId == districtId)
                          .OrderBy(c => c.Order).ThenBy(c => c.Name) // Consistent sorting
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    // Helper method to build the query for GetListAsync and GetCountAsync
    private async Task<IQueryable<Commune>> GetListQueryAsync(
        string? filterText = null,
        CommuneStatus? status = null,
        Guid? provinceId = null,
        Guid? districtId = null,
        bool includeDetails = false) // Parameter kept for signature consistency if needed later
    {
        var dbSet = await GetDbSetAsync();

        // Use AsNoTracking for read-only list queries
        // Apply filtering
        return dbSet.AsNoTracking()
            .WhereIf(!filterText.IsNullOrWhiteSpace(), c =>
                c.Code.Contains(filterText!) || c.Name.Contains(filterText!))
            .WhereIf(status.HasValue, c => c.Status == status!.Value)
            .WhereIf(provinceId.HasValue, c => c.ProvinceId == provinceId!.Value)
            .WhereIf(districtId.HasValue, c => c.DistrictId == districtId!.Value);
        // Note: Includes are not added here as names are handled in AppService
    }
}