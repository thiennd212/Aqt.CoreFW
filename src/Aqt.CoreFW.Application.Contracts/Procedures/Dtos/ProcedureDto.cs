using System;
using Aqt.CoreFW.Procedures; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Procedures.Dtos;

public class ProcedureDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProcedureStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }

    // Thông tin đồng bộ (hiển thị nếu cần)
    public DateTime? LastSyncedDate { get; set; }
    public Guid? SyncRecordId { get; set; }
    public string? SyncRecordCode { get; set; }
} 