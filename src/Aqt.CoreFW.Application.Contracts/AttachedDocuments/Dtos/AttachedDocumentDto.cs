using System;
using Aqt.CoreFW.AttachedDocuments; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos;

public class AttachedDocumentDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AttachedDocumentStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public Guid ProcedureId { get; set; } // ID của thủ tục hành chính

    // Thông tin bổ sung về thủ tục hành chính (optional, được fill bởi AppService)
    public string? ProcedureName { get; set; } // Tên của Procedure
    public string? ProcedureCode { get; set; } // Mã của Procedure
} 