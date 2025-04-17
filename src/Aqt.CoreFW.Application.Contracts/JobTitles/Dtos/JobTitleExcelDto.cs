using System;
using MiniExcelLibs.Attributes; // Sử dụng thuộc tính của MiniExcel

namespace Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;

public class JobTitleExcelDto
{
    // Sử dụng attribute để đặt tên cột trong Excel (có thể đa ngôn ngữ nếu cần)
    [ExcelColumnName("Mã Chức Danh")] // Ví dụ tiếng Việt
    public string Code { get; set; }

    [ExcelColumnName("Tên Chức Danh")]
    public string Name { get; set; }

    [ExcelColumnName("Mô Tả")]
    public string? Description { get; set; }

    [ExcelColumnName("Trạng Thái")] // Hiển thị "Hoạt động" / "Không hoạt động" thay vì true/false
    public string IsActiveText { get; set; } // Dùng chuỗi thay vì bool

    // Có thể bỏ qua các trường Audit (CreatedBy, LastModified...) nếu không cần thiết trong file export
    // [ExcelIgnore] // Bỏ qua nếu không muốn xuất cột này
    // public DateTime CreationTime { get; set; }
}