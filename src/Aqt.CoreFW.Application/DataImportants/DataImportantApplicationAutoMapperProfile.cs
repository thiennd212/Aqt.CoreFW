using Aqt.CoreFW.Application.Contracts.DataImportants.Dtos; // DTOs for DataImportant
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTOs
using Aqt.CoreFW.Domain.DataImportants.Entities; // DataImportant Entity
using Aqt.CoreFW.Domain.DataGroups.Entities; // DataGroup Entity (for lookup mapping)
using AutoMapper;

namespace Aqt.CoreFW.Application.DataImportants; // Namespace for DataImportant Application layer

public class DataImportantApplicationAutoMapperProfile : Profile
{
    public DataImportantApplicationAutoMapperProfile()
    {
        // --- DataImportant Mappings ---
        CreateMap<DataImportant, DataImportantDto>()
            // Ignore DataGroupName/DataGroupCode, will be populated in AppService GetList/Get
            .ForMember(dest => dest.DataGroupName, opt => opt.Ignore())
            .ForMember(dest => dest.DataGroupCode, opt => opt.Ignore());

        // Map DTO to DTO for edit form prepopulation (optional but convenient)
        CreateMap<DataImportantDto, CreateUpdateDataImportantDto>();

        // Map Entity to Lookup DTO
        CreateMap<DataImportant, DataImportantLookupDto>();

        // Map Entity to Excel DTO (if Excel Export is implemented)
        CreateMap<DataImportant, DataImportantExcelDto>()
            .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // Handled by MappingAction or AppService
            .ForMember(dest => dest.DataGroupCode, opt => opt.Ignore()) // Handled by AppService
            .ForMember(dest => dest.DataGroupName, opt => opt.Ignore()); // Handled by AppService
            // .AfterMap<DataImportantToExcelMappingAction>(); // Apply action if used and only handles localization

        // --- Shared/Lookup Mappings (ensure DataGroup profile exists) ---
        // CreateMap<DataGroup, DataGroupLookupDto>(); // Should exist in DataGroup profile

        // No direct mapping from CreateUpdateDataImportantDto to DataImportant entity
        // Create/Update operations use DTO data with DataImportantManager
    }
}
