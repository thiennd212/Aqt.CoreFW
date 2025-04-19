using Aqt.CoreFW.Application.Contracts.Provinces.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Using cho CountryLookupDto
using Aqt.CoreFW.Domain.Countries.Entities; // Namespace Country Entity - Kiểm tra lại nếu cần
using Aqt.CoreFW.Domain.Provinces.Entities; // Namespace Province Entity - Kiểm tra lại nếu cần
using AutoMapper;

namespace Aqt.CoreFW.Application.Provinces;

public class ProvinceApplicationAutoMapperProfile : Profile
{
    public ProvinceApplicationAutoMapperProfile()
    {
        // --- Province Mappings ---
        CreateMap<Province, ProvinceDto>()
            .ForMember(dest => dest.CountryName, opt => opt.Ignore()); // Sẽ được map thủ công trong AppService

        CreateMap<ProvinceDto, CreateUpdateProvinceDto>(); // Dùng để điền form Edit

        CreateMap<Province, ProvinceLookupDto>(); // Dùng cho dropdown chọn Province

        CreateMap<Province, ProvinceExcelDto>()
            .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // Xử lý bởi MappingAction
            .ForMember(dest => dest.CountryName, opt => opt.Ignore()) // Xử lý thủ công/context trong AppService
            .AfterMap<ProvinceToExcelMappingAction>(); // Áp dụng Mapping Action

        // --- Country Mapping (for lookup) ---
        // Đặt ở đây nếu chỉ dùng trong context Province, hoặc chuyển sang profile của Country nếu dùng chung
        CreateMap<Country, CountryLookupDto>();
    }
}