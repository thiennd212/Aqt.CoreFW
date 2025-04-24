using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataImportants.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace chứa DataGroupLookupDto và DataImportantLookupDto
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.DataImportants;

public interface IDataImportantAppService :
    ICrudAppService< // Kế thừa ICrudAppService cho CRUD cơ bản
        DataImportantDto,               // DTO hiển thị
        Guid,                           // Kiểu khóa chính
        GetDataImportantsInput,         // DTO lọc/phân trang
        CreateUpdateDataImportantDto>   // DTO tạo/cập nhật
{
    /// <summary>
    /// Lấy danh sách DataImportant dạng phẳng (flat list) cho lookup theo DataGroupId.
    /// Chỉ trả về các DataImportant đang hoạt động.
    /// </summary>
    /// <param name="dataGroupId">ID của DataGroup cần lấy DataImportant.</param>
    Task<ListResultDto<DataImportantLookupDto>> GetLookupByDataGroupAsync(Guid dataGroupId);

    /// <summary>
    /// Lấy danh sách DataGroup dạng lookup (đã có trong IDataGroupAppService).
    /// Cần đảm bảo interface này có thể truy cập được.
    /// </summary>
    // Task<ListResultDto<DataGroupLookupDto>> GetDataGroupLookupAsync(); // Gọi qua IDataGroupAppService

    /// <summary>
    /// (Optional) Xuất danh sách DataImportant (dạng phẳng) ra file Excel dựa trên bộ lọc.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetDataImportantsInput input);
} 