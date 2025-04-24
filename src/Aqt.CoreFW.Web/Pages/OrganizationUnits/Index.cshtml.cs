using System.Threading.Tasks;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.OrganizationUnits;

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;

    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnGetAsync()
    {
        // Truyền quyền sang JavaScript qua ViewData
        ViewData["CanCreate"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.OrganizationUnits.Create)).ToString().ToLowerInvariant();
        ViewData["CanUpdate"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.OrganizationUnits.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.OrganizationUnits.Delete)).ToString().ToLowerInvariant();
        ViewData["CanMove"]   = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.OrganizationUnits.Move)).ToString().ToLowerInvariant();
    }
} 