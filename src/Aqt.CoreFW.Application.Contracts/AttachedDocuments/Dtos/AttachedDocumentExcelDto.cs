using System;
using MiniExcelLibs.Attributes; // Giả định sử dụng MiniExcel

namespace Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu AttachedDocument ra Excel.
/// </summary>
public class AttachedDocumentExcelDto
{
    [ExcelColumnName("Mã Hồ sơ")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Hồ sơ")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Mã Thủ tục")] // Thêm thông tin Procedure
    public string? ProcedureCode { get; set; }

    [ExcelColumnName("Tên Thủ tục")] // Thêm thông tin Procedure
    public string? ProcedureName { get; set; }

    [ExcelColumnName("Thứ tự")]
    public int Order { get; set; }

    [ExcelColumnName("Trạng thái")]
    public string StatusText { get; set; } = string.Empty; // Lấy từ localization

    [ExcelColumnName("Mô tả")]
    public string? Description { get; set; }

    // Có thể thêm các trường audit nếu cần
    // [ExcelColumnName("Ngày tạo")]
    // [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    // public DateTime CreationTime { get; set; }
} 