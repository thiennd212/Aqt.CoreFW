using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

/// <summary>
/// DTO for displaying Country information.
/// Includes audit properties from the base class.
/// </summary>
public class CountryDto : AuditedEntityDto<Guid>
{
    /// <summary>
    /// Country code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    public string Name { get; set; }
} 