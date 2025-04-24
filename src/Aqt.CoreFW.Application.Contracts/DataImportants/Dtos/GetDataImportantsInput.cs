using System;
using Aqt.CoreFW.DataImportants; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.DataImportants.Dtos;

public class GetDataImportantsInput : PagedAndSortedResultRequestDto // Kế thừa để hỗ trợ phân trang và sắp xếp
{
    public string? Filter { get; set; } // Filter theo Code hoặc Name
    public DataImportantStatus? Status { get; set; } // Filter theo Status
    public Guid? DataGroupId { get; set; } // Filter theo DataGroup ID chính xác
} 