using System;
using MiniExcelLibs.Attributes; // Dùng MiniExcelLibs

namespace Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu AccountType ra Excel.
/// </summary>
public class AccountTypeExcelDto
{
    [ExcelColumnName("Mã Loại tài khoản")] // Tên cột trong file Excel
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Loại tài khoản")]
    public string Name { get; set; } = string.Empty;

    [ExcelColumnName("Thứ tự")]
    public int Order { get; set; }

    [ExcelColumnName("Trạng thái")] // Xuất ra text đã được localize
    public string StatusText { get; set; } = string.Empty; // Sẽ được gán giá trị localize trong Application Service

    [ExcelColumnName("Mô tả")]
    public string? Description { get; set; }

    [ExcelColumnName("ID Bản ghi Đồng bộ")]
    public Guid? SyncRecordId { get; set; }

    [ExcelColumnName("Mã Bản ghi Đồng bộ")]
    public string? SyncRecordCode { get; set; }

    [ExcelColumnName("Ngày đồng bộ cuối")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")] // Định dạng ngày giờ trong Excel
    public DateTime? LastSyncDate { get; set; }

    // Thêm các trường audit nếu cần xuất (ví dụ: CreationTime)
    // [ExcelColumnName("Ngày tạo")]
    // [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    // public DateTime CreationTime { get; set; }
}