using Aqt.CoreFW.Permissions;
using Aqt.CoreFW.Web.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Aqt.CoreFW.Web.Pages.Countries;

/// <summary>
/// PageModel for the Country list page.
/// </summary>
public class IndexModel : CoreFWPageModel
{
    private readonly IAuthorizationService _authorizationService;

    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Called when the page is requested via GET.
    /// No specific logic needed here as data is loaded via AJAX.
    /// </summary>
    public async Task OnGetAsync() 
    {
        var canEdit = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Countries.Edit);
        var canDelete = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Countries.Delete);
        ViewData["CanEdit"] = canEdit.ToString().ToLower();
        ViewData["CanDelete"] = canDelete.ToString().ToLower();

    }
} 