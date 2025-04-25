using System;
using MiniExcelLibs.Attributes;

namespace Aqt.CoreFW.Application.Contracts.Procedures.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu Procedure ra Excel.
/// </summary>
public class ProcedureExcelDto
{
    [ExcelColumnName("Mã Thủ tục")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Thủ tục")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Thứ tự")]
    public int Order { get; set; }

    [ExcelColumnName("Trạng thái")]
    public string StatusText { get; set; } = string.Empty; // Lấy từ localization

    [ExcelColumnName("Mô tả")]
    public string? Description { get; set; }

    [ExcelColumnName("Ngày đồng bộ cuối")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")] // Định dạng ngày giờ
    public DateTime? LastSyncedDate { get; set; }

    // Có thể thêm các trường audit nếu cần
    // [ExcelColumnName("Ngày tạo")]
    // [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    // public DateTime CreationTime { get; set; }
} 