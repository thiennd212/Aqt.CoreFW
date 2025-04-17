using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;
using Aqt.CoreFW.Web.Pages.JobTitles.ViewModels;
using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Web.Pages.Countries.ViewModels;
using AutoMapper;

namespace Aqt.CoreFW.Web;

public class CoreFWWebAutoMapperProfile : Profile
{
    public CoreFWWebAutoMapperProfile()
    {
        // Configure your AutoMapper configuration here for the Web project.

        // Mapping từ CountryViewModel (Web) sang CreateUpdateCountryDto (Application.Contracts)
        CreateMap<CountryViewModel, CreateUpdateCountryDto>();
        // Mapping từ JobTitleViewModel (Web) sang CreateUpdateJobTitleDto (Application.Contracts)
        CreateMap<JobTitleViewModel, CreateUpdateJobTitleDto>();

        // Thêm các mapping khác của tầng Web nếu cần...
    }
}
