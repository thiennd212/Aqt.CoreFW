using System.Threading.Tasks;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Provinces;

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;

    // Permissions will be checked and passed to the view/js
    public bool CanCreate { get; private set; }
    public bool CanEdit { get; private set; }
    public bool CanDelete { get; private set; }
    public bool CanExport { get; private set; }


    public IndexModel(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnGetAsync()
    {
        CanCreate = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Provinces.Create);
        CanEdit = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Provinces.Update);
        CanDelete = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Provinces.Delete);
        CanExport = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Provinces.Export);

        // Pass permissions to the view/js as lowercase strings for easy JS boolean conversion
        ViewData["CanEdit"] = CanEdit.ToString().ToLowerInvariant();
        ViewData["CanDelete"] = CanDelete.ToString().ToLowerInvariant();

        // CanCreate and CanExport are checked directly in the cshtml using AuthorizationService
    }
}