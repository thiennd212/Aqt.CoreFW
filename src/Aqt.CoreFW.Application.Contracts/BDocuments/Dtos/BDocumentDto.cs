using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

public class BDocumentDto : FullAuditedEntityDto<Guid>
{
    public Guid ProcedureId { get; set; }
    public string? ProcedureName { get; set; }

    public string Code { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? ApplicantIdentityNumber { get; set; }
    public string? ApplicantAddress { get; set; }
    public string? ApplicantEmail { get; set; }
    public string? ApplicantPhoneNumber { get; set; }
    public string? ScopeOfActivity { get; set; }
    public bool ReceiveByPost { get; set; }

    public Guid? WorkflowStatusId { get; set; }
    public string? WorkflowStatusName { get; set; }
    public string? WorkflowStatusColorCode { get; set; }

    public DateTime? SubmissionDate { get; set; }
    public DateTime? ReceptionDate { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public DateTime? ResultDate { get; set; }
    public string? RejectionOrAdditionReason { get; set; }

    public List<BDocumentDataDto> DocumentData { get; set; } = new List<BDocumentDataDto>();
} 