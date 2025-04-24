using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace dùng chung

// Dùng cho việc lookup DataImportant dạng danh sách phẳng (flat list) theo DataGroup
public class DataImportantLookupDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Thêm Code để dễ nhận biết
} 