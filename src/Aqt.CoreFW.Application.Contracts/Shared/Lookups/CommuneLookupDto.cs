using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Communes.Dtos;

public class CommuneLookupDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public Guid? DistrictId { get; set; }
    public Guid ProvinceId { get; set; }
}