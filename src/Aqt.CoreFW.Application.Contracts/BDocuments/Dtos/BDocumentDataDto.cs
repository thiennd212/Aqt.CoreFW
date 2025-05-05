using System;
using Aqt.CoreFW.Components; // Enum ComponentType
using EasyAbp.FileManagement.Files.Dtos; // Sử dụng FileInfoDto từ FileManagement
using Volo.Abp.Application.Dtos; // Cần cho AuditedEntityDto

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// Represents the data or file associated with a specific ProcedureComponent
/// within a BDocument context.
/// Đại diện cho dữ liệu hoặc tệp được liên kết với một ProcedureComponent cụ thể
/// trong ngữ cảnh BDocument.
/// </summary>
// Kế thừa AuditedEntityDto nếu cần thông tin audit
public class BDocumentDataDto : AuditedEntityDto<Guid>
{
    // public Guid Id { get; set; } // Kế thừa từ AuditedEntityDto
    public Guid ProcedureComponentId { get; set; }
    public string? ComponentCode { get; set; } // Cho phép null vì enrich sau
    public string? ComponentName { get; set; } // Cho phép null vì enrich sau
    public ComponentType ComponentType { get; set; }
    public bool IsRequired { get; set; } // Lấy từ ProcedureComponentLink

    // Dữ liệu cho loại Form (JSON Tờ khai)
    public string? InputData { get; set; } // Đổi tên từ FormData

    // Thông tin cho loại File (Lấy từ FileManagement)
    public Guid? FileId { get; set; }
    public FileInfoDto? FileInfo { get; set; } // Sử dụng FileInfoDto từ EasyAbp
} 