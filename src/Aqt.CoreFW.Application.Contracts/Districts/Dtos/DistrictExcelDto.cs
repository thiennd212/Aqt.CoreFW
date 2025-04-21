using System;
using MiniExcelLibs.Attributes;

namespace Aqt.CoreFW.Application.Contracts.Districts.Dtos;

public class DistrictExcelDto
{
    [ExcelColumnName("Mã Quận/Huyện")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Quận/Huyện")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Tỉnh/Thành phố")]
    public string ProvinceName { get; set; } = string.Empty; // Province Name instead of Id

    [ExcelColumnName("Thứ tự")]
    public int Order { get; set; }

    [ExcelColumnName("Trạng thái")]
    public string StatusText { get; set; } = string.Empty; // Localized status text

    [ExcelColumnName("Mô tả")]
    public string? Description { get; set; }

    [ExcelColumnName("Sync ID")]
    public string? SyncId { get; set; }

    [ExcelColumnName("Sync Code")]
    public string? SyncCode { get; set; }

    [ExcelColumnName("Thời gian đồng bộ cuối")]
    public DateTime? LastSyncedTime { get; set; }
    // public DateTime CreationTime { get; set; } // Optional: Add audit fields if needed
}