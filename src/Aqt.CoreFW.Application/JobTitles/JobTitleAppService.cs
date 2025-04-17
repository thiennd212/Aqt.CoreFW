using System;
using System.Collections.Generic;
using System.Linq; // Cần cho các phương thức LINQ cơ bản
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.JobTitles;      // Interface IJobTitleAppService
using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos; // Các DTOs
using Aqt.CoreFW.Domain.JobTitles;                     // Interface IJobTitleRepository
using Aqt.CoreFW.Domain.JobTitles.Entities;            // Entity JobTitle
using Aqt.CoreFW.Localization;                         // Lớp CoreFWResource cho localization
using Aqt.CoreFW.Permissions;                          // Lớp CoreFWPermissions cho quyền hạn
using Microsoft.AspNetCore.Authorization;              // Các thuộc tính Authorize
using Volo.Abp;                                        // Check, UserFriendlyException
using Volo.Abp.Application.Dtos;                       // PagedResultDto, ListResultDto
using Volo.Abp.Application.Services;                   // CrudAppService
using Volo.Abp.Domain.Repositories;                    // IRepository
using Volo.Abp.Guids;                                  // IGuidGenerator
using System.Linq.Dynamic.Core;                        // Cần cho việc sắp xếp động (nếu dùng CreateFilteredQueryAsync)
using Volo.Abp.Domain.Entities;
using Volo.Abp.Content;
using System.IO;
using MiniExcelLibs;                        // GetEntityByIdAsync
using Volo.Abp.Timing;

namespace Aqt.CoreFW.Application.JobTitles;

