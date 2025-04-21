using System;
using Volo.Abp.Application.Dtos;

// Namespace sẽ cần cập nhật theo vị trí mới
namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups
{
    // Dùng cho việc lookup Rank ở các module khác nếu cần
    public class RankLookupDto : EntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
    }
}