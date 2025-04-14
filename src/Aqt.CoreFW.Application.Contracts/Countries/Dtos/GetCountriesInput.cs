using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

/// <summary>
/// Input DTO for getting a paged list of Countries.
/// Inherits standard paging and sorting properties and adds a custom filter.
/// </summary>
public class GetCountriesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Generic filter string to search by Code or Name.
    /// </summary>
    public string? Filter { get; set; }
} 