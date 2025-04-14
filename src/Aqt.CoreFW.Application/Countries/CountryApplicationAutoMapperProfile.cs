using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Domain.Countries.Entities;
using AutoMapper;
using Volo.Abp.ObjectExtending; // Required for IgnoreAuditedObjectProperties - actually in Volo.Abp.AutoMapper
using Volo.Abp.AutoMapper; // Added for IgnoreAuditedObjectProperties extension method
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Aqt.CoreFW.Application.Countries;

/// <summary>
/// Configures AutoMapper profiles for the Country entity and its DTOs.
/// </summary>
public class CountryApplicationAutoMapperProfile : Profile
{
    // Inject IGuidGenerator via constructor
    public CountryApplicationAutoMapperProfile(IGuidGenerator guidGenerator)
    {
        // Mapping from Entity to standard DTO
        CreateMap<Country, CountryDto>();

        // Mapping from Create/Update DTO to Entity
        // Ignores base audit properties and Id (handled by ABP/EF)
        // Uses ConstructUsing to ensure a new Guid is generated for Creates
        CreateMap<CreateUpdateCountryDto, Country>()
            .IgnoreAuditedObjectProperties() // Extension method from Volo.Abp.AutoMapper
            .Ignore(x => x.Id)
            .ConstructUsing(dto => new Country(guidGenerator.Create(), dto.Code, dto.Name));

        // Mapping from standard DTO back to Create/Update DTO (useful for loading Edit forms)
        CreateMap<CountryDto, CreateUpdateCountryDto>();

        // Mapping from Entity to Lookup DTO
        CreateMap<Country, CountryLookupDto>();
    }
} 