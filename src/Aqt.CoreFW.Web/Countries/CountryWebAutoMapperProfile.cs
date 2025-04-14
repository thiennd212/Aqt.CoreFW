using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Web.Pages.Countries; // Namespace of the ViewModel
using AutoMapper;

namespace Aqt.CoreFW.Web.Countries;

/// <summary>
/// Configures AutoMapper profiles for mapping between Web ViewModels and Application DTOs for Countries.
/// </summary>
public class CountryWebAutoMapperProfile : Profile
{
    public CountryWebAutoMapperProfile()
    {
        // Map from ViewModel used in Create/Edit forms to the DTO used by the AppService
        CreateMap<CountryViewModel, CreateUpdateCountryDto>();

        // Map from the Create/Update DTO back to the ViewModel (less common, but might be needed)
        CreateMap<CreateUpdateCountryDto, CountryViewModel>();

        // Map from the standard DTO (used for listing/getting) to the ViewModel (used in EditModal OnGet)
        CreateMap<CountryDto, CountryViewModel>();

        // Map from ViewModel back to standard DTO (less common, might be needed if displaying ViewModel data elsewhere)
        // CreateMap<CountryViewModel, CountryDto>();
    }
} 