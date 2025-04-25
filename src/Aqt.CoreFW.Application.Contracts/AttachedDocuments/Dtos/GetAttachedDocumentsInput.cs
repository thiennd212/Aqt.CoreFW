using System;
using Aqt.CoreFW.AttachedDocuments; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos;

public class GetAttachedDocumentsInput : PagedAndSortedResultRequestDto // Kế thừa để hỗ trợ phân trang và sắp xếp
{
    public string? Filter { get; set; } // Filter theo Code hoặc Name
    public AttachedDocumentStatus? Status { get; set; } // Filter theo Status
    public Guid? ProcedureId { get; set; } // Filter theo Procedure ID chính xác
} 