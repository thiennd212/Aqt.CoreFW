using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.Countries;

namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

/// <summary>
/// DTO for creating or updating a Country.
/// Includes validation attributes.
/// </summary>
public class CreateUpdateCountryDto
{
    /// <summary>
    /// Country code.
    /// Required, Max Length defined in CountryConsts.
    /// </summary>
    [Required]
    [StringLength(CountryConsts.MaxCodeLength)]
    public string Code { get; set; }

    /// <summary>
    /// Country name.
    /// Required, Max Length defined in CountryConsts.
    /// </summary>
    [Required]
    [StringLength(CountryConsts.MaxNameLength)]
    public string Name { get; set; }
} 