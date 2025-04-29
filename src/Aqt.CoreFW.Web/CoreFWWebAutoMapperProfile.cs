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
using Aqt.CoreFW.Application.Contracts.Components.Dtos;
using Aqt.CoreFW.Web.Pages.Components.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;
using Aqt.CoreFW.Web.Pages.BDocuments.ViewModels;
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

        // --- Thêm mapping cho ProcedureComponent ViewModel <-> DTO ---
        CreateMap<ProcedureComponentViewModel, CreateUpdateProcedureComponentDto>();
        CreateMap<ProcedureComponentDto, ProcedureComponentViewModel>();

        // --- BEGIN BDocument Mappings ---

        // ViewModel -> Create Input DTO
        CreateMap<BDocumentViewModel, CreateBDocumentInputDto>()
            // ComponentDataList trong ViewModel chứa BDocumentDataViewModel
            // CreateBDocumentInputDto.ComponentData chứa CreateBDocumentComponentDataInputDto
            // AutoMapper sẽ tự map list dựa trên mapping của item (được định nghĩa bên dưới)
            .ForMember(dest => dest.ComponentData, opt => opt.MapFrom(src => src.ComponentDataList));

        // ViewModel -> Update Input DTO (Chỉ map trường thông tin chính của BDocument)
        CreateMap<BDocumentViewModel, UpdateBDocumentInputDto>();
        // Update DTO không chứa ComponentDataList, chỉ map các trường cơ bản

        // Detail DTO (BDocumentDto) -> ViewModel (BDocumentViewModel)
        CreateMap<BDocumentDto, BDocumentViewModel>()
            // Map tên và màu từ các đối tượng lồng nhau
            .ForMember(dest => dest.TrangThaiHoSoName, opt => opt.MapFrom(src => src.TrangThaiHoSo != null ? src.TrangThaiHoSo.Name : null))
            .ForMember(dest => dest.TrangThaiHoSoColorCode, opt => opt.MapFrom(src => src.TrangThaiHoSo != null ? src.TrangThaiHoSo.ColorCode : null))
            .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Name : null))
            // Bỏ qua các trường Tờ khai đã bị loại bỏ khỏi ViewModel
            // Map danh sách BDocumentDataDto sang BDocumentDataViewModel
            .ForMember(dest => dest.ComponentDataList, opt => opt.MapFrom(src => src.DocumentData));

        // --- END BDocument Mappings ---


        // --- BEGIN BDocumentData Mappings ---

        // DTO (BDocumentDataDto) -> ViewModel (BDocumentDataViewModel)
        CreateMap<BDocumentDataDto, BDocumentDataViewModel>()
             // Map thông tin file từ FileInfoDto lồng nhau
             .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileInfo != null ? src.FileInfo.FileName : null))
             .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileInfo != null ? src.FileInfo.ByteSize : (long?)null))
            // .ForMember(dest => dest.FileContentType, opt => opt.MapFrom(src => src.FileInfo != null ? src.FileInfo.FileContentType : null))
             // FormDefinition sẽ được load từ AppService khi gọi OnGetAsync của CreateModal, không map ở đây
             .ForMember(dest => dest.FormDefinition, opt => opt.Ignore())
             // Map dữ liệu JSON đã lưu từ DTO sang ViewModel
             .ForMember(dest => dest.FormData, opt => opt.MapFrom(src => src.FormData));

        // ViewModel (BDocumentDataViewModel) -> Create Input DTO Component (CreateBDocumentComponentDataInputDto)
        // Dùng khi map từ BDocumentViewModel sang CreateBDocumentInputDto
        CreateMap<BDocumentDataViewModel, CreateBDocumentComponentDataInputDto>();
        // AutoMapper sẽ tự động map các trường trùng tên: ProcedureComponentId, FormData, FileId

        // --- END BDocumentData Mappings ---


        // --- BEGIN File Management Mappings ---

        // Map DTO của EasyAbp.FileManagement sang FileInfoDto của Application.Contracts
        // Cần thiết nếu bạn sử dụng FileInfoDto của Contracts ở đâu đó trong Web layer
        //CreateMap<EasyAbp.FileManagement.Files.Dtos.FileInfoDto, Aqt.CoreFW.Application.Contracts.Files.FileInfoDto>();

        // --- END File Management Mappings ---


        // --- BEGIN Lookup Mappings (Cho Filter trang Index) ---

        CreateMap<ProcedureLookupDto, SelectListItem>()
           .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id.ToString()))
           .ForMember(dest => dest.Text, opt => opt.MapFrom(src => $"{src.Code} - {src.Name}"));

        CreateMap<WorkflowStatusLookupDto, SelectListItem>()
           .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id.ToString()))
           .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

        // --- END Lookup Mappings ---
    }
}
