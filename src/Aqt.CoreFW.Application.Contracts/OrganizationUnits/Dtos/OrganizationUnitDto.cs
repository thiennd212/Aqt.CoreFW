using System;
using Aqt.CoreFW.OrganizationUnits;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;

public class OrganizationUnitDto : ExtensibleAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    // --- Extended Properties ---
    public string? ManualCode { get; set; }
    public OrganizationUnitStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public DateTime? LastSyncedTime { get; set; }
    public string? SyncRecordId { get; set; }
    public string? SyncRecordCode { get; set; }

    public string ConcurrencyStamp { get; set; } = string.Empty;

    // Optional additional properties can be added here if needed for display
    // public int ChildrenCount { get; set; }
    // public string? ParentDisplayName { get; set; }
} 