using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

public class GetBDocumentsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; } // Filter theo Code hoặc ApplicantName
    public Guid? ProcedureId { get; set; }
    public Guid? WorkflowStatusId { get; set; } // Đổi tên từ StatusId
    public DateTime? SubmissionDateFrom { get; set; }
    public DateTime? SubmissionDateTo { get; set; }
    // public bool? ReceiveByPost { get; set; } // Filter theo nhận qua bưu điện?
} 