using System;
using Aqt.CoreFW.Ranks; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Ranks.Dtos
{
    public class GetRanksInput : PagedAndSortedResultRequestDto // Kế thừa để hỗ trợ phân trang và sắp xếp
    {
        public string? Filter { get; set; } // Filter theo Code hoặc Name
        public RankStatus? Status { get; set; } // Filter theo Status
    }
}