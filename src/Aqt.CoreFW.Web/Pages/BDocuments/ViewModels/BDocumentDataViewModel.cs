using System;
using Aqt.CoreFW.Components; // Namespace chứa enum ComponentType
using Microsoft.AspNetCore.Mvc;

namespace Aqt.CoreFW.Web.Pages.BDocuments.ViewModels;

public class BDocumentDataViewModel
{
    // ID của bản ghi BDocumentData (nếu đã tồn tại)
    [HiddenInput]
    public Guid? Id { get; set; }

    // ID của ProcedureComponent mà dữ liệu này thuộc về
    [HiddenInput]
    public Guid ProcedureComponentId { get; set; }

    // --- Thông tin Component (để hiển thị trên UI) ---
    public string ComponentCode { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public ComponentType ComponentType { get; set; } // FORM hoặc FILE
    public bool IsRequired { get; set; } // Lấy từ ProcedureComponentLink
    public string? Description { get; set; } // Mô tả của component
    public string? TempPath { get; set; } // Đường dẫn file mẫu (nếu có)

    // --- Dữ liệu cho Form động ---
    /// <summary>
    /// Chuỗi JSON định nghĩa cấu trúc của form (cho component loại FORM).
    /// Được nạp từ ProcedureComponentDto.
    /// </summary>
    public string? FormDefinition { get; set; }

    /// <summary>
    /// Chuỗi JSON chứa dữ liệu người dùng nhập cho form (cho component loại FORM).
    /// Được bind từ hidden input (do JS cập nhật) khi submit.
    /// Được nạp từ BDocumentDataDto khi xem/sửa.
    /// </summary>
    [HiddenInput] // Bind giá trị này từ hidden input trên form
    public string? FormData { get; set; }

    // --- Dữ liệu cho File đính kèm ---
    /// <summary>
    /// ID của file đã được upload lên File Management (cho component loại FILE).
    /// Được bind từ hidden input (do JS cập nhật) khi submit.
    /// </summary>
    [HiddenInput] // Bind giá trị này từ hidden input trên form
    public Guid? FileId { get; set; }

    // --- Thông tin file đã upload (chỉ để hiển thị, không bind ngược) ---
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? FileContentType { get; set; }
}