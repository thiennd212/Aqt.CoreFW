using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;
using Aqt.CoreFW.Web.Pages.JobTitles.ViewModels;
using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Web.Pages.Countries.ViewModels;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;
using Aqt.CoreFW.Web.Pages.WorkflowStatuses.ViewModels;
using AutoMapper;
using Aqt.CoreFW.Application.Contracts.Provinces.Dtos;
using Aqt.CoreFW.Web.Pages.Provinces.ViewModels;

namespace Aqt.CoreFW.Web;

public class CoreFWWebAutoMapperProfile : Profile
{
    public CoreFWWebAutoMapperProfile()
    {
        // Configure your AutoMapper configuration here for the Web project.

        //Mapping cho country
        CreateMap<CountryViewModel, CreateUpdateCountryDto>();
        CreateMap<CreateUpdateCountryDto, CountryViewModel>();
        //Mapping cho jobTitle
        CreateMap<JobTitleViewModel, CreateUpdateJobTitleDto>();
        CreateMap<CreateUpdateJobTitleDto, JobTitleViewModel>();
        //Mapping cho workflowStatus
        CreateMap<WorkflowStatusViewModel, CreateUpdateWorkflowStatusDto>();
        CreateMap<WorkflowStatusDto, WorkflowStatusViewModel>();

        // Thêm mapping cho Province ViewModel <-> DTO
        CreateMap<ProvinceViewModel, CreateUpdateProvinceDto>();
        CreateMap<ProvinceDto, ProvinceViewModel>(); 

        // Thêm các mapping khác của tầng Web nếu cần...
    }
}
