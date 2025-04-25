using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Components.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace chứa LookupDto
using Aqt.CoreFW.Components; // Namespace chứa ComponentType Enum
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent (Excel export)

namespace Aqt.CoreFW.Application.Contracts.Components;

public interface IProcedureComponentAppService :
    ICrudAppService< // Kế thừa ICrudAppService cho CRUD cơ bản
        ProcedureComponentDto,          // DTO hiển thị chi tiết
        Guid,                           // Kiểu khóa chính
        GetProcedureComponentsInput,    // DTO lọc/phân trang
        CreateUpdateProcedureComponentDto> // DTO tạo/cập nhật (bao gồm cả ProcedureIds)
{
    /// <summary>
    /// Lấy danh sách ProcedureComponents dạng phẳng (flat list) cho lookup.
    /// Có thể lọc theo loại (Form/File) và chỉ lấy các component đang hoạt động.
    /// </summary>
    /// <param name="type">Lọc theo loại ComponentType (tùy chọn).</param>
    Task<ListResultDto<ProcedureComponentLookupDto>> GetLookupAsync(ComponentType? type = null);

    /// <summary>
    /// (Optional) Xuất danh sách ProcedureComponents ra file Excel dựa trên bộ lọc.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetProcedureComponentsInput input);

    // Ghi chú: Việc cập nhật liên kết Procedure-Component được xử lý thông qua
    // trường ProcedureIds trong CreateUpdateProcedureComponentDto khi gọi phương thức
    // CreateAsync hoặc UpdateAsync (từ ICrudAppService).
    // Không cần phương thức riêng biệt ở đây trừ khi có logic đặc biệt.
} 