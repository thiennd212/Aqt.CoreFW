using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for OrderBy with string and PageBy
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Countries.Repositories;
using Aqt.CoreFW.Domain.Countries.Entities;
// using Aqt.CoreFW.Domain.Provinces.Entities; // TODO: Uncomment when Province entity exists
using Aqt.CoreFW.EntityFrameworkCore; // Namespace of the DbContext
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Countries;

/// <summary>
/// Entity Framework Core implementation of the <see cref="ICountryRepository"/>.
/// </summary>
public class CountryRepository :
    EfCoreRepository<CoreFWDbContext, Country, Guid>, // Inherit standard EF Core repository
    ICountryRepository // Implement the custom interface
{
    public CountryRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Country?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(c => c.Code == code, GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc />
    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Check if any entity matches the code, optionally excluding the given ID
        return await dbSet.AnyAsync(c => c.Code == code && (!excludedId.HasValue || c.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc />
    public async Task<List<Country>> GetListAsync(
        string? filterText = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet
            .WhereIf(!filterText.IsNullOrWhiteSpace(), // Apply filter if provided
                c => c.Code.Contains(filterText) || c.Name.Contains(filterText));

        // Apply sorting, default to Name ascending if not specified
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Country.Name) + " asc" : sorting);

        // Apply paging
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc />
    public async Task<long> GetCountAsync(
        string? filterText = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet
            .WhereIf(!filterText.IsNullOrWhiteSpace(), // Apply filter if provided
                c => c.Code.Contains(filterText) || c.Name.Contains(filterText));

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method is temporarily commented out as it depends on the Province entity.
    /// Uncomment and ensure DbSet<Province> exists in the DbContext when Province is implemented.
    /// </remarks>
    public Task<bool> HasProvincesAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Province entity and DbSet are available.
        // var dbContext = await GetDbContextAsync();
        // return await dbContext.Provinces.AnyAsync(p => p.CountryId == countryId, GetCancellationToken(cancellationToken));
        return Task.FromResult(false); // Temporarily return false
    }

    /*
    // Override GetQueryableAsync if you need to include related entities by default
    // Not usually necessary for simple entities like Country.
    public override async Task<IQueryable<Country>> GetQueryableAsync()
    {
        return (await base.GetQueryableAsync()); //.Include(...) if needed
    }
    */
} 