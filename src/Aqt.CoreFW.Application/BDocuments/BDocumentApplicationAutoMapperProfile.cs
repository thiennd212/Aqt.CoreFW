using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos; // DTOs
// using Aqt.CoreFW.Application.Contracts.Common.Dtos; // Tạm comment, chờ xác định đúng namespace/tạo file
// using Aqt.CoreFW.Application.Contracts.Components.Dtos; // Tạm comment
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Sử dụng namespace mới cho Lookups
using Aqt.CoreFW.Domain.BDocuments.Entities; // Entities
using Aqt.CoreFW.Domain.Components.Entities;
using Aqt.CoreFW.Domain.Procedures.Entities;
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities;
using AutoMapper;
using EasyAbp.FileManagement.Files; // File Entity from FileManagement
using EasyAbp.FileManagement.Files.Dtos; // FileInfoDto from FileManagement
using System;

namespace Aqt.CoreFW.Application.BDocuments; // Đổi namespace cho phù hợp

public class BDocumentApplicationAutoMapperProfile : Profile
{
    public BDocumentApplicationAutoMapperProfile()
    {
        // --- BDocument Mappings ---
        CreateMap<BDocument, BDocumentDto>()
             .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Name : null))
             .ForMember(dest => dest.WorkflowStatusName, opt => opt.MapFrom(src => src.WorkflowStatus != null ? src.WorkflowStatus.Name : null))
             .ForMember(dest => dest.WorkflowStatusColorCode, opt => opt.MapFrom(src => src.WorkflowStatus != null ? src.WorkflowStatus.ColorCode : null));

        CreateMap<BDocument, BDocumentListDto>()
             .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Name : null))
             .ForMember(dest => dest.WorkflowStatusName, opt => opt.MapFrom(src => src.WorkflowStatus != null ? src.WorkflowStatus.Name : null))
             .ForMember(dest => dest.WorkflowStatusColorCode, opt => opt.MapFrom(src => src.WorkflowStatus != null ? src.WorkflowStatus.ColorCode : null));

        // --- BDocumentData Mapping ---
        CreateMap<BDocumentData, BDocumentDataDto>()
            .ForMember(dest => dest.ComponentCode, opt => opt.Ignore())
            .ForMember(dest => dest.ComponentName, opt => opt.Ignore())
            .ForMember(dest => dest.ComponentType, opt => opt.Ignore())
            .ForMember(dest => dest.IsRequired, opt => opt.Ignore())
            .ForMember(dest => dest.FileInfo, opt => opt.Ignore());

        // --- Excel Mapping ---
        CreateMap<BDocument, BDocumentExcelDto>()
             .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Name : null))
             .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.WorkflowStatus != null ? src.WorkflowStatus.Name : null))
             .ForMember(dest => dest.ReceiveByPost, opt => opt.MapFrom(src => src.ReceiveByPost ? "Yes" : "No"));

        CreateMap<File, FileInfoDto>();

        // --- Lookups ---
        CreateMap<Procedure, LookupDto<Guid>>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<WorkflowStatus, LookupDto<Guid>>()
             .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        // --- Component Lookup ---
        CreateMap<ProcedureComponent, ProcedureComponentLookupDto>();

        // --- Input DTOs to Entity Mappings ---
        // NO direct mapping for Create/Update DTOs to Entities.
    }
}