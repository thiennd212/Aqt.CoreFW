using System.Threading.Tasks;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages; // Kế thừa từ AbpPageModel

namespace Aqt.CoreFW.Web.Pages.Communes;

public class IndexModel : AbpPageModel // Kế thừa từ AbpPageModel như kế hoạch
{
    private readonly IAuthorizationService _authorizationService;

    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnGetAsync()
    {
        // Pass permissions to the view/js via ViewData as lowercase strings
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Communes.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Communes.Delete)).ToString().ToLowerInvariant();
    }
}