using System;
using Aqt.CoreFW.AccountTypes; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos;

public class AccountTypeDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto để có thông tin audit
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountTypeStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public DateTime? LastSyncDate { get; set; }
    public Guid? SyncRecordId { get; set; }
    public string? SyncRecordCode { get; set; }
}