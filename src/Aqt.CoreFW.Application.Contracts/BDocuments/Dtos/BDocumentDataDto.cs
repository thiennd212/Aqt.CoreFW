using System;
using Aqt.CoreFW.Components; // Enum ComponentType
using EasyAbp.FileManagement.Files.Dtos; // Sử dụng FileInfoDto từ FileManagement

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// Represents the data or file associated with a specific ProcedureComponent
/// within a BDocument context.
/// </summary>
public class BDocumentDataDto
{
    public Guid Id { get; set; } // ID của bản ghi BDocumentData
    public Guid ProcedureComponentId { get; set; }
    public string ComponentCode { get; set; } = string.Empty; // Mã của Component để dễ nhận biết
    public string ComponentName { get; set; } = string.Empty; // Tên của Component để hiển thị
    public ComponentType ComponentType { get; set; }
    public bool IsRequired { get; set; } // Cờ bắt buộc (cần lấy từ ProcedureComponentLink)

    // Dữ liệu cho loại Form (JSON Tờ khai)
    public string? FormData { get; set; }

    // Thông tin cho loại File (Lấy từ FileManagement)
    public Guid? FileId { get; set; }
    public FileInfoDto? FileInfo { get; set; } // Sử dụng FileInfoDto từ EasyAbp
} 