using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Provinces.Dtos;

public class ProvinceLookupDto : EntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
}