using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos; // Namespace chứa các DTOs
using Aqt.CoreFW.Domain.JobTitles.Entities;       // Namespace chứa Entity JobTitle
using AutoMapper;

namespace Aqt.CoreFW.Application.JobTitles;

/// <summary>
/// Configures AutoMapper mappings for the JobTitle module.
/// </summary>
public class JobTitleApplicationAutoMapperProfile : Profile
{
    public JobTitleApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        // Ánh xạ từ Entity JobTitle sang JobTitleDto (để hiển thị danh sách, chi tiết)
        CreateMap<JobTitle, JobTitleDto>();

        // Ánh xạ từ JobTitleDto sang CreateUpdateJobTitleDto
        // Hữu ích khi lấy dữ liệu hiện tại của JobTitle để điền vào form chỉnh sửa.
        // Giao diện người dùng thường nhận JobTitleDto, sau đó map sang CreateUpdateJobTitleDto để gửi lên server khi lưu.
        CreateMap<JobTitleDto, CreateUpdateJobTitleDto>();

        // Ánh xạ từ Entity JobTitle sang JobTitleLookupDto (để dùng trong dropdown)
        // Chỉ map những trường cần thiết (Id - có sẵn từ EntityDto, Name)
        CreateMap<JobTitle, JobTitleLookupDto>();

        // No mapping from CreateUpdateJobTitleDto to JobTitle is defined
        // as entity creation/update is handled manually in JobTitleAppService.

        // Mapping từ Entity sang Excel DTO
        CreateMap<JobTitle, JobTitleExcelDto>()
            .AfterMap<JobTitleToExcelMappingAction>(); // ✅ gọi mapping action
    }
}