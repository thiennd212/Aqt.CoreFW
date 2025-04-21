using System;
using Aqt.CoreFW.DataGroups; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.DataGroups.Dtos;

public class GetDataGroupsInput : PagedAndSortedResultRequestDto // Kế thừa để hỗ trợ phân trang và sắp xếp
{
    public string? Filter { get; set; } // Filter theo Code hoặc Name
    public DataGroupStatus? Status { get; set; } // Filter theo Status
    public Guid? ParentId { get; set; } // Filter theo ParentId chính xác
    public bool? ParentIdIsNull { get; set; } // Lọc riêng các nhóm gốc (ParentId == null)
} 