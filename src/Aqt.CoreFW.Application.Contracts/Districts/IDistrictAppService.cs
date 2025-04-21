using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Districts.Dtos;
// Đảm bảo using đúng namespace cho Lookup DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Cần cho IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.Districts;

/// <summary>
/// Application Service interface for managing Districts.
/// Defines CRUD operations and custom methods like lookups and export.
/// </summary>
public interface IDistrictAppService :
    ICrudAppService< // Kế thừa CrudAppService chuẩn của ABP
        DistrictDto,               // DTO để hiển thị Quận/Huyện
        Guid,                      // Kiểu khóa chính
        GetDistrictsInput,         // DTO input cho GetList (lọc, phân trang)
        CreateUpdateDistrictDto>   // DTO input cho Tạo/Sửa
{
    /// <summary>
    /// Lấy danh sách các Quận/Huyện (dạng lookup) đang hoạt động,
    /// có thể lọc theo Tỉnh/Thành phố. Dùng cho dropdown.
    /// </summary>
    /// <param name="provinceId">Optional Id của Tỉnh/Thành phố để lọc.</param>
    /// <returns>Danh sách DistrictLookupDto.</returns>
    Task<ListResultDto<DistrictLookupDto>> GetLookupAsync(Guid? provinceId = null);

    /// <summary>
    /// Lấy danh sách các Tỉnh/Thành phố (dạng lookup).
    /// Dùng cho dropdown lọc trên trang danh sách Quận/Huyện.
    /// </summary>
    /// <returns>Danh sách ProvinceLookupDto.</returns>
    Task<ListResultDto<ProvinceLookupDto>> GetProvinceLookupAsync();

    /// <summary>
    /// Xuất danh sách Quận/Huyện ra file Excel dựa trên bộ lọc đầu vào.
    /// </summary>
    /// <param name="input">Input chứa các tiêu chí lọc giống GetListAsync.</param>
    /// <returns>Nội dung file Excel.</returns>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetDistrictsInput input);
}