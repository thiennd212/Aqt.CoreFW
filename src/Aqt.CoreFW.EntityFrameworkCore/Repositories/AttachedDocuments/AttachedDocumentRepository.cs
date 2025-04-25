using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.AttachedDocuments; // IAttachedDocumentRepository
using Aqt.CoreFW.Domain.AttachedDocuments.Entities; // AttachedDocument Entity
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext, namespace chứa EfCoreRepository
using Aqt.CoreFW.AttachedDocuments; // AttachedDocumentStatus enum
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.AttachedDocuments;

public class AttachedDocumentRepository :
    EfCoreRepository<CoreFWDbContext, AttachedDocument, Guid>, // Kế thừa từ EfCoreRepository
    IAttachedDocumentRepository // Implement interface từ Domain
{
    public AttachedDocumentRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Implementation requires ProcedureId because Code is unique per procedure
    public async Task<AttachedDocument?> FindByCodeAsync(
        [NotNull] string code,
        Guid procedureId,
        bool includeDetails = false, // Typically false if no navigation props needed
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Sử dụng AsNoTracking() cho các truy vấn chỉ đọc để tăng hiệu năng
        var query = dbSet.AsNoTracking();

        // Nếu includeDetails là true và có navigation property, thêm .Include() ở đây
        // query = includeDetails ? query.Include(ad => ad.Procedure) : query; // Giả định có navigation property Procedure

        return await query.FirstOrDefaultAsync(
            ad => ad.ProcedureId == procedureId && ad.Code == code,
            GetCancellationToken(cancellationToken)); // Sử dụng GetCancellationToken để tôn trọng CancellationToken đầu vào
    }

    // Implementation requires ProcedureId
    public async Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid procedureId,
        Guid? excludeId = null, // Dùng khi cập nhật để loại trừ chính entity đang sửa
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking()
                         .Where(ad => ad.ProcedureId == procedureId && ad.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(ad => ad.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<AttachedDocument>> GetListAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        AttachedDocumentStatus? status = null,
        Guid? procedureId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, code, name, status, procedureId);

        // Áp dụng sắp xếp, nếu sorting null hoặc rỗng thì sắp xếp mặc định theo Order, Name
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(AttachedDocument.Order)} asc, {nameof(AttachedDocument.Name)} asc"
            : sorting);

        // Áp dụng phân trang
        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        AttachedDocumentStatus? status = null,
        Guid? procedureId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, code, name, status, procedureId);
        // Đếm số lượng bản ghi khớp với điều kiện lọc
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<AttachedDocument>> GetListByProcedureIdAsync(
        Guid procedureId,
        bool onlyActive = true, // Mặc định chỉ lấy Active cho lookup
        string? sorting = null, // Mặc định sẽ được xử lý bên dưới
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking().Where(ad => ad.ProcedureId == procedureId);

        if (onlyActive)
        {
            query = query.Where(ad => ad.Status == AttachedDocumentStatus.Active);
        }

        // Sắp xếp mặc định hợp lý cho lookup
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
             $"{nameof(AttachedDocument.Order)} asc, {nameof(AttachedDocument.Name)} asc"
             : sorting);

        // Thường không phân trang cho danh sách lookup
        return await query.ToListAsync(GetCancellationToken(cancellationToken));
    }

    // --- Phương thức nội bộ để xây dựng câu truy vấn lọc ---
    private async Task<IQueryable<AttachedDocument>> GetListQueryInternalAsync(
        string? filterText = null,
        string? code = null,
        string? name = null,
        AttachedDocumentStatus? status = null,
        Guid? procedureId = null)
    {
        var dbSet = await GetDbSetAsync();
        return dbSet.AsNoTracking()
            // Lọc kết hợp theo Code HOẶC Name nếu filterText có giá trị
            .WhereIf(!filterText.IsNullOrWhiteSpace(), ad =>
                (ad.Code != null && ad.Code.Contains(filterText!)) ||
                (ad.Name != null && ad.Name.Contains(filterText!)))
            // Lọc chính xác theo các trường cụ thể nếu có giá trị
            .WhereIf(!code.IsNullOrWhiteSpace(), ad => ad.Code == code)
            .WhereIf(!name.IsNullOrWhiteSpace(), ad => ad.Name != null && ad.Name.Contains(name))
            .WhereIf(status.HasValue, ad => ad.Status == status!.Value)
            .WhereIf(procedureId.HasValue, ad => ad.ProcedureId == procedureId!.Value);
    }
}