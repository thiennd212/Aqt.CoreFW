    using Aqt.CoreFW.Application.Contracts.Districts.Dtos;
    using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
    using Aqt.CoreFW.Domain.Districts.Entities; // Verify namespace
    using Aqt.CoreFW.Domain.Provinces.Entities; // Verify namespace
    using AutoMapper;

    namespace Aqt.CoreFW.Application.Districts;

    public class DistrictApplicationAutoMapperProfile : Profile
    {
        public DistrictApplicationAutoMapperProfile()
        {
            // District -> DistrictDto
            CreateMap<District, DistrictDto>()
                .ForMember(dest => dest.ProvinceName, opt => opt.Ignore()); // Manual mapping in AppService

            // For Edit form population
            CreateMap<DistrictDto, CreateUpdateDistrictDto>();

            // District -> DistrictLookupDto (used in Shared/Lookups)
            CreateMap<District, DistrictLookupDto>();

            // District -> DistrictExcelDto
            CreateMap<District, DistrictExcelDto>()
                .ForMember(dest => dest.StatusText, opt => opt.Ignore())   // Handled by MappingAction
                .ForMember(dest => dest.ProvinceName, opt => opt.Ignore()) // Handled manually in AppService before mapping
                .AfterMap<DistrictToExcelMappingAction>();

            // Province -> ProvinceLookupDto (Ensure this exists somewhere, e.g., Province module or Shared profile)
            // If it doesn't exist, add it:
            CreateMap<Province, ProvinceLookupDto>();
        }
    }