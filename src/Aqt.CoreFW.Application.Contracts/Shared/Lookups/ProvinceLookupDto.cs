    using System;
    using Volo.Abp.Application.Dtos;

    namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups;

    public class ProvinceLookupDto : EntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        // public string Code { get; set; } = string.Empty; // Optional: Add Code if needed
    }