using System.Threading.Tasks;
using Aqt.CoreFW.Permissions; // Using cho Permissions
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Procedures; // Updated namespace

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
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Procedures.Update)).ToString().ToLowerInvariant(); // Updated permission
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.Procedures.Delete)).ToString().ToLowerInvariant(); // Updated permission
    }

    // Không cần Page Handler để lấy lookup
}