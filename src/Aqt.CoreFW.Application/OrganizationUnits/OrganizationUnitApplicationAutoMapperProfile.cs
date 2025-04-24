using System;
using System.Linq;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Domain.Shared; // Use the root shared namespace
using Aqt.CoreFW.Application.OrganizationUnits.MappingActions; // Add using for mapping actions
using AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.Identity;
// using Volo.Abp.ObjectExtending; // Not needed
// Remove other incorrect/redundant usings
// using Aqt.CoreFW.Domain.Shared.OrganizationUnits; 

namespace Aqt.CoreFW.Application.OrganizationUnits;

public class OrganizationUnitApplicationAutoMapperProfile : Profile
{
    public OrganizationUnitApplicationAutoMapperProfile()
    {
        // Map from OrganizationUnit Entity to OrganizationUnitDto
        CreateMap<OrganizationUnit, OrganizationUnitDto>()
            // Use the custom mapping action
            .AfterMap<OrganizationUnitToDtoMappingAction>();

        // Map from OrganizationUnit Entity to OrganizationUnitLookupDto
        CreateMap<OrganizationUnit, OrganizationUnitLookupDto>(); // DisplayName is mapped by convention
            // Add ManualCode mapping if needed for lookup display:
            // .ForMember(dest => dest.ManualCode, opt => opt.MapFrom(src => src.GetProperty<string>(OrganizationUnitExtensionProperties.ManualCode)));

        // Map from OrganizationUnit Entity to OrganizationUnitTreeNodeDto (basic mapping)
         CreateMap<OrganizationUnit, OrganizationUnitTreeNodeDto>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
             .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.ParentId == null ? "#" : src.ParentId.ToString()))
             // Keep basic mapping for properties not handled by AfterMap
             .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.DisplayName)) // Map DisplayName first
             .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code)) // Map DisplayName first
             .ForMember(dest => dest.Children, opt => opt.Ignore()) 
             .ForMember(dest => dest.Icon, opt => opt.Ignore()) 
             .ForMember(dest => dest.State, opt => opt.Ignore())
             .ForPath(dest => dest.Data.Code, opt => opt.MapFrom(src => src.Code))
             // Use the custom mapping action for extra properties and Text adjustment
             .AfterMap<OrganizationUnitToTreeNodeDtoMappingAction>();
    }
} 