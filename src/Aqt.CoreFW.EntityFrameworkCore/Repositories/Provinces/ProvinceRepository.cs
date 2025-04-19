using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Provinces; // Namespace IProvinceRepository
using Aqt.CoreFW.Domain.Provinces.Entities; // Namespace Entity
using Aqt.CoreFW.EntityFrameworkCore; // Namespace DbContext
using Aqt.CoreFW.Domain.Shared.Provinces; // Namespace Enum
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Linq; // Required for WhereIf

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.Provinces;

public class ProvinceRepository :
    EfCoreRepository<CoreFWDbContext, Province, Guid>,
    IProvinceRepository
{
    // IAsyncQueryableExecuter can be injected if needed for complex queries outside standard repo methods
    // private readonly IAsyncQueryableExecuter _asyncExecuter;

    public ProvinceRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
        // _asyncExecuter = asyncExecuter;
    }

    public async Task<Province?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(p => p.Code == code, GetCancellationToken(cancellationToken));
    }

    public async Task<Province?> FindByNameAsync(string name, Guid countryId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(p => p.Name == name && p.CountryId == countryId, GetCancellationToken(cancellationToken));
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(p => p.Code == code && (!excludedId.HasValue || p.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    // Optional: Implement NameExistsInCountryAsync if needed

    public async Task<List<Province>> GetListAsync(
        string? filterText = null,
        ProvinceStatus? status = null,
        Guid? countryId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Consider using IAsyncQueryableExecuter for complex queries if needed
        var query = dbSet
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                p => p.Code.Contains(filterText) || p.Name.Contains(filterText))
            .WhereIf(status.HasValue, p => p.Status == status.Value)
            .WhereIf(countryId.HasValue, p => p.CountryId == countryId.Value);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? $"{nameof(Province.Order)} asc, {nameof(Province.Name)} asc" : sorting);

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        ProvinceStatus? status = null,
        Guid? countryId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                p => p.Code.Contains(filterText) || p.Name.Contains(filterText))
            .WhereIf(status.HasValue, p => p.Status == status.Value)
            .WhereIf(countryId.HasValue, p => p.CountryId == countryId.Value);

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }
}