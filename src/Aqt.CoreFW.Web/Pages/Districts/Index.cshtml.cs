using System.Threading.Tasks;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Microsoft.AspNetCore.Mvc; // Thêm using cho BindProperty và các thuộc tính MVC khác
using Microsoft.AspNetCore.Mvc.Rendering; // Thêm using cho SelectListItem
using System.Collections.Generic; // Thêm using cho List


namespace Aqt.CoreFW.Web.Pages.Districts;

public class IndexModel : AbpPageModel // Kế thừa AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;

    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnGetAsync()
    {
        // Truyền giá trị quyền sang ViewData dưới dạng chuỗi 'true'/'false'
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Districts.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Districts.Delete)).ToString().ToLowerInvariant();
    }
}