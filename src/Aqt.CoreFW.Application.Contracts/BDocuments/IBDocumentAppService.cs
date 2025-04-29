using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos; // Namespace chứa DTOs
using EasyAbp.FileManagement.Files.Dtos; // Sử dụng FileInfoDto từ FileManagement
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.BDocuments;

public interface IBDocumentAppService : IApplicationService
{
    /// <summary>
    /// Tạo BDocument mới với thông tin chủ hồ sơ, thông tin bổ sung,
    /// dữ liệu component ban đầu (bao gồm JSON tờ khai và FileId).
    /// Trạng thái ban đầu là null.
    /// </summary>
    Task<BDocumentDto> CreateAsync(CreateBDocumentInputDto input);

    /// <summary>
    /// Cập nhật thông tin chính của BDocument (chủ hồ sơ, phạm vi...).
    /// KHÔNG cập nhật component data qua phương thức này.
    /// </summary>
    Task<BDocumentDto> UpdateAsync(Guid id, UpdateBDocumentInputDto input);

    /// <summary>
    /// Lấy thông tin chi tiết của BDocument, bao gồm cả dữ liệu component.
    /// </summary>
    Task<BDocumentDto> GetAsync(Guid id);

    /// <summary>
    /// Lấy danh sách BDocument đã phân trang và lọc.
    /// </summary>
    Task<PagedResultDto<BDocumentListDto>> GetListAsync(GetBDocumentsInput input);

    /// <summary>
    /// Xóa một BDocument (xóa mềm).
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Thêm hoặc cập nhật dữ liệu/file cho một thành phần cụ thể của BDocument.
    /// (Dùng để sửa Tờ khai JSON hoặc upload/thay đổi file đính kèm).
    /// </summary>
    Task<BDocumentDataDto> AddOrUpdateDataAsync(Guid id, AddOrUpdateBDocumentDataInputDto input);

    /// <summary>
    /// Xóa dữ liệu/file của một thành phần khỏi BDocument.
    /// </summary>
    /// <param name="id">ID của BDocument.</param>
    /// <param name="bDocumentDataId">ID của bản ghi BDocumentData cần xóa.</param>
    /// <param name="deleteFile">Chỉ định có xóa file vật lý trong FileManagement hay không.</param>
    Task RemoveDataAsync(Guid id, Guid bDocumentDataId, bool deleteFile = false);

    /// <summary>
    /// Thay đổi trạng thái của BDocument (được gọi bởi hệ thống Workflow).
    /// </summary>
    Task<BDocumentDto> ChangeStatusAsync(Guid id, ChangeBDocumentStatusInputDto input);

    /// <summary>
    /// (Optional) Trigger quá trình tạo file từ dữ liệu Tờ khai đã lưu.
    /// </summary>
    /// <param name="id">ID của BDocument.</param>
    /// <returns>Thông tin file đã tạo (FileInfoDto).</returns>
    Task<FileInfoDto> GenerateDeclarationFileAsync(Guid id);

    /// <summary>
    /// (Optional) Xuất danh sách BDocuments ra file Excel dựa trên bộ lọc.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetBDocumentsInput input);
} 