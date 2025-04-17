using System.Threading.Tasks; // Cần cho Task và async/await
using Aqt.CoreFW.Permissions; // Namespace chứa CoreFWPermissions
using Microsoft.AspNetCore.Authorization; // Namespace cho IAuthorizationService
using Microsoft.AspNetCore.Mvc.RazorPages; // Namespace cơ bản cho PageModel

namespace Aqt.CoreFW.Web.Pages.JobTitles;

/// <summary>
/// PageModel for the Job Title list page.
/// </summary>
public class IndexModel : PageModel // Hoặc CoreFWPageModel
{
    private readonly IAuthorizationService _authorizationService; // Inject dịch vụ Authorization

    // Constructor để inject IAuthorizationService
    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Called when the page is requested via HTTP GET.
    /// Checks Edit and Delete permissions and stores them in ViewData.
    /// </summary>
    public async Task OnGetAsync()
    {
        // Kiểm tra quyền sửa và xóa
        var canEdit = await _authorizationService.IsGrantedAsync(CoreFWPermissions.JobTitles.Edit);
        var canDelete = await _authorizationService.IsGrantedAsync(CoreFWPermissions.JobTitles.Delete);

        // Lưu kết quả (dưới dạng chuỗi "true"/"false") vào ViewData
        // để truyền sang file .cshtml và sau đó là JavaScript
        ViewData["CanEdit"] = canEdit.ToString().ToLower();
        ViewData["CanDelete"] = canDelete.ToString().ToLower();

        // Quyền Create vẫn được kiểm tra trực tiếp trong .cshtml cho nút NewJobTitleButton
        // vì nó chỉ ảnh hưởng đến việc render nút đó, không cần truyền xuống JS.
    }
}