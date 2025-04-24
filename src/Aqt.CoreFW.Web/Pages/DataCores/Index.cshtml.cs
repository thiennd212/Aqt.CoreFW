using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataGroups;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.DataCores;

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IDataGroupAppService _dataGroupAppService; // Inject service to get lookup

    public IndexModel(
        IAuthorizationService authorizationService,
        IDataGroupAppService dataGroupAppService)
    {
        _authorizationService = authorizationService;
        _dataGroupAppService = dataGroupAppService;
    }

    public async Task OnGetAsync()
    {
        // Pass permissions to JS via ViewData
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.DataCores.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.DataCores.Delete)).ToString().ToLowerInvariant();
    }

    // Page Handler to provide DataGroup lookup data for the filter dropdown
    public async Task<JsonResult> OnGetDataGroupLookupAsync()
    {
        var lookupResult = await _dataGroupAppService.GetLookupAsync();
        // Map to SelectListItem format expected by JS
        var selectList = lookupResult.Items
            .OrderBy(i => i.Name) // Sort for better UX
            .Select(i => new SelectListItem($"{i.Name} ({i.Code})", i.Id.ToString()))
            .ToList();
        return new JsonResult(selectList);
    }
}