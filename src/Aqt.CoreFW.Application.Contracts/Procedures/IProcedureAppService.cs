using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace chứa ProcedureLookupDto
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.Procedures;

public interface IProcedureAppService :
    ICrudAppService< // Kế thừa ICrudAppService cho CRUD cơ bản
        ProcedureDto,               // DTO hiển thị
        Guid,                           // Kiểu khóa chính
        GetProceduresInput,         // DTO lọc/phân trang
        CreateUpdateProcedureDto>   // DTO tạo/cập nhật
        // Lưu ý: DTO cập nhật (Update) ở đây giống DTO tạo (Create),
        // nhưng logic xử lý trong AppService sẽ khác (ví dụ: không cho sửa Code)
{
    /// <summary>
    /// Lấy danh sách Procedures dạng phẳng (flat list) cho lookup.
    /// Chỉ trả về các Procedures đang hoạt động.
    /// </summary>
    Task<ListResultDto<ProcedureLookupDto>> GetLookupAsync();

    /// <summary>
    /// (Optional) Xuất danh sách Procedures ra file Excel dựa trên bộ lọc.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetProceduresInput input);

    // Cân nhắc: Nếu cần phương thức cập nhật thông tin đồng bộ qua API,
    // có thể thêm một DTO và phương thức riêng ở đây.
    // Ví dụ: Task UpdateSyncInfoAsync(Guid id, UpdateProcedureSyncInfoDto syncInfo);
} 