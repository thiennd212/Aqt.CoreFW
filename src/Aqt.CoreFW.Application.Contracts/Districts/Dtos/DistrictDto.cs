    using System;
    using Aqt.CoreFW.Domain.Shared.Districts;
    using Volo.Abp.Application.Dtos;

    namespace Aqt.CoreFW.Application.Contracts.Districts.Dtos;

    public class DistrictDto : FullAuditedEntityDto<Guid>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DistrictStatus Status { get; set; }
        public int Order { get; set; }
        public string? Description { get; set; }
        public Guid ProvinceId { get; set; }
        public string ProvinceName { get; set; } = string.Empty; // Added Province Name
        public DateTime? LastSyncedTime { get; set; }
        public string? SyncId { get; set; }
        public string? SyncCode { get; set; }
    }