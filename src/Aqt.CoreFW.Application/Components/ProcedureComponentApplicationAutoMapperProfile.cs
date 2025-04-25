    using Aqt.CoreFW.Application.Contracts.Components.Dtos; // DTOs for Component
    using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTOs
    using Aqt.CoreFW.Domain.Components.Entities; // Component Entity
    using AutoMapper;
    using System.Linq; // For Select

    namespace Aqt.CoreFW.Application.Components; // Namespace for Component Application layer

    public class ProcedureComponentApplicationAutoMapperProfile : Profile
    {
        public ProcedureComponentApplicationAutoMapperProfile()
        {
            // --- ProcedureComponent Mappings ---

            // Entity to Read DTO
            // Note: Mapping ProcedureLinks requires the entity to be loaded with details.
            // The standard GetListAsync might not load links, GetAsync override will handle it.
            CreateMap<ProcedureComponent, ProcedureComponentDto>()
                 .ForMember(dest => dest.ProcedureIds,
                            opt => opt.MapFrom(src => src.ProcedureLinks.Select(l => l.ProcedureId).ToList()));

            // Read DTO to Create/Update DTO (for prepopulating edit forms)
            CreateMap<ProcedureComponentDto, CreateUpdateProcedureComponentDto>();

            // Entity to Lookup DTO
            CreateMap<ProcedureComponent, ProcedureComponentLookupDto>();

            // Entity to Excel DTO (if Excel Export is implemented)
            CreateMap<ProcedureComponent, ProcedureComponentExcelDto>()
                .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // Handled by MappingAction
                .ForMember(dest => dest.TypeText, opt => opt.Ignore())   // Handled by MappingAction
                .AfterMap<ComponentToExcelMappingAction>(); // Apply action for localized texts

            // IMPORTANT: No direct mapping from CreateUpdateProcedureComponentDto to ProcedureComponent entity.
            // Create/Update operations use DTO data with ProcedureComponentManager to ensure
            // business logic (like Code uniqueness, Type/Content consistency, link updates) is executed correctly.
        }
    }