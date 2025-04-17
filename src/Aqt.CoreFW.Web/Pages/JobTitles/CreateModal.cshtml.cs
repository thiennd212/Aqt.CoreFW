using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.JobTitles;
using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;
using Aqt.CoreFW.Web.Pages;
using Aqt.CoreFW.Web.Pages.JobTitles.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectMapping;

namespace Aqt.CoreFW.Web.Pages.JobTitles;

/// <summary>
/// PageModel for the Create Job Title modal.
/// Handles form submission by mapping ViewModel to DTO and calling the AppService.
/// </summary>
public class CreateModalModel : CoreFWPageModel // Hoặc kế thừa PageModel nếu không có base
{
    [BindProperty]
    public JobTitleViewModel JobTitleViewModel { get; set; }

    // Inject AppService để gọi logic tạo mới
    private readonly IJobTitleAppService _jobTitleAppService;

    // Inject IObjectMapper (hoặc sử dụng thuộc tính ObjectMapper có sẵn từ base class)

    public CreateModalModel(IJobTitleAppService jobTitleAppService)
    {
        _jobTitleAppService = jobTitleAppService;
        JobTitleViewModel = new JobTitleViewModel();
    }

    /// <summary>
    /// Initializes the ViewModel with default values for the create form.
    /// </summary>
    public void OnGet()
    {
        JobTitleViewModel = new JobTitleViewModel { IsActive = true };
    }

    /// <summary>
    /// Handles the POST request when the form is submitted.
    /// Maps the ViewModel to CreateUpdateJobTitleDto and calls the CreateAsync service.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        // Map từ ViewModel (dữ liệu từ form) sang CreateUpdateJobTitleDto
        var dto = ObjectMapper.Map<JobTitleViewModel, CreateUpdateJobTitleDto>(JobTitleViewModel);

        // Gọi phương thức CreateAsync của AppService
        await _jobTitleAppService.CreateAsync(dto);

        // Trả về NoContent để báo hiệu thành công cho request AJAX
        // JavaScript (thông qua abp framework) sẽ tự động đóng modal và reload bảng
        return NoContent();
    }
}
