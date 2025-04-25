using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Cần cho sắp xếp động bằng chuỗi
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Procedures; // IProcedureRepository
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext
using Aqt.CoreFW.Procedures; // ProcedureStatus enum
using JetBrains.Annotations; // Cho [NotNull]
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore; // Base class EfCoreRepository
using Volo.Abp.EntityFrameworkCore; // IDbContextProvider

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.Procedures;

public class ProcedureRepository :
    EfCoreRepository<CoreFWDbContext, Procedure, Guid>, // Kế thừa từ base repository của ABP
    IProcedureRepository // Implement interface tùy chỉnh
{
    public ProcedureRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Implement FindByCodeAsync từ IProcedureRepository
    public async Task<Procedure?> FindByCodeAsync(
        [NotNull] string code,
        bool includeDetails = false, // Mặc định là false vì Procedure không có navigation properties phức tạp
        CancellationToken cancellationToken = default)
    {
        // Lấy DbSet từ DbContext được quản lý bởi Abp
        var dbSet = await GetDbSetAsync();
        // Sử dụng AsNoTracking cho các truy vấn chỉ đọc để tối ưu hiệu năng
        var query = dbSet.AsNoTracking();

        // Nếu includeDetails là true và có các navigation properties cần include:
        // if (includeDetails)
        // {
        //     query = query.Include(p => p.SomeRelatedEntity);
        // }

        // Tìm Procedure đầu tiên khớp Code
        return await query.FirstOrDefaultAsync(
            p => p.Code == code,
            GetCancellationToken(cancellationToken)); // Sử dụng helper để lấy CancellationToken hiệu quả
    }

    // Implement CodeExistsAsync từ IProcedureRepository
    public async Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludeId = null, // Dùng khi cập nhật để loại trừ chính bản ghi đang sửa
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking()
                         .Where(p => p.Code == code); // Lọc theo Code

        // Nếu có excludeId, loại trừ bản ghi có Id đó ra khỏi kiểm tra
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        // Kiểm tra xem có bất kỳ bản ghi nào khớp điều kiện không
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    // Implement GetListAsync từ IProcedureRepository (phiên bản có filter/sort/paging)
    public async Task<List<Procedure>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        ProcedureStatus? status = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        // Tạo câu query cơ sở với các filter
        var query = await GetListQueryInternalAsync(filterText, code, name, status);

        // Áp dụng sắp xếp: nếu sorting là null hoặc trắng, dùng sắp xếp mặc định
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(Procedure.Order)} asc, {nameof(Procedure.Name)} asc" // Sắp xếp mặc định theo Order, Name
            : sorting); // Ngược lại, dùng chuỗi sorting được truyền vào

        // Áp dụng phân trang và thực thi truy vấn
        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    // Implement GetCountAsync từ IProcedureRepository
    public async Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        ProcedureStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        // Tạo câu query cơ sở với các filter giống GetListAsync
        var query = await GetListQueryInternalAsync(filterText, code, name, status);
        // Đếm số lượng bản ghi khớp
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    // Implement GetLookupAsync từ IProcedureRepository
    public async Task<List<Procedure>> GetLookupAsync(
        bool onlyActive = true, // Mặc định chỉ lấy bản ghi Active cho lookup
        string? sorting = null, // Sắp xếp tùy chọn, mặc định là Order, Name
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking();

        // Lọc theo trạng thái Active nếu được yêu cầu
        if (onlyActive)
        {
            query = query.Where(p => p.Status == ProcedureStatus.Active);
        }

        // Áp dụng sắp xếp
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(Procedure.Order)} asc, {nameof(Procedure.Name)} asc" // Sắp xếp mặc định
            : sorting!); // Dùng dấu ! vì đã kiểm tra IsNullOrWhiteSpace

        // Lookup thường không cần phân trang, lấy tất cả kết quả khớp
        return await query.ToListAsync(GetCancellationToken(cancellationToken));
    }


    // --- Private Helper Method ---

    /// <summary>
    /// Xây dựng IQueryable cơ sở với các điều kiện lọc.
    /// </summary>
    private async Task<IQueryable<Procedure>> GetListQueryInternalAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        ProcedureStatus? status = null)
    {
        var dbSet = await GetDbSetAsync();
        return dbSet.AsNoTracking() // Luôn dùng AsNoTracking cho query lấy danh sách
                                    // Filter tổng hợp: tìm trong Code hoặc Name nếu filterText có giá trị
            .WhereIf(!filterText.IsNullOrWhiteSpace(), p =>
                (p.Code != null && p.Code.Contains(filterText!)) || // Tìm trong Code
                (p.Name != null && p.Name.Contains(filterText!)))   // Tìm trong Name
                                                                    // Filter chính xác theo Code nếu code có giá trị
            .WhereIf(!code.IsNullOrWhiteSpace(), p => p.Code == code)
            // Filter chứa trong Name nếu name có giá trị
            .WhereIf(!name.IsNullOrWhiteSpace(), p => p.Name != null && p.Name.Contains(name!))
            // Filter theo Status nếu status có giá trị
            .WhereIf(status.HasValue, p => p.Status == status!.Value);
    }
}