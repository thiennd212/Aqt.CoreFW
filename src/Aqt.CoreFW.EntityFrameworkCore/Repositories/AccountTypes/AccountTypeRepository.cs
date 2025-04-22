using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Cần cho sắp xếp động
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.AccountTypes; // Namespace chứa IAccountTypeRepository
using Aqt.CoreFW.Domain.AccountTypes.Entities; // Namespace chứa AccountType Entity
using Aqt.CoreFW.EntityFrameworkCore; // Namespace chứa CoreFWDbContext và EfCoreRepository
using Aqt.CoreFW.AccountTypes; // Namespace chứa AccountTypeStatus enum
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore; // Cần cho IDbContextProvider

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.AccountTypes; // Namespace Repository implementation

public class AccountTypeRepository :
    EfCoreRepository<CoreFWDbContext, AccountType, Guid>, // Kế thừa từ base repository của ABP
    IAccountTypeRepository // Triển khai interface cụ thể
{
    public AccountTypeRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Tìm AccountType theo Code (Code là unique)
    public async Task<AccountType?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Sử dụng AsNoTracking vì đây là truy vấn chỉ đọc
        return await dbSet.AsNoTracking()
                          .FirstOrDefaultAsync(at => at.Code == code, GetCancellationToken(cancellationToken));
    }

    // Kiểm tra sự tồn tại của Code (cho việc validate)
    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        // Kiểm tra có bản ghi nào trùng Code không, loại trừ ID hiện tại nếu đang cập nhật (excludedId)
        return await dbSet.AnyAsync(at => at.Code == code && (!excludedId.HasValue || at.Id != excludedId.Value), GetCancellationToken(cancellationToken));
    }

    // Lấy danh sách AccountType có phân trang, lọc và sắp xếp
    public async Task<List<AccountType>> GetListAsync(
        string? filterText = null,
        AccountTypeStatus? status = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(filterText, status);

        // Áp dụng sắp xếp, mặc định theo Order rồi đến Name
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(AccountType.Order)} asc, {nameof(AccountType.Name)} asc" // Sắp xếp mặc định
            : sorting); // Sắp xếp theo tham số truyền vào

        // Phân trang và lấy kết quả
        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    // Đếm số lượng AccountType thỏa mãn điều kiện lọc
    public async Task<long> GetCountAsync(
        string? filterText = null,
        AccountTypeStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(filterText, status);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    // --- Helper Methods ---

    // Phương thức private để xây dựng câu truy vấn cơ sở (lọc)
    private async Task<IQueryable<AccountType>> GetListQueryAsync(
        string? filterText = null,
        AccountTypeStatus? status = null)
    {
        var dbSet = await GetDbSetAsync();
        // Bắt đầu với AsNoTracking() cho hiệu năng
        var query = dbSet.AsNoTracking()
            // Lọc theo filterText (Code hoặc Name) nếu có
            .WhereIf(!filterText.IsNullOrWhiteSpace(),
                     at => (at.Code != null && at.Code.Contains(filterText)) || (at.Name != null && at.Name.Contains(filterText))) // Thêm kiểm tra null cho Contains
                                                                                                                                   // Lọc theo Status nếu có
            .WhereIf(status.HasValue, at => at.Status == status.Value);

        // Không cần Include cho AccountType vì không có quan hệ phức tạp cần load
        return query;
    }
}