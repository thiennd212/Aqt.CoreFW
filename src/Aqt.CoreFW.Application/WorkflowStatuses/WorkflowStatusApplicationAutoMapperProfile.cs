using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // Namespace DTOs
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities; // Namespace Entity
// using Aqt.CoreFW.Web.Pages.WorkflowStatuses.ViewModels; // <-- XÓA USING NÀY
using AutoMapper;

namespace Aqt.CoreFW.Application.WorkflowStatuses;

/// <summary>
/// Configures AutoMapper profiles for mapping between WorkflowStatus entities and DTOs.
/// </summary>
public class WorkflowStatusApplicationAutoMapperProfile : Profile
{
    public WorkflowStatusApplicationAutoMapperProfile()
    {
        /* Define your AutoMapper configuration here for the WorkflowStatus domain. */

        // Map from Entity to the main display DTO
        CreateMap<WorkflowStatus, WorkflowStatusDto>();

        // Map from the display DTO back to the Create/Update DTO
        // Useful for populating the edit modal form from existing data
        CreateMap<WorkflowStatusDto, CreateUpdateWorkflowStatusDto>();

        // Map from Entity to the Lookup DTO used for dropdowns/selections
        CreateMap<WorkflowStatus, WorkflowStatusLookupDto>();

        // Map from Entity to the Excel DTO
        CreateMap<WorkflowStatus, WorkflowStatusExcelDto>()
                .ForMember(dest => dest.IsActiveText, opt => opt.Ignore()) // Bỏ qua mapping tự động cho IsActiveText
                .AfterMap<WorkflowStatusToExcelMappingAction>(); // Áp dụng logic tùy chỉnh sau khi mapping cơ bản

        // IMPORTANT: We do NOT map from CreateUpdateWorkflowStatusDto to WorkflowStatus entity here.
        // Entity creation and updates are handled manually within the AppService
        // to ensure correct use of the entity's constructor and methods (DDD principles).
    }
}