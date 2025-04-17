using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Cần cho OrderBy và WhereIf
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.JobTitles;               // Interface IJobTitleRepository
using Aqt.CoreFW.Domain.JobTitles.Entities;      // Entity JobTitle
                                                 // using Aqt.CoreFW.Domain.Employees.Entities;    // Sẽ cần khi triển khai HasEmployeesAsync
using Aqt.CoreFW.EntityFrameworkCore;            // DbContext
using Microsoft.EntityFrameworkCore;             // Các phương thức của EF Core
using Volo.Abp.Domain.Repositories.EntityFrameworkCore; // EfCoreRepository
using Volo.Abp.EntityFrameworkCore;             // IDbContextProvider, GetDbContextAsync, GetDbSetAsync

// ***** NAMESPACE ĐÃ ĐƯỢC CẬP NHẬT *****
namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.JobTitles;

/// <summary>
/// Implements the custom repository for the JobTitle entity using Entity Framework Core.
/// </summary>
public class JobTitleRepository :
    EfCoreRepository<CoreFWDbContext, JobTitle, Guid>, // Kế thừa repository cơ sở của ABP
    IJobTitleRepository                                // Triển khai interface tùy chỉnh
{
    public JobTitleRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider) // Truyền DbContextProvider cho lớp cha
    {
    }

    // --- Các phương thức triển khai giữ nguyên như trước ---

    /// <summary>
    /// Finds a job title by its unique code.
    /// </summary>
    public async Task<JobTitle?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(jt => jt.Code == code, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Checks if a job title code already exists, optionally excluding a specific ID.
    /// </summary>
    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(jt => jt.Code == code && (!excludedId.HasValue || jt.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Retrieves a list of job titles based on filtering, sorting, and pagination.
    /// </summary>
    public async Task<List<JobTitle>> GetListAsync(
        string? filterText = null,
        bool? isActive = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                     jt => jt.Code.Contains(filterText!) || jt.Name.Contains(filterText!))
            .WhereIf(isActive.HasValue, jt => jt.IsActive == isActive!.Value);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? (nameof(JobTitle.Name) + " ASC") : sorting!);

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Gets the total count of job titles based on the filtering parameters.
    /// </summary>
    public async Task<long> GetCountAsync(
        string? filterText = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                     jt => jt.Code.Contains(filterText!) || jt.Name.Contains(filterText!))
            .WhereIf(isActive.HasValue, jt => jt.IsActive == isActive!.Value);

        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Checks if a specific job title is currently assigned to any employees.
    /// Placeholder implementation.
    /// </summary>
    public async Task<bool> HasEmployeesAsync(Guid jobTitleId, CancellationToken cancellationToken = default)
    {
        // TODO: Triển khai logic thực sự khi có Entity và DbSet<Employee>
        await Task.CompletedTask; // Placeholder
        return false; // Tạm thời trả về false
    }
}