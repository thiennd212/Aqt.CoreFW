using System.Threading.Tasks;
using Aqt.CoreFW.Permissions; // Namespace permissions
using Microsoft.AspNetCore.Authorization; // Namespace IAuthorizationService
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.AccountTypes; // Cập nhật namespace

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService; // Inject service

    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    // Cập nhật OnGetAsync để kiểm tra và truyền quyền cho AccountTypes
    public async Task OnGetAsync()
    {
        // Kiểm tra quyền và gán vào ViewData dưới dạng chuỗi 'true'/'false'
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.AccountTypes.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.AccountTypes.Delete)).ToString().ToLowerInvariant();
    }
}