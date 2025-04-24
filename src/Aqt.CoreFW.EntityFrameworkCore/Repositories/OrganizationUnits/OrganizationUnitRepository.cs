using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
// Use alias for the custom repository interface to avoid ambiguity
using CustomIOrganizationUnitRepository = Aqt.CoreFW.Domain.OrganizationUnits.IOrganizationUnitRepository;
using Aqt.CoreFW.Domain.OrganizationUnits; // Domain Enum, Consts, Extensions - Keep verifying this namespace
using Aqt.CoreFW.EntityFrameworkCore;     // DbContext
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;                // OrganizationUnit entity, Standard IOrganizationUnitRepository
using Volo.Abp.ObjectExtending;
using Aqt.CoreFW.OrganizationUnits;
using Volo.Abp.Data;          // GetProperty extension method

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.OrganizationUnits;

public class OrganizationUnitRepository :
    EfCoreRepository<CoreFWDbContext, OrganizationUnit, Guid>,
    CustomIOrganizationUnitRepository // Implement the custom interface using the alias
{
    public OrganizationUnitRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Find OU by ManualCode (Extended Property)
    public async Task<OrganizationUnit?> FindByManualCodeAsync(
        string manualCode, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // AsNoTracking since this is likely a read operation; filtering by ManualCode sẽ được thực hiện client-side
        var list = await dbSet.AsNoTracking().ToListAsync(GetCancellationToken(cancellationToken));
        return list.Find(ou => ou.GetProperty<string>(OrganizationUnitExtensionProperties.ManualCode, default) == manualCode);
    }

    // Check if ManualCode exists (Extended Property)
    public async Task<bool> ManualCodeExistsAsync(
        string manualCode, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        // Lọc theo excludedId nếu có, sau đó tải về và kiểm tra client-side
        var queryable = await GetQueryableAsync();
        queryable = queryable.AsNoTracking();
        if (excludedId.HasValue)
        {
            queryable = queryable.Where(ou => ou.Id != excludedId.Value);
        }
        var list = await queryable.ToListAsync(GetCancellationToken(cancellationToken));
        return list.Exists(ou => ou.GetProperty<string>(OrganizationUnitExtensionProperties.ManualCode, default) == manualCode);
    }

    // Get list of OUs with details (Extended Property filtering/sorting)
    // Note: This implementation differs slightly from the plan's GetLookupAsync in AppService
    // It provides more filtering/sorting capabilities if needed directly at repo level
    public async Task<List<OrganizationUnit>> GetListWithDetailsAsync(
        string? filterText = null,
        Guid? parentId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        query = query
            .WhereIf(parentId.HasValue, ou => ou.ParentId == parentId)
            .WhereIf(!filterText.IsNullOrWhiteSpace(),
                     ou => ou.DisplayName.Contains(filterText)); // Lọc ManualCode client-side nếu cần

        // Dynamic sorting: default sort by DisplayName ascending
        var defaultSort = "DisplayName";
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ? defaultSort : sorting);

        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    // Get count based on filters (matches GetListWithDetailsAsync filters)
    public async Task<long> GetCountAsync(
        string? filterText = null,
        Guid? parentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        query = query
            .WhereIf(parentId.HasValue, ou => ou.ParentId == parentId)
            .WhereIf(!filterText.IsNullOrWhiteSpace(),
                     ou => ou.DisplayName.Contains(filterText)); // Lọc ManualCode client-side nếu cần

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    // Get children OUs, potentially recursive
    public async Task<List<OrganizationUnit>> GetChildrenAsync(
        Guid? parentId, bool recursive = false, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var directChildren = await dbSet
            .Where(ou => ou.ParentId == parentId)
            .OrderBy(ou => ou.DisplayName) // Sắp xếp theo DisplayName; Order sẽ xử lý client-side
            .ToListAsync(GetCancellationToken(cancellationToken));

        if (!recursive || !directChildren.Any())
        {
            return directChildren;
        }

        var allChildren = new List<OrganizationUnit>(directChildren);
        foreach (var child in directChildren)
        {
            // Recursively get children and add to the list
            var grandChildren = await GetChildrenAsync(child.Id, true, cancellationToken);
            allChildren.AddRange(grandChildren);
        }
        // Note: The order might not be fully hierarchical in the final flat list with this simple recursive approach.
        // Consider CTEs or client-side structuring if strict hierarchical order in the flat list is needed.
        return allChildren;
    }

    // Get all OUs optimized for tree display (read-only)
    public async Task<List<OrganizationUnit>> GetAllForTreeAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Use AsNoTracking for read-only optimization
        // Sorting by DisplayName; Order sẽ xử lý client-side
        return await dbSet.AsNoTracking()
                      .OrderBy(ou => ou.DisplayName)
                      .ToListAsync(GetCancellationToken(cancellationToken));
        // Note: Selecting specific columns might be more performant if the entity has many columns,
        // but requires mapping back or using a lighter DTO/anonymous type.
        // Returning full entities allows AutoMapper to handle mapping easily in the AppService.
    }
} 