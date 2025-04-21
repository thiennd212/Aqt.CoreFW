using Aqt.CoreFW.Application.Contracts.DataGroups.Dtos; // DTOs for DataGroup
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO
using Aqt.CoreFW.Domain.DataGroups.Entities; // DataGroup Entity
using AutoMapper;

namespace Aqt.CoreFW.Application.DataGroups; // Namespace for DataGroup Application layer

public class DataGroupApplicationAutoMapperProfile : Profile
{
    public DataGroupApplicationAutoMapperProfile()
    {
        // --- DataGroup Mappings ---
        CreateMap<DataGroup, DataGroupDto>()
            // Ignore ParentCode/ParentName, will be populated in AppService GetList/Get
            .ForMember(dest => dest.ParentCode, opt => opt.Ignore())
            .ForMember(dest => dest.ParentName, opt => opt.Ignore());

        CreateMap<DataGroupDto, CreateUpdateDataGroupDto>(); // For prepopulating edit form

        CreateMap<DataGroup, DataGroupLookupDto>(); // For flat lookup lists

        CreateMap<DataGroup, DataGroupTreeNodeDto>()
            .ForMember(dest => dest.Children, opt => opt.Ignore()); // Children populated recursively

        CreateMap<DataGroup, DataGroupExcelDto>()
            .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // Handled by MappingAction (or AppService)
            .ForMember(dest => dest.ParentCode, opt => opt.Ignore()) // Handled by AppService
            .ForMember(dest => dest.ParentName, opt => opt.Ignore()) // Handled by AppService
            .AfterMap<DataGroupToExcelMappingAction>(); // Apply action if only StatusText is handled by it

        // No direct mapping from CreateUpdateDataGroupDto to DataGroup entity
        // Create/Update operations use DTO data with DataGroupManager
    }
}