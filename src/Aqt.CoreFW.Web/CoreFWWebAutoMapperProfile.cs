using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;
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
using Aqt.CoreFW.Application.Contracts.DataCores.Dtos;
using Aqt.CoreFW.Web.Pages.DataCores.ViewModels;
using Aqt.CoreFW.Application.Contracts.DataImportants.Dtos;
using Aqt.CoreFW.Web.Pages.DataImportants.ViewModels;
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos;
using Aqt.CoreFW.Web.Pages.Procedures.ViewModels;
using Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos; 
using Aqt.CoreFW.Web.Pages.AttachedDocuments.ViewModels;   
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

        // Thêm mapping cho DataCore ViewModel <-> DTO
        CreateMap<DataCoreViewModel, CreateUpdateDataCoreDto>();
        CreateMap<DataCoreDto, DataCoreViewModel>();

        // Thêm mapping cho DataImportant ViewModel <-> DTO
        CreateMap<DataImportantViewModel, CreateUpdateDataImportantDto>();
        CreateMap<DataImportantDto, DataImportantViewModel>();

        // --- Mappings cho Procedure ---
        CreateMap<ProcedureViewModel, CreateUpdateProcedureDto>(); // ViewModel -> Create/Update DTO
        CreateMap<ProcedureDto, ProcedureViewModel>();

        // Mapping cho AttachedDocument ViewModel <-> DTO
        CreateMap<AttachedDocumentViewModel, CreateUpdateAttachedDocumentDto>();
        CreateMap<AttachedDocumentDto, AttachedDocumentViewModel>();
    }
}
