using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

/// <summary>
/// DTO for providing Country information in lookup scenarios (e.g., dropdowns).
/// </summary>
public class CountryLookupDto : EntityDto<Guid>
{
    /// <summary>
    /// Country name.
    /// </summary>
    public string Name { get; set; }

    // You can uncomment this if the Code is also needed in the lookup display.
    // public string Code { get; set; }
} 