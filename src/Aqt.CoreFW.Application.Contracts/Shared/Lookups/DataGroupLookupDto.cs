using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups;

// Dùng cho việc lookup DataGroup dạng danh sách phẳng (flat list)
public class DataGroupLookupDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Thêm Code để dễ nhận biết
    public Guid? ParentId { get; set; } // Thêm ParentId để có thể tái tạo cây phía client nếu cần
}
