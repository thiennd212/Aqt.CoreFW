using System;
using Aqt.CoreFW.Procedures; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Procedures.Dtos;

public class GetProceduresInput : PagedAndSortedResultRequestDto // Kế thừa để hỗ trợ phân trang và sắp xếp
{
    public string? Filter { get; set; } // Filter theo Code hoặc Name
    public ProcedureStatus? Status { get; set; } // Filter theo Status
} 