using System.Threading.Tasks;
using Aqt.CoreFW.Permissions; // Using cho Permissions
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Components; // Namespace của Page

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;

    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnGetAsync()
    {
        // Truyền permissions vào JS qua ViewData
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Components.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Components.Delete)).ToString().ToLowerInvariant();
        ViewData["CanManageLinks"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Components.ManageProcedureLinks)).ToString().ToLowerInvariant();
    }
}