using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups;

public class OrganizationUnitLookupDto : EntityDto<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    // Add ManualCode if needed for display in lookup
    // public string? ManualCode { get; set; }
} 