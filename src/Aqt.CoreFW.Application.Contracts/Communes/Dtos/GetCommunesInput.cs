using System;
using Aqt.CoreFW.Domain.Shared.Communes;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Communes.Dtos;

public class GetCommunesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public CommuneStatus? Status { get; set; }
    public Guid? ProvinceId { get; set; }
    public Guid? DistrictId { get; set; }
}