using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;

public class GetOrganizationUnitsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    // Add other filter properties if needed, e.g.:
    // public OrganizationUnitStatus? Status { get; set; }
} 