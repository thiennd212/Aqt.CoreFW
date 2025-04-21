    using System;
    using Volo.Abp.Application.Dtos;

    // Namespace updated to Shared/Lookups
    namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups;

    public class DistrictLookupDto : EntityDto<Guid>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid ProvinceId { get; set; } // Keep ProvinceId for context if needed
    }