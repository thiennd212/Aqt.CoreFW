    using System;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos; // Tham chiếu đến các DTOs đã tạo
    using Volo.Abp.Application.Dtos;
    using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Aqt.CoreFW.Application.Contracts.JobTitles;

    /// <summary>
    /// Defines the application service interface for managing Job Titles.
    /// Inherits from ICrudAppService to provide standard CRUD operations.
    /// </summary>
    public interface IJobTitleAppService :
        ICrudAppService< // Kế thừa ICrudAppService của ABP để có sẵn các phương thức CRUD chuẩn
            JobTitleDto,              // DTO dùng để hiển thị dữ liệu (Get, GetList)
            Guid,                     // Kiểu dữ liệu của khóa chính (Primary Key)
            GetJobTitlesInput,        // DTO dùng làm đầu vào cho phương thức GetListAsync (lọc, phân trang, sắp xếp)
            CreateUpdateJobTitleDto,  // DTO dùng làm đầu vào cho phương thức CreateAsync
            CreateUpdateJobTitleDto>  // DTO dùng làm đầu vào cho phương thức UpdateAsync (thường dùng chung DTO tạo/sửa)
                                      // Nếu cần DTO riêng cho Update, khai báo ở đây
    {
        /// <summary>
        /// Gets a list of active job titles suitable for lookup controls (e.g., dropdowns).
        /// </summary>
        /// <returns>A list of JobTitleLookupDto.</returns>
        Task<ListResultDto<JobTitleLookupDto>> GetLookupAsync();
        Task<IRemoteStreamContent> GetListAsExcelAsync(GetJobTitlesInput input);
    }