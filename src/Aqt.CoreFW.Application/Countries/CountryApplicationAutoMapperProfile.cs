using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Domain.Countries.Entities;
using AutoMapper;
using Volo.Abp.ObjectExtending; // Required for IgnoreAuditedObjectProperties - actually in Volo.Abp.AutoMapper
using Volo.Abp.AutoMapper; // Added for IgnoreAuditedObjectProperties extension method
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Application.Countries;

/// <summary>
/// Configures AutoMapper profiles for the Country entity and its DTOs.
/// </summary>
public class CountryApplicationAutoMapperProfile : Profile
{
    // Removed constructor injection of IGuidGenerator
    public CountryApplicationAutoMapperProfile()
    {
        // Mapping from Entity to standard DTO
        CreateMap<Country, CountryDto>();

        // Mapping from standard DTO back to Create/Update DTO (useful for loading Edit forms)
        CreateMap<CountryDto, CreateUpdateCountryDto>();

        // Mapping from Entity to Lookup DTO
        CreateMap<Country, CountryLookupDto>();
    }
} 