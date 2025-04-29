using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos; // DTOs
using Aqt.CoreFW.Domain.BDocuments.Entities; // Entities
using AutoMapper;
using EasyAbp.FileManagement.Files; // File Entity from FileManagement
using EasyAbp.FileManagement.Files.Dtos; // FileInfoDto from FileManagement

namespace Aqt.CoreFW.Application.BDocuments;

public class BDocumentApplicationAutoMapperProfile : Profile
{
    public BDocumentApplicationAutoMapperProfile()
    {
        // --- BDocument Mappings ---
        CreateMap<BDocument, BDocumentDto>()
             .ForMember(dest => dest.Procedure, opt => opt.Ignore()) // Enrich later in AppService
             .ForMember(dest => dest.TrangThaiHoSo, opt => opt.Ignore()); // Enrich later in AppService

        CreateMap<BDocument, BDocumentListDto>()
             .ForMember(dest => dest.ProcedureName, opt => opt.Ignore()) // Enrich later in AppService
             .ForMember(dest => dest.TrangThaiHoSoName, opt => opt.Ignore()) // Enrich later in AppService
             .ForMember(dest => dest.TrangThaiHoSoColorCode, opt => opt.Ignore()); // Enrich later in AppService

        // --- BDocumentData Mapping ---
        CreateMap<BDocumentData, BDocumentDataDto>()
            .ForMember(dest => dest.ComponentCode, opt => opt.Ignore()) // Enrich later in AppService
            .ForMember(dest => dest.ComponentName, opt => opt.Ignore()) // Enrich later in AppService
            .ForMember(dest => dest.ComponentType, opt => opt.Ignore()) // Enrich later in AppService
            .ForMember(dest => dest.IsRequired, opt => opt.Ignore())   // Enrich later in AppService (from ProcedureComponentLink)
            .ForMember(dest => dest.FileInfo, opt => opt.Ignore());    // Enrich later in AppService (from FileManagement)

        // --- Excel Mapping ---
        CreateMap<BDocument, BDocumentExcelDto>()
             .ForMember(dest => dest.ProcedureName, opt => opt.Ignore()) // Handle in AppService or Action
             .ForMember(dest => dest.StatusName, opt => opt.Ignore())   // Handle in AppService or Action
             .ForMember(dest => dest.DangKyNhanQuaBuuDien, opt => opt.MapFrom(src => src.DangKyNhanQuaBuuDien ? "Yes" : "No")); // Simple bool mapping
                                                                                                                                // Uncomment if using BDocumentToExcelMappingAction for complex logic:
                                                                                                                                // .AfterMap<BDocumentToExcelMappingAction>();

        // --- File Management Mapping ---
        // Map File Entity from FileManagement to our FileInfoDto Contract
        // This assumes Aqt.CoreFW.Application.Contracts.Files.FileInfoDto exists and matches needed properties.
        // If not, create a specific mapping or use FileManagement's DTO directly.
        // CreateMap<File, Aqt.CoreFW.Application.Contracts.Files.FileInfoDto>(); // Map File Entity to our contract DTO

        // Map FileInfoDto from EasyAbp.FileManagement to our contract FileInfoDto if needed
        // CreateMap<EasyAbp.FileManagement.Files.Dtos.FileInfoDto, Aqt.CoreFW.Application.Contracts.Files.FileInfoDto>();

        // Map FileInfoDto from EasyAbp directly (if BDocumentDataDto uses it)
        // No explicit mapping needed if BDocumentDataDto directly references EasyAbp's DTO.
        // If BDocumentDataDto uses its own FileInfoDto contract, the mapping above is required.
        CreateMap<File, FileInfoDto>(); // Map FileManagement Entity to FileManagement DTO
                                        // Map FileManagement DTO to our Contract DTO (if they are different)
                                        // CreateMap<EasyAbp.FileManagement.Files.Dtos.FileInfoDto, Aqt.CoreFW.Application.Contracts.Files.FileInfoDto>();


        // --- Input DTOs to Entity Mappings ---
        // NO direct mapping for Create/Update DTOs to Entities.
        // Logic should go through the Domain Service (BDocumentManager).
    }
}