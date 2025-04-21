using System;
using MiniExcelLibs.Attributes;

namespace Aqt.CoreFW.Application.Contracts.DataGroups.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu DataGroup ra Excel.
/// </summary>
public class DataGroupExcelDto
{
    [ExcelColumnName("Mã Nhóm")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Nhóm")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Mã Nhóm Cha")] // Thêm thông tin nhóm cha
    public string? ParentCode { get; set; }

    [ExcelColumnName("Tên Nhóm Cha")] // Thêm thông tin nhóm cha
    public string? ParentName { get; set; }

    [ExcelColumnName("Thứ tự")]
    public int Order { get; set; }

    [ExcelColumnName("Trạng thái")]
    public string StatusText { get; set; } = string.Empty; // Lấy từ localization

    [ExcelColumnName("Mô tả")]
    public string? Description { get; set; }

    [ExcelColumnName("ID Bản ghi Đồng bộ")]
    public Guid? SyncRecordId { get; set; }

    [ExcelColumnName("Mã Bản ghi Đồng bộ")]
    public string? SyncRecordCode { get; set; }

    [ExcelColumnName("Ngày đồng bộ cuối")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? LastSyncDate { get; set; }

    // Có thể thêm các trường audit nếu cần
    // [ExcelColumnName("Ngày tạo")]
    // [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    // public DateTime CreationTime { get; set; }
} 