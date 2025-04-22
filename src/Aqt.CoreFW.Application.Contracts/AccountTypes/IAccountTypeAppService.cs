using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace chứa Lookup DTO
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent (xuất file)

namespace Aqt.CoreFW.Application.Contracts.AccountTypes; // Namespace chứa Interface

public interface IAccountTypeAppService :
    ICrudAppService< // Kế thừa ICrudAppService cho các thao tác CRUD cơ bản
        AccountTypeDto,           // DTO hiển thị AccountType
        Guid,                     // Kiểu khóa chính
        GetAccountTypesInput,     // DTO cho lọc/phân trang danh sách
        CreateUpdateAccountTypeDto> // DTO cho tạo/cập nhật
{
    /// <summary>
    /// Lấy danh sách các loại tài khoản đang hoạt động, phù hợp cho dropdown lookup.
    /// </summary>
    Task<ListResultDto<AccountTypeLookupDto>> GetLookupAsync(); // Phương thức lookup

    /// <summary>
    /// Xuất danh sách AccountType ra file Excel dựa trên bộ lọc đầu vào.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetAccountTypesInput input); // Phương thức xuất Excel
}