/// <summary>
/// Implements the application service for managing Job Titles.
/// Handles CRUD operations, authorization, and business logic orchestration.
/// </summary>
[Authorize(CoreFWPermissions.JobTitles.Default)] // Yêu cầu quyền xem mặc định cho cả lớp
public class JobTitleAppService :
    CrudAppService<                     // Kế thừa CrudAppService chuẩn của ABP
        JobTitle,                       // Entity chính
        JobTitleDto,                    // DTO hiển thị
        Guid,                           // Kiểu khóa chính
        GetJobTitlesInput,              // DTO input cho GetList
        CreateUpdateJobTitleDto,        // DTO input cho Create
        CreateUpdateJobTitleDto>,       // DTO input cho Update
    IJobTitleAppService                 // Triển khai interface đã định nghĩa
{
    private readonly IJobTitleRepository _jobTitleRepository; // Repository tùy chỉnh
    private readonly IGuidGenerator _guidGenerator;         // Dịch vụ tạo GUID
    private readonly IClock _clock; // Inject IClock

    public JobTitleAppService(
        IRepository<JobTitle, Guid> repository,     // Repository cơ sở (được inject tự động cho CrudAppService)
        IJobTitleRepository jobTitleRepository,     // Repository tùy chỉnh (inject thêm)
        IGuidGenerator guidGenerator,
        IClock clock)               // Inject IGuidGenerator
        : base(repository)                          // Truyền repository cơ sở cho lớp cha
    {
        _jobTitleRepository = jobTitleRepository;
        _guidGenerator = guidGenerator;

        // Thiết lập Localization Resource để sử dụng hàm L(...)
        LocalizationResource = typeof(CoreFWResource);

        // Gán các Policy Name từ lớp Permissions cho CrudAppService
        // CrudAppService sẽ tự động kiểm tra các quyền này trước khi thực thi phương thức tương ứng
        // GetPolicyName = CoreFWPermissions.JobTitles.Default; // Đã đặt ở cấp độ lớp
        // GetListPolicyName = CoreFWPermissions.JobTitles.Default; // Đã đặt ở cấp độ lớp
        CreatePolicyName = CoreFWPermissions.JobTitles.Create;
        UpdatePolicyName = CoreFWPermissions.JobTitles.Edit;
        DeletePolicyName = CoreFWPermissions.JobTitles.Delete;
        _clock = clock;
    }

    /// <summary>
    /// Creates a new Job Title. Requires Create permission.
    /// </summary>
    [Authorize(CoreFWPermissions.JobTitles.Create)] // Kiểm tra quyền tạo cụ thể
    public override async Task<JobTitleDto> CreateAsync(CreateUpdateJobTitleDto input)
    {
        // 1. Kiểm tra nghiệp vụ: Mã không được trùng
        if (await _jobTitleRepository.CodeExistsAsync(input.Code))
        {
            // Ném lỗi thân thiện với người dùng, sử dụng mã lỗi và localization
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.JobTitleCodeAlreadyExists, input.Code]);
        }

        // 2. Tạo đối tượng Entity bằng constructor (đảm bảo trạng thái hợp lệ ban đầu)
        var jobTitle = new JobTitle(
            _guidGenerator.Create(), // Tạo Guid mới
            input.Code,
            input.Name,
            input.Description,
            input.IsActive
        );

        // 3. Lưu vào database thông qua repository cơ sở
        await Repository.InsertAsync(jobTitle, autoSave: true); // autoSave: true để commit ngay lập tức

        // 4. Map Entity sang DTO để trả về cho client
        return ObjectMapper.Map<JobTitle, JobTitleDto>(jobTitle);
    }

    /// <summary>
    /// Updates an existing Job Title. Requires Edit permission.
    /// </summary>
    [Authorize(CoreFWPermissions.JobTitles.Edit)] // Kiểm tra quyền sửa cụ thể
    public override async Task<JobTitleDto> UpdateAsync(Guid id, CreateUpdateJobTitleDto input)
    {
        // 1. Lấy Entity từ database bằng Id
        var jobTitle = await GetEntityByIdAsync(id); // Lấy entity, nếu không tìm thấy sẽ ném EntityNotFoundException

        // 2. Kiểm tra nghiệp vụ: Mã không được trùng với Entity khác
        // Chỉ kiểm tra nếu mã đã thay đổi
        if (jobTitle.Code != input.Code && await _jobTitleRepository.CodeExistsAsync(input.Code, excludedId: id))
        {
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.JobTitleCodeAlreadyExists, input.Code]);
        }

        // 3. Cập nhật các thuộc tính của Entity thông qua các phương thức của nó (DDD)
        jobTitle.SetCode(input.Code)       // Đảm bảo validation trong phương thức SetCode được gọi
               .SetName(input.Name)       // Đảm bảo validation trong phương thức SetName được gọi
               .SetDescription(input.Description); // Đảm bảo validation trong phương thức SetDescription được gọi

        // Cập nhật trạng thái IsActive
        if (input.IsActive)
        {
            jobTitle.Activate();
        }
        else
        {
            jobTitle.Deactivate();
        }

        // 4. Lưu thay đổi vào database thông qua repository cơ sở
        await Repository.UpdateAsync(jobTitle, autoSave: true);

        // 5. Map Entity đã cập nhật sang DTO để trả về
        return ObjectMapper.Map<JobTitle, JobTitleDto>(jobTitle);
    }

    /// <summary>
    /// Deletes a Job Title (Soft Delete). Requires Delete permission.
    /// </summary>
    [Authorize(CoreFWPermissions.JobTitles.Delete)] // Kiểm tra quyền xóa cụ thể
    public override async Task DeleteAsync(Guid id)
    {
        // --- Kiểm tra ràng buộc ---
        // TODO: Bỏ comment và triển khai logic khi module Employee tồn tại
        /*
        if (await _jobTitleRepository.HasEmployeesAsync(id))
        {
            // Lấy tên để hiển thị trong thông báo lỗi (an toàn hơn là chỉ dùng Id)
            var jobTitle = await GetEntityByIdAsync(id);
            throw new UserFriendlyException(
                L[CoreFWDomainErrorCodes.CannotDeleteJobTitleWithEmployees, jobTitle.Name ?? jobTitle.Code]
            );
        }
        */
        // --- Kết thúc kiểm tra ràng buộc ---

        // Nếu không có ràng buộc, gọi phương thức DeleteAsync của lớp cha (CrudAppService)
        // Mặc định, nó sẽ thực hiện Soft Delete nếu Entity kế thừa ISoftDelete
        await base.DeleteAsync(id);
    }

    /// <summary>
    /// Gets a list of active job titles for lookup purposes.
    /// Usually allowed for anonymous or logged-in users depending on requirements.
    /// </summary>
    [AllowAnonymous] // Hoặc [Authorize] nếu chỉ user đăng nhập mới được lấy lookup
    public async Task<ListResultDto<JobTitleLookupDto>> GetLookupAsync()
    {
        // Lấy danh sách các chức danh đang Active từ repository tùy chỉnh
        // Sắp xếp theo tên để hiển thị trong dropdown hợp lý
        var jobTitles = await _jobTitleRepository.GetListAsync(
            isActive: true, // Chỉ lấy các bản ghi active
            sorting: nameof(JobTitle.Name) + " ASC" // Sắp xếp theo tên tăng dần
                                                    // Không cần phân trang ở đây, thường lookup lấy hết active
        );

        // Map danh sách Entity sang danh sách Lookup DTO
        var lookupDtos = ObjectMapper.Map<List<JobTitle>, List<JobTitleLookupDto>>(jobTitles);

        // Trả về kết quả dưới dạng ListResultDto
        return new ListResultDto<JobTitleLookupDto>(lookupDtos);
    }

    /// <summary>
    /// Gets a paginated list of Job Titles based on input filters.
    /// Requires Default view permission (đã đặt ở cấp lớp).
    /// </summary>
    public override async Task<PagedResultDto<JobTitleDto>> GetListAsync(GetJobTitlesInput input)
    {
        // Sử dụng repository tùy chỉnh để lấy dữ liệu hiệu quả hơn

        // 1. Lấy tổng số lượng bản ghi thỏa mãn điều kiện lọc (cho phân trang)
        var totalCount = await _jobTitleRepository.GetCountAsync(
            filterText: input.Filter,
            isActive: input.IsActive
        );

        // 2. Lấy danh sách bản ghi thực tế đã được lọc, sắp xếp và phân trang
        var jobTitles = await _jobTitleRepository.GetListAsync(
            filterText: input.Filter,
            isActive: input.IsActive,
            sorting: input.Sorting, // Sử dụng sorting từ input DTO
            maxResultCount: input.MaxResultCount, // Số lượng tối đa mỗi trang
            skipCount: input.SkipCount // Số lượng bản ghi bỏ qua
        );

        // 3. Map danh sách Entity kết quả sang danh sách DTO hiển thị
        var jobTitleDtos = ObjectMapper.Map<List<JobTitle>, List<JobTitleDto>>(jobTitles);

        // 4. Trả về kết quả phân trang
        return new PagedResultDto<JobTitleDto>(totalCount, jobTitleDtos);
    }

    [Authorize(CoreFWPermissions.JobTitles.ExportExcel)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetJobTitlesInput input)
    {
        // 1. Lấy danh sách Entity phù hợp với filter
        var jobTitles = await _jobTitleRepository.GetListAsync(
            filterText: input.Filter,
            isActive: input.IsActive,
            sorting: input.Sorting,
            maxResultCount: int.MaxValue, // Lấy hết kết quả
            skipCount: 0
        );

        // 2. Map sang List<JobTitleExcelDto>
        var excelDtos = ObjectMapper.Map<List<JobTitle>, List<JobTitleExcelDto>>(jobTitles);

        // 3. Tạo MemoryStream để lưu file Excel
        var stream = new MemoryStream();

        // 4. Sử dụng MiniExcel để lưu danh sách DTO vào stream
        await stream.SaveAsAsync(excelDtos, sheetName: "JobTitles");

        // 5. Reset vị trí stream về đầu để đọc được
        stream.Seek(0, SeekOrigin.Begin);

        // 6. Tạo tên file động
        var fileName = $"JobTitles_{_clock.Now:yyyyMMdd_HHmmss}.xlsx";

        // 7. Trả về RemoteStreamContent sử dụng MimeTypeNames
        return new RemoteStreamContent(
            stream,
            fileName,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );
    }

    // Ghi chú: Vì chúng ta đã ghi đè hoàn toàn GetListAsync và sử dụng các phương thức
    // GetCountAsync, GetListAsync từ IJobTitleRepository (nơi logic lọc thực sự nằm ở tầng EF Core),
    // nên việc ghi đè CreateFilteredQueryAsync ở đây là không cần thiết và có thể bị dư thừa.
    // Nếu bạn triển khai lọc trực tiếp trong AppService bằng IQueryable, bạn mới cần CreateFilteredQueryAsync.
    // protected override async Task<IQueryable<JobTitle>> CreateFilteredQueryAsync(GetJobTitlesInput input)
    // {
    //     // Gọi base để lấy IQueryable ban đầu
    //     var query = await base.CreateFilteredQueryAsync(input);
    //     // Áp dụng các bộ lọc tùy chỉnh
    //     query = query
    //         .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
    //                  jt => jt.Code.Contains(input.Filter!) || jt.Name.Contains(input.Filter!))
    //         .WhereIf(input.IsActive.HasValue,
    //                  jt => jt.IsActive == input.IsActive!.Value);
    //     return query;
    // }
}