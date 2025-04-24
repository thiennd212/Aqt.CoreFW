using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataCores.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace chứa DataGroupLookupDto và DataCoreLookupDto
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.DataCores;

public interface IDataCoreAppService :
    ICrudAppService< // Kế thừa ICrudAppService cho CRUD cơ bản
        DataCoreDto,                  // DTO hiển thị
        Guid,                         // Kiểu khóa chính
        GetDataCoresInput,            // DTO lọc/phân trang
        CreateUpdateDataCoreDto>      // DTO tạo/cập nhật
{
    /// <summary>
    /// Lấy danh sách DataCore dạng phẳng (flat list) cho lookup theo DataGroupId.
    /// Chỉ trả về các DataCore đang hoạt động.
    /// </summary>
    /// <param name="dataGroupId">ID của DataGroup cần lấy DataCore.</param>
    Task<ListResultDto<DataCoreLookupDto>> GetLookupByDataGroupAsync(Guid dataGroupId);

    /// <summary>
    /// (Optional) Xuất danh sách DataCore (dạng phẳng) ra file Excel dựa trên bộ lọc.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetDataCoresInput input);
} 