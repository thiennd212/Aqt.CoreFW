using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.BDocuments; // IBDocumentRepository
using Aqt.CoreFW.Domain.BDocuments.Entities; // Entities namespace
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext
using JetBrains.Annotations; // For [NotNull] attribute
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.BDocuments;

public class BDocumentRepository :
    EfCoreRepository<CoreFWDbContext, BDocument, Guid>,
    IBDocumentRepository
{
    public BDocumentRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Overriding GetQueryableAsync to apply default filters or includes if necessary
    // public override async Task<IQueryable<BDocument>> GetQueryableAsync()
    // {
    //     return (await base.GetQueryableAsync()).Where(/* Default filters if needed */);
    // }

    // Helper to optionally include details
    protected virtual async Task<IQueryable<BDocument>> GetQueryableWithDetailsAsync(bool includeDetails = false)
    {
        var query = (await GetDbSetAsync()).AsQueryable();
        if (includeDetails)
        {
            query = query.Include(d => d.DocumentData);
            // Optionally include Procedure and Status if needed for specific operations,
            // but be mindful of performance impacts. Consider separate methods if frequently needed.
            // query = query.Include(d => d.Procedure);
            // query = query.Include(d => d.TrangThaiHoSo);
        }
        return query;
    }

    public async Task<BDocument?> FindByMaHoSoAsync(
        [NotNull] string maHoSo,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(maHoSo, nameof(maHoSo));

        var query = await GetQueryableWithDetailsAsync(includeDetails);
        return await query.AsNoTracking() // Use AsNoTracking for read-only operations
                          .FirstOrDefaultAsync(p => p.MaHoSo == maHoSo, GetCancellationToken(cancellationToken));
    }

    public async Task<bool> MaHoSoExistsAsync(
        [NotNull] string maHoSo,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(maHoSo, nameof(maHoSo));

        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking().Where(p => p.MaHoSo == maHoSo);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<BDocument>> GetListAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? trangThaiHoSoId = null,
        DateTime? ngayNopFrom = null,
        DateTime? ngayNopTo = null,
        bool? dangKyNhanQuaBuuDien = null, // Thêm filter nếu có trong interface
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false, // Controlled inclusion of details
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, procedureId, trangThaiHoSoId, ngayNopFrom, ngayNopTo, dangKyNhanQuaBuuDien, includeDetails);

        // Apply sorting. Default sort if not specified.
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(BDocument.CreationTime)} desc" // Default sort by CreationTime descending
            : sorting);

        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? trangThaiHoSoId = null,
        DateTime? ngayNopFrom = null,
        DateTime? ngayNopTo = null,
        bool? dangKyNhanQuaBuuDien = null, // Thêm filter nếu có trong interface
        CancellationToken cancellationToken = default)
    {
        // Details are not needed for count
        var query = await GetListQueryInternalAsync(filterText, procedureId, trangThaiHoSoId, ngayNopFrom, ngayNopTo, dangKyNhanQuaBuuDien, includeDetails: false);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<BDocument?> GetWithDataAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        // Explicitly include DocumentData
        var query = await GetQueryableWithDetailsAsync(includeDetails: true);
        // Use tracking here as the entity might be updated after retrieval
        return await query.FirstOrDefaultAsync(d => d.Id == id, GetCancellationToken(cancellationToken));
    }


    // --- Private Helper Method for Query Construction ---

    private async Task<IQueryable<BDocument>> GetListQueryInternalAsync(
         string? filterText = null,
         Guid? procedureId = null,
         Guid? trangThaiHoSoId = null,
         DateTime? ngayNopFrom = null,
         DateTime? ngayNopTo = null,
         bool? dangKyNhanQuaBuuDien = null, // Thêm filter nếu có
         bool includeDetails = false)
    {
        var query = await GetQueryableWithDetailsAsync(includeDetails);

        // Apply filters using WhereIf for conditional filtering
        query = query.AsNoTracking() // Use AsNoTracking for list queries
             .WhereIf(!filterText.IsNullOrWhiteSpace(), p =>
                 (p.MaHoSo != null && p.MaHoSo.Contains(filterText!)) || // Search MaHoSo
                 (p.TenChuHoSo != null && p.TenChuHoSo.Contains(filterText!)) || // Search TenChuHoSo
                 (p.SoDinhDanhChuHoSo != null && p.SoDinhDanhChuHoSo.Contains(filterText!)) || // Search SoDinhDanh (nếu cần)
                 (p.EmailChuHoSo != null && p.EmailChuHoSo.Contains(filterText!)) || // Search Email (nếu cần)
                 (p.SoDienThoaiChuHoSo != null && p.SoDienThoaiChuHoSo.Contains(filterText!))) // Search Phone (nếu cần)
             .WhereIf(procedureId.HasValue, p => p.ProcedureId == procedureId!.Value) // Filter by ProcedureId
             .WhereIf(trangThaiHoSoId.HasValue, p => p.TrangThaiHoSoId == trangThaiHoSoId!.Value) // Filter by StatusId
             .WhereIf(ngayNopFrom.HasValue, p => p.NgayNop.HasValue && p.NgayNop.Value.Date >= ngayNopFrom.Value.Date) // Filter by NgayNop Start Date
             .WhereIf(ngayNopTo.HasValue, p => p.NgayNop.HasValue && p.NgayNop.Value.Date <= ngayNopTo.Value.Date) // Filter by NgayNop End Date
             .WhereIf(dangKyNhanQuaBuuDien.HasValue, p => p.DangKyNhanQuaBuuDien == dangKyNhanQuaBuuDien.Value); // Filter by DangKyNhanQuaBuuDien

        return query;
    }

    // Optional: Implement GetListWithNavigationPropertiesAsync if complex includes are needed
    // public async Task<List<BDocument>> GetListWithNavigationPropertiesAsync(...) { ... }

    // Optional: Implement GetWithNavigationPropertiesAsync if complex includes are needed
    // public async Task<BDocument> GetWithNavigationPropertiesAsync(...) { ... }
}