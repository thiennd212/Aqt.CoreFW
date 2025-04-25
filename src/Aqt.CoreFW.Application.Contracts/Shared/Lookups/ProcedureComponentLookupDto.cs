using System;
using Aqt.CoreFW.Components; // Để lấy Enum Type nếu cần
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace dùng chung

// Dùng cho việc lookup ProcedureComponents dạng danh sách phẳng (flat list)
public class ProcedureComponentLookupDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Thêm Code để dễ nhận biết
    public ComponentType Type { get; set; } // Thêm Type để có thể lọc ở client nếu cần
} 