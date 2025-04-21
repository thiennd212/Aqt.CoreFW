using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Districts.Entities;
using Aqt.CoreFW.Domain.Districts;
using Aqt.CoreFW.Domain.Provinces.Entities; // Needed for ProvinceExistsAsync
using Aqt.CoreFW.Domain.Shared.Districts;
using Aqt.CoreFW.EntityFrameworkCore; // DbContext namespace
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.Districts;

public class DistrictRepository :
    EfCoreRepository<CoreFWDbContext, District, Guid>,
    IDistrictRepository // Implement the updated interface
{
    public DistrictRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Implementation for FindByCodeAsync (as per updated interface)
    public async Task<District?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AsNoTracking()
                          .FirstOrDefaultAsync(d => d.Code == code, GetCancellationToken(cancellationToken));
    }

    // Implementation for FindByNameAsync (NEW)
    public async Task<District?> FindByNameAsync(string name, Guid provinceId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AsNoTracking()
                          .FirstOrDefaultAsync(d => d.Name == name && d.ProvinceId == provinceId, GetCancellationToken(cancellationToken));
    }

    // Implementation for CodeExistsAsync (NEW)
    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(d => d.Code == code && (!excludedId.HasValue || d.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    // Implementation for NameExistsInProvinceAsync (NEW)
    public async Task<bool> NameExistsInProvinceAsync(string name, Guid provinceId, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(d => d.Name == name && d.ProvinceId == provinceId && (!excludedId.HasValue || d.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    // Implementation for GetListAsync (parameter includeDetails remains but might not be used for joining here)
    public async Task<List<District>> GetListAsync(
        string? filterText = null,
        DistrictStatus? status = null,
        Guid? provinceId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false, // Parameter kept, but join logic is not implemented here by default
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(filterText, status, provinceId, includeDetails);

        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ? $"{nameof(District.ProvinceId)} asc, {nameof(District.Order)} asc, {nameof(District.Name)} asc" : sorting);

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    // Implementation for GetCountAsync (parameters updated)
    public async Task<long> GetCountAsync(
        string? filterText = null,
        DistrictStatus? status = null,
        Guid? provinceId = null,
        CancellationToken cancellationToken = default)
    {
        // Pass includeDetails: false as details are not needed for count
        var query = await GetListQueryAsync(filterText, status, provinceId, includeDetails: false);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

     // Implementation for GetListByProvinceIdAsync (NEW)
     public async Task<List<District>> GetListByProvinceIdAsync(Guid provinceId, CancellationToken cancellationToken = default)
     {
         var dbSet = await GetDbSetAsync();
         return await dbSet.AsNoTracking()
                           .Where(d => d.ProvinceId == provinceId)
                           .OrderBy(d => d.Order).ThenBy(d => d.Name) // Consistent sorting
                           .ToListAsync(GetCancellationToken(cancellationToken));
     }

    // Implementation for ProvinceExistsAsync (Remains the same)
    public async Task<bool> ProvinceExistsAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        // Directly query the Provinces DbSet
        return await dbContext.Provinces.AnyAsync(p => p.Id == provinceId, GetCancellationToken(cancellationToken));
    }


    // Helper method to build the base queryable (Remains largely the same)
    private async Task<IQueryable<District>> GetListQueryAsync(
        string? filterText = null,
        DistrictStatus? status = null,
        Guid? provinceId = null,
        bool includeDetails = false) // includeDetails flag is present but not used to Include Province here
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking() // Use AsNoTracking for read queries
            .WhereIf(provinceId.HasValue, d => d.ProvinceId == provinceId)
            .WhereIf(status.HasValue, d => d.Status == status)
            .WhereIf(!filterText.IsNullOrWhiteSpace(),
                // Ensure null checks for Code and Name if they are nullable in the DB model (though not in Entity)
                d => (d.Code != null && d.Code.Contains(filterText)) || (d.Name != null && d.Name.Contains(filterText)));

        // Explicitly not including Province details here based on includeDetails flag.
        // AppService handles fetching Province names separately if needed.

        return query;
    }
}