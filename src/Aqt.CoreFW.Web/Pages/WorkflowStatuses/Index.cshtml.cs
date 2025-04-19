using System.Threading.Tasks;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.WorkflowStatuses;

// No need to explicitly authorize the page model if the menu item requires the permission.
// However, adding it provides an extra layer of defense.
// [Authorize(CoreFWPermissions.WorkflowStatuses.Default)]
public class IndexModel : AbpPageModel // Or your custom base PageModel if you have one
{
    private readonly IAuthorizationService _authorizationService;

    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnGetAsync()
    {
        // Check specific permissions needed by the UI
        var canEdit = await _authorizationService.IsGrantedAsync(CoreFWPermissions.WorkflowStatuses.Edit);
        var canDelete = await _authorizationService.IsGrantedAsync(CoreFWPermissions.WorkflowStatuses.Delete);

        // Pass permissions down to the view/js as lowercase strings ("true"/"false")
        ViewData["CanEdit"] = canEdit.ToString().ToLower();
        ViewData["CanDelete"] = canDelete.ToString().ToLower();
    }
}