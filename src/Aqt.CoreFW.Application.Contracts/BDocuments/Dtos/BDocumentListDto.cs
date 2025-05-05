using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// DTO simplified for listing BDocuments.
/// DTO đơn giản hóa cho việc liệt kê BDocuments.
/// </summary>
// Có thể kế thừa từ AuditedEntityDto nếu cần CreationTime, hoặc thêm thủ công
public class BDocumentListDto : EntityDto<Guid>
{
    public string Code { get; set; } = string.Empty; // Đã đổi tên
    public string ApplicantName { get; set; } = string.Empty; // Đã đổi tên

    public Guid ProcedureId { get; set; }
    public string? ProcedureName { get; set; } // Cho phép null

    public Guid? WorkflowStatusId { get; set; } // Đã đổi tên, nullable
    public string? WorkflowStatusName { get; set; } // Đã đổi tên, cho phép null
    public string? WorkflowStatusColorCode { get; set; } // Đã đổi tên, cho phép null

    // Ngày tạo hoặc ngày nộp tùy theo yêu cầu hiển thị/sắp xếp
    public DateTime CreationTime { get; set; } // Kế thừa từ Audited...Dto hoặc thêm thủ công
    public DateTime? SubmissionDate { get; set; } // Hoặc dùng ngày nộp

    // Thêm các cột khác nếu cần
} 