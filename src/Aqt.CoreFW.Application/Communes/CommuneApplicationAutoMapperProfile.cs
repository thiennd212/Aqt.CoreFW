using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Domain.Districts.Entities; // Assuming District entity exists
using Aqt.CoreFW.Domain.Provinces.Entities; // Assuming Province entity exists
using AutoMapper;

namespace Aqt.CoreFW.Application.Communes;

public class CommuneApplicationAutoMapperProfile : Profile
{
    public CommuneApplicationAutoMapperProfile()
    {
        CreateMap<Commune, CommuneDto>()
            .ForMember(dest => dest.ProvinceName, opt => opt.Ignore())
            .ForMember(dest => dest.DistrictName, opt => opt.Ignore());

        CreateMap<CommuneDto, CreateUpdateCommuneDto>(); // For edit form prepopulation

        CreateMap<Commune, CommuneLookupDto>();

        CreateMap<Commune, CommuneExcelDto>()
            .ForMember(dest => dest.StatusText, opt => opt.Ignore())
            .ForMember(dest => dest.ProvinceName, opt => opt.Ignore())
            .ForMember(dest => dest.DistrictName, opt => opt.Ignore())
            .AfterMap<CommuneToExcelMappingAction>();

        // Ensure these mappings exist (or define them here if not defined elsewhere)
        CreateMap<Province, ProvinceLookupDto>();
        CreateMap<District, DistrictLookupDto>();
    }
}