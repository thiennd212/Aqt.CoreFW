using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // DTOs for Procedure
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTOs
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity
using AutoMapper;

namespace Aqt.CoreFW.Application.Procedures; // Namespace for Procedure Application layer

public class ProcedureApplicationAutoMapperProfile : Profile
{
    public ProcedureApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        // --- Procedure Mappings ---
        CreateMap<Procedure, ProcedureDto>();

        // Map DTO to DTO for edit form prepopulation (optional but convenient)
        CreateMap<ProcedureDto, CreateUpdateProcedureDto>();

        // Map Entity to Lookup DTO
        CreateMap<Procedure, ProcedureLookupDto>();

        // Map Entity to Excel DTO (if Excel Export is implemented)
        CreateMap<Procedure, ProcedureExcelDto>()
            .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // StatusText được xử lý riêng
            .AfterMap<ProcedureToExcelMappingAction>(); // Áp dụng Mapping Action để xử lý StatusText

        // Quan trọng: Không map trực tiếp CreateUpdateProcedureDto -> Procedure
        // Việc tạo/cập nhật Entity sẽ thông qua ProcedureManager sử dụng dữ liệu từ DTO.
    }
}