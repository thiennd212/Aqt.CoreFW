using System;
using Aqt.CoreFW.Components; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Components.Dtos;

public class GetProcedureComponentsInput : PagedAndSortedResultRequestDto // Kế thừa để hỗ trợ phân trang và sắp xếp
{
    public string? Filter { get; set; } // Filter theo Code hoặc Name
    public ComponentStatus? Status { get; set; } // Filter theo Status
    public ComponentType? Type { get; set; } // Filter theo Type
    public Guid? ProcedureId { get; set; } // Filter theo Procedure liên kết
} 