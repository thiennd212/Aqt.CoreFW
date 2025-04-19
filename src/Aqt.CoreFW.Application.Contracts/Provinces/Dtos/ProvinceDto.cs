using System;
using Aqt.CoreFW.Domain.Shared.Provinces;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Provinces.Dtos;

public class ProvinceDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public ProvinceStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public DateTime? LastSyncedTime { get; set; }
    public string? SyncId { get; set; }
    public string? SyncCode { get; set; }
}