using System;
using Aqt.CoreFW.DataGroups; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.DataGroups.Dtos;

public class DataGroupDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DataGroupStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; } // ID của nhóm cha

    // Thông tin bổ sung về nhóm cha (optional, được fill bởi AppService)
    public string? ParentCode { get; set; }
    public string? ParentName { get; set; }

    public DateTime? LastSyncDate { get; set; }
    public Guid? SyncRecordId { get; set; }
    public string? SyncRecordCode { get; set; }
} 