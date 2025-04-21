using System;
using System.Collections.Generic; // Cần cho List<DataGroupTreeNodeDto>
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.DataGroups.Dtos;

// DTO đại diện cho một nút trong cây DataGroup
public class DataGroupTreeNodeDto : EntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; } // Thêm các trường khác nếu cần hiển thị trên cây

    // Danh sách các nút con
    public List<DataGroupTreeNodeDto> Children { get; set; }

    public DataGroupTreeNodeDto()
    {
        Children = new List<DataGroupTreeNodeDto>();
    }
} 