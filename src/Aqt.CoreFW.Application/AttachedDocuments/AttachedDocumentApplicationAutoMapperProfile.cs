using Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos; // DTOs for AttachedDocument
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTOs
using Aqt.CoreFW.Domain.AttachedDocuments.Entities; // AttachedDocument Entity
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity (for lookup mapping - Giả định)
using AutoMapper;

namespace Aqt.CoreFW.Application.AttachedDocuments; // Namespace for AttachedDocument Application layer

public class AttachedDocumentApplicationAutoMapperProfile : Profile
{
    public AttachedDocumentApplicationAutoMapperProfile()
    {
        // --- AttachedDocument Mappings ---
        CreateMap<AttachedDocument, AttachedDocumentDto>()
            // Ignore ProcedureName/ProcedureCode, will be populated in AppService GetList/Get
            .ForMember(dest => dest.ProcedureName, opt => opt.Ignore())
            .ForMember(dest => dest.ProcedureCode, opt => opt.Ignore());

        // Map DTO to DTO for edit form prepopulation (optional but convenient)
        CreateMap<AttachedDocumentDto, CreateUpdateAttachedDocumentDto>();

        // Map Entity to Lookup DTO
        CreateMap<AttachedDocument, AttachedDocumentLookupDto>();

        // Map Entity to Excel DTO (if Excel Export is implemented)
        CreateMap<AttachedDocument, AttachedDocumentExcelDto>()
            .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // Handled by MappingAction or AppService
            .ForMember(dest => dest.ProcedureCode, opt => opt.Ignore()) // Handled by AppService
            .ForMember(dest => dest.ProcedureName, opt => opt.Ignore()); // Handled by AppService
            // .AfterMap<AttachedDocumentToExcelMappingAction>(); // Apply action if used and only handles localization

        // --- Shared/Lookup Mappings (ensure Procedure profile exists - Giả định) ---
        // CreateMap<Procedure, ProcedureLookupDto>(); // Should exist in Procedure profile

        // No direct mapping from CreateUpdateAttachedDocumentDto to AttachedDocument entity
        // Create/Update operations use DTO data with AttachedDocumentManager
    }
} 