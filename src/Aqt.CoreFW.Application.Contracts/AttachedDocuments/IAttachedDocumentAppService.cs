using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace chứa ProcedureLookupDto và AttachedDocumentLookupDto
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.AttachedDocuments;

public interface IAttachedDocumentAppService :
    ICrudAppService< // Kế thừa ICrudAppService cho CRUD cơ bản
        AttachedDocumentDto,               // DTO hiển thị
        Guid,                              // Kiểu khóa chính
        GetAttachedDocumentsInput,         // DTO lọc/phân trang
        CreateUpdateAttachedDocumentDto>   // DTO tạo/cập nhật
{
    /// <summary>
    /// Lấy danh sách AttachedDocument dạng phẳng (flat list) cho lookup theo ProcedureId.
    /// Chỉ trả về các AttachedDocument đang hoạt động.
    /// </summary>
    /// <param name="procedureId">ID của Procedure cần lấy AttachedDocument.</param>
    Task<ListResultDto<AttachedDocumentLookupDto>> GetLookupByProcedureAsync(Guid procedureId);

    /// <summary>
    /// Lấy danh sách Procedure dạng lookup (Giả định có trong IProcedureAppService).
    /// Cần đảm bảo interface này có thể truy cập được.
    /// </summary>
    // Task<ListResultDto<ProcedureLookupDto>> GetProcedureLookupAsync(); // Gọi qua IProcedureAppService

    /// <summary>
    /// (Optional) Xuất danh sách AttachedDocument (dạng phẳng) ra file Excel dựa trên bộ lọc.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetAttachedDocumentsInput input);
} 