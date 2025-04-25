using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace dùng chung

// Dùng cho việc lookup Procedures dạng danh sách phẳng (flat list)
public class ProcedureLookupDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Thêm Code để dễ nhận biết
} 