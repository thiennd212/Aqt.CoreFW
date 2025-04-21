using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Ranks.Dtos; // Namespace chứa DTOs
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent (xuất file)

namespace Aqt.CoreFW.Application.Contracts.Ranks // Namespace chứa Interface
{
    public interface IRankAppService :
        ICrudAppService< // Kế thừa ICrudAppService cho các thao tác CRUD cơ bản
            RankDto,                  // DTO hiển thị Rank
            Guid,                     // Kiểu khóa chính
            GetRanksInput,            // DTO cho lọc/phân trang danh sách
            CreateUpdateRankDto>      // DTO cho tạo/cập nhật
    {
        /// <summary>
        /// Lấy danh sách các rank đang hoạt động, phù hợp cho dropdown lookup (nếu cần).
        /// </summary>
        Task<ListResultDto<RankLookupDto>> GetLookupAsync(); // Phương thức lookup (tùy chọn)

        /// <summary>
        /// Xuất danh sách Rank ra file Excel dựa trên bộ lọc đầu vào.
        /// </summary>
        Task<IRemoteStreamContent> GetListAsExcelAsync(GetRanksInput input); // Phương thức xuất Excel
    }
}