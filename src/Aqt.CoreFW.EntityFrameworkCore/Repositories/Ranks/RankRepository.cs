using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Ranks;
using Aqt.CoreFW.Domain.Ranks.Entities;
using Aqt.CoreFW.EntityFrameworkCore; // Namespace chứa DbContext
using Aqt.CoreFW.Ranks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.Ranks
{
    public class RankRepository :
        EfCoreRepository<CoreFWDbContext, Rank, Guid>,
        IRankRepository
    {
        public RankRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<Rank?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.AsNoTracking() // Sử dụng AsNoTracking
                              .FirstOrDefaultAsync(r => r.Code == code, GetCancellationToken(cancellationToken));
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            // Sử dụng AnyAsync trực tiếp
            return await dbSet.AnyAsync(r => r.Code == code && (!excludedId.HasValue || r.Id != excludedId.Value), GetCancellationToken(cancellationToken));
        }

        public async Task<List<Rank>> GetListAsync(
            string? filterText = null,
            RankStatus? status = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetListQueryAsync(filterText, status);

            query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
                $"{nameof(Rank.Order)} asc, {nameof(Rank.Name)} asc" // Sắp xếp mặc định theo kế hoạch
                : sorting);

            return await query.PageBy(skipCount, maxResultCount)
                              .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string? filterText = null,
            RankStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetListQueryAsync(filterText, status);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        // Helper method theo kế hoạch
        private async Task<IQueryable<Rank>> GetListQueryAsync(
            string? filterText = null,
            RankStatus? status = null)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.AsNoTracking() // Bắt đầu với AsNoTracking
                .WhereIf(!filterText.IsNullOrWhiteSpace(),
                         r => r.Code.Contains(filterText) || r.Name.Contains(filterText)) // Sử dụng WhereIf
                .WhereIf(status.HasValue, r => r.Status == status.Value); // Sử dụng WhereIf

            return query;
        }
    }
}