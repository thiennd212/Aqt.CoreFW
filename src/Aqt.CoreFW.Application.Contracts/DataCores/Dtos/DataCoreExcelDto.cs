using System;
using MiniExcelLibs.Attributes;

namespace Aqt.CoreFW.Application.Contracts.DataCores.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu DataCore ra Excel.
/// </summary>
public class DataCoreExcelDto
{
    [ExcelColumnName("Mã DL Cốt lõi")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên DL Cốt lõi")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Mã Nhóm DL")] // Thêm thông tin nhóm dữ liệu
    public string? DataGroupCode { get; set; }

    [ExcelColumnName("Tên Nhóm DL")] // Thêm thông tin nhóm dữ liệu
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