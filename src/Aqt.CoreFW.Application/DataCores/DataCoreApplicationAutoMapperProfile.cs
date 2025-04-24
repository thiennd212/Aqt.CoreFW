using Aqt.CoreFW.Application.Contracts.DataCores.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Domain.DataCores.Entities;
using AutoMapper;

namespace Aqt.CoreFW.Application.DataCores;

public class DataCoreApplicationAutoMapperProfile : Profile
{
    public DataCoreApplicationAutoMapperProfile()
    {
        // --- DataCore Mappings ---
        CreateMap<DataCore, DataCoreDto>()
            // Ignore DataGroupName/DataGroupCode, will be populated in AppService GetList/Get
            .ForMember(dest => dest.DataGroupName, opt => opt.Ignore())
            .ForMember(dest => dest.DataGroupCode, opt => opt.Ignore());

        // Sử dụng ReverseMap để tạo map từ DataCoreDto sang CreateUpdateDataCoreDto cho việc điền form sửa
        // Cần đảm bảo các thuộc tính khớp tên hoặc cấu hình riêng nếu cần.
        CreateMap<DataCoreDto, CreateUpdateDataCoreDto>();

        CreateMap<DataCore, DataCoreLookupDto>(); // For flat lookup lists

        CreateMap<DataCore, DataCoreExcelDto>() // If Excel Export is implemented
            .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // Handled by AppService
            .ForMember(dest => dest.DataGroupCode, opt => opt.Ignore()) // Handled by AppService
            .ForMember(dest => dest.DataGroupName, opt => opt.Ignore()); // Handled by AppService

        // No direct mapping from CreateUpdateDataCoreDto to DataCore entity
        // Create/Update operations use DTO data with DataCoreManager
    }
} 