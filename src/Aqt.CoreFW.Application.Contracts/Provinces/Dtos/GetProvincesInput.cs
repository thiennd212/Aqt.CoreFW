using System;
using Aqt.CoreFW.Domain.Shared.Provinces;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Provinces.Dtos;

public class GetProvincesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public ProvinceStatus? Status { get; set; }
    public Guid? CountryId { get; set; }
}