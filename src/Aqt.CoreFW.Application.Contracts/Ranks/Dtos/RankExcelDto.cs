using System;
using MiniExcelLibs.Attributes;

namespace Aqt.CoreFW.Application.Contracts.Ranks.Dtos
{
    /// <summary>
    /// DTO được thiết kế riêng cho việc xuất dữ liệu Rank ra Excel.
    /// </summary>
    public class RankExcelDto
    {
        [ExcelColumnName("Mã Cấp bậc")] // Tên cột trong file Excel
        public string Code { get; set; } = string.Empty;

        [ExcelColumnName("Tên Cấp bậc")]
        public string Name { get; set; } = string.Empty;

        [ExcelColumnName("Thứ tự")]
        public int Order { get; set; }

        [ExcelColumnName("Trạng thái")]
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
    }
}