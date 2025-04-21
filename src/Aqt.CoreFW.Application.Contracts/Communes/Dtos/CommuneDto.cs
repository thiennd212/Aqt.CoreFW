using System;
using Aqt.CoreFW.Domain.Shared.Communes;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Communes.Dtos;

public class CommuneDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CommuneStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public Guid ProvinceId { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public DateTime? LastSyncedTime { get; set; }
    public string? SyncId { get; set; }
    public string? SyncCode { get; set; }
}