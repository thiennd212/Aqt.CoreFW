using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataGroups.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace chứa Lookup DTOs
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.DataGroups;

public interface IDataGroupAppService :
    ICrudAppService< // Kế thừa ICrudAppService cho CRUD cơ bản
        DataGroupDto,                  // DTO hiển thị
        Guid,                          // Kiểu khóa chính
        GetDataGroupsInput,            // DTO lọc/phân trang
        CreateUpdateDataGroupDto>      // DTO tạo/cập nhật
{
    /// <summary>
    /// Lấy danh sách DataGroup dạng phẳng (flat list) cho lookup.
    /// Chỉ trả về các nhóm đang hoạt động.
    /// </summary>
    Task<ListResultDto<DataGroupLookupDto>> GetLookupAsync();

    /// <summary>
    /// Lấy danh sách DataGroup dưới dạng cấu trúc cây.
    /// Có thể thêm tham số để lọc (ví dụ: chỉ lấy cây từ một nút gốc cụ thể).
    /// </summary>
    /// <param name="onlyActive">Chỉ lấy các nút đang hoạt động.</param>
    Task<ListResultDto<DataGroupTreeNodeDto>> GetAsTreeAsync(bool onlyActive = true); // Thêm tham số lọc trạng thái

    /// <summary>
    /// Xuất danh sách DataGroup (dạng phẳng) ra file Excel dựa trên bộ lọc.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetDataGroupsInput input);
}
