using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Aqt.CoreFW.Application.Contracts.Countries;

/// <summary>
/// Application service interface for managing Countries.
/// Extends the standard CRUD interface and adds specific methods like lookup.
/// </summary>
public interface ICountryAppService :
    ICrudAppService< // Defines CRUD methods
        CountryDto,               // Used to show countries
        Guid,                     // Primary key of the Country entity
        GetCountriesInput,        // Used for filtering and paging the list
        CreateUpdateCountryDto>   // Used to create/update a country
{
    /// <summary>
    /// Gets a list of countries suitable for lookup (e.g., for dropdowns).
    /// </summary>
    /// <returns>A list of country lookups.</returns>
    Task<ListResultDto<CountryLookupDto>> GetLookupAsync();
} 