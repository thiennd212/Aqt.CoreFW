using System;
using MiniExcelLibs.Attributes;

namespace Aqt.CoreFW.Application.Contracts.Communes.Dtos;

public class CommuneExcelDto
{
    [ExcelColumnName("Mã Xã/Phường")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Xã/Phường")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Quận/Huyện")]
    public string? DistrictName { get; set; }

    [ExcelColumnName("Tỉnh/Thành phố")]
    public string ProvinceName { get; set; } = string.Empty;

    [ExcelColumnName("Thứ tự")]
    public int Order { get; set; }

    [ExcelColumnName("Trạng thái")]
    public string StatusText { get; set; } = string.Empty; // Sẽ được gán giá trị localized trong AppService

    [ExcelColumnName("Mô tả")]
    public string? Description { get; set; }

    [ExcelColumnName("Sync ID")]
    public string? SyncId { get; set; }

    [ExcelColumnName("Sync Code")]
    public string? SyncCode { get; set; }

    [ExcelColumnName("Thời gian đồng bộ cuối")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")] // Định dạng ngày giờ cho Excel
    public DateTime? LastSyncedTime { get; set; }
}