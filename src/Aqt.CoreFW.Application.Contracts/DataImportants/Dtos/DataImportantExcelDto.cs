using System;
using MiniExcelLibs.Attributes;

namespace Aqt.CoreFW.Application.Contracts.DataImportants.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu DataImportant ra Excel.
/// </summary>
public class DataImportantExcelDto
{
    [ExcelColumnName("Mã DL Quan trọng")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên DL Quan trọng")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Mã Nhóm DL")]
    public string? DataGroupCode { get; set; }

    [ExcelColumnName("Tên Nhóm DL")]
    public string? DataGroupName { get; set; }

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