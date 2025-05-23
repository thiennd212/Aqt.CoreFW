﻿using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;
using Aqt.CoreFW.Web.Pages.JobTitles.ViewModels;
using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Web.Pages.Countries.ViewModels;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;
using Aqt.CoreFW.Web.Pages.WorkflowStatuses.ViewModels;
using AutoMapper;
using Aqt.CoreFW.Application.Contracts.Provinces.Dtos;
using Aqt.CoreFW.Web.Pages.Provinces.ViewModels;
using Aqt.CoreFW.Application.Contracts.Districts.Dtos;
using Aqt.CoreFW.Web.Pages.Districts.ViewModels;
using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Web.Pages.Communes.ViewModels;
using Aqt.CoreFW.Application.Contracts.Ranks.Dtos;
using Aqt.CoreFW.Web.Pages.Ranks.ViewModels;
using Aqt.CoreFW.Application.Contracts.DataGroups.Dtos;
using Aqt.CoreFW.Web.Pages.DataGroups.ViewModels;
using Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos;
using Aqt.CoreFW.Web.Pages.AccountTypes.ViewModels;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using Aqt.CoreFW.Web.Pages.OrganizationUnits.ViewModels;

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

        CreateMap<DistrictViewModel, CreateUpdateDistrictDto>(); // ViewModel -> DTO (Create/Update)
        CreateMap<DistrictDto, DistrictViewModel>();

        CreateMap<CommuneViewModel, CreateUpdateCommuneDto>();
        CreateMap<CommuneDto, CommuneViewModel>();

        CreateMap<RankViewModel, CreateUpdateRankDto>();
        CreateMap<RankDto, RankViewModel>();

        CreateMap<DataGroupViewModel, CreateUpdateDataGroupDto>();
        CreateMap<DataGroupDto, DataGroupViewModel>();

        CreateMap<AccountTypeViewModel, CreateUpdateAccountTypeDto>(); // ViewModel -> DTO (cho Create/Update)
        CreateMap<AccountTypeDto, AccountTypeViewModel>();

        // Thêm mapping cho OrganizationUnit ViewModel <-> DTO
        CreateMap<OrganizationUnitViewModel, CreateOrganizationUnitDto>(); // ViewModel -> DTO (cho Create)
        CreateMap<OrganizationUnitViewModel, UpdateOrganizationUnitDto>(); // ViewModel -> DTO (cho Update)
        CreateMap<OrganizationUnitDto, OrganizationUnitViewModel>();             // DTO -> ViewModel (cho Edit)

        // Thêm các mapping khác của tầng Web nếu cần...
    }
}
