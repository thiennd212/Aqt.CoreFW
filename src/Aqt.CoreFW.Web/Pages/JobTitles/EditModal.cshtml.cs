using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.JobTitles;      // Interface AppService
using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos; // DTOs
using Aqt.CoreFW.Web.Pages;                           // Namespace chứa CoreFWPageModel (nếu có)
using Aqt.CoreFW.Web.Pages.JobTitles.ViewModels;      // ViewModel
using AutoMapper;                                     // Cần cho ObjectMapper nếu không dùng base class có sẵn
using Microsoft.AspNetCore.Mvc;                       // Namespace cho [HiddenInput], [BindProperty], IActionResult
using Volo.Abp.ObjectMapping;                         // Namespace cho IObjectMapper (hoặc dùng thuộc tính có sẵn)

namespace Aqt.CoreFW.Web.Pages.JobTitles;

/// <summary>
/// PageModel for the Edit Job Title modal.
/// Handles fetching existing data for the form and processing updates.
/// </summary>
public class EditModalModel : CoreFWPageModel // Hoặc kế thừa PageModel nếu không có base
{
    /// <summary>
    /// ID of the Job Title being edited. Bound from the query string on GET.
    /// </summary>
    [HiddenInput] // Không hiển thị trên form
    [BindProperty(SupportsGet = true)] // Cho phép bind giá trị Id từ query string khi gọi GET
    public Guid Id { get; set; }

    /// <summary>
    /// ViewModel bound to the form in EditModal.cshtml.
    /// </summary>
    [BindProperty]
    public JobTitleViewModel JobTitleViewModel { get; set; }

    // Inject AppService để lấy và cập nhật dữ liệu
    private readonly IJobTitleAppService _jobTitleAppService;

    // Inject IObjectMapper hoặc sử dụng thuộc tính ObjectMapper từ base class
    // private readonly IObjectMapper _objectMapper;

    public EditModalModel(IJobTitleAppService jobTitleAppService/*, IObjectMapper objectMapper*/)
    {
        _jobTitleAppService = jobTitleAppService;
        // _objectMapper = objectMapper; // Bỏ comment nếu inject thay vì dùng base
        JobTitleViewModel = new JobTitleViewModel(); // Khởi tạo để tránh null
    }

    /// <summary>
    /// Called when the modal is requested via GET.
    /// Fetches the existing Job Title data using the Id and maps it to the ViewModel.
    /// </summary>
    public async Task OnGetAsync()
    {
        // Gọi AppService để lấy DTO của JobTitle cần sửa dựa vào Id
        var jobTitleDto = await _jobTitleAppService.GetAsync(Id);

        // Map từ JobTitleDto sang JobTitleViewModel để hiển thị lên form
        // Sử dụng thuộc tính ObjectMapper kế thừa từ AbpPageModel hoặc CoreFWPageModel
        JobTitleViewModel = ObjectMapper.Map<JobTitleDto, JobTitleViewModel>(jobTitleDto);
    }

    /// <summary>
    /// Handles the POST request when the form is submitted.
    /// Maps the ViewModel back to CreateUpdateJobTitleDto and calls the UpdateAsync service.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        // Map từ JobTitleViewModel (dữ liệu đã sửa trên form) sang CreateUpdateJobTitleDto
        var dto = ObjectMapper.Map<JobTitleViewModel, CreateUpdateJobTitleDto>(JobTitleViewModel);

        // Gọi AppService để cập nhật JobTitle, truyền cả Id và DTO chứa dữ liệu mới
        await _jobTitleAppService.UpdateAsync(Id, dto);

        // Trả về NoContent để báo hiệu thành công cho request AJAX
        return NoContent();
    }
}