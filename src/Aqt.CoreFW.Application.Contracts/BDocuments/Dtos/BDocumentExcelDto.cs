using System;
using MiniExcelLibs.Attributes; // Giả sử dùng MiniExcel

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu BDocument ra Excel.
/// </summary>
public class BDocumentExcelDto
{
    [ExcelColumnName("Mã Hồ sơ")]
    public string Code { get; set; } = string.Empty;

    [ExcelColumnName("Tên Chủ hồ sơ")]
    public string ApplicantName { get; set; } = string.Empty;

    [ExcelColumnName("Thủ tục")]
    public string ProcedureName { get; set; } = string.Empty;

    [ExcelColumnName("Trạng thái")]
    public string StatusName { get; set; } = string.Empty;

    [ExcelColumnName("Phạm vi hoạt động")]
    public string? ScopeOfActivity { get; set; }

    [ExcelColumnName("Đăng ký nhận qua BĐ")]
    public string ReceiveByPost { get; set; } = string.Empty;

    [ExcelColumnName("Ngày nộp")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? SubmissionDate { get; set; }

    [ExcelColumnName("Ngày tiếp nhận")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? ReceptionDate { get; set; }

    [ExcelColumnName("Ngày hẹn trả")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? AppointmentDate { get; set; }

    [ExcelColumnName("Ngày trả kết quả")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? ResultDate { get; set; }

    [ExcelIgnore]
    public Guid ProcedureId { get; set; }

    [ExcelIgnore]
    public Guid? WorkflowStatusId { get; set; }
} 