using System;
using MiniExcelLibs.Attributes; // Giả sử dùng MiniExcel

namespace Aqt.CoreFW.Application.Contracts.Components.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu ProcedureComponent ra Excel.
/// </summary>
public class ProcedureComponentExcelDto
{
    [ExcelColumnName("Mã Thành phần")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Thành phần")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Thứ tự")]
    public int Order { get; set; }

    [ExcelColumnName("Loại")] // Loại (Form/File)
    public string TypeText { get; set; } = string.Empty; // Lấy từ localization

    [ExcelColumnName("Trạng thái")]
    public string StatusText { get; set; } = string.Empty; // Lấy từ localization

    [ExcelColumnName("Mô tả")]
    public string? Description { get; set; }

    [ExcelColumnName("Định nghĩa Form (JSON)")] // Có thể không cần nếu quá dài
    public string? FormDefinition { get; set; }

    [ExcelColumnName("Đường dẫn Template")]
    public string? TempPath { get; set; }

    // Có thể thêm các trường audit nếu cần
    // [ExcelColumnName("Ngày tạo")]
    // [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    // public DateTime CreationTime { get; set; }
} 