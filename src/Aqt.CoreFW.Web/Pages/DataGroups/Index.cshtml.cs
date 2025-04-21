using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataGroups; // App Service
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using Volo.Abp.Application.Dtos; // ListResultDto
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.DataGroups;

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IDataGroupAppService _dataGroupAppService; // Inject App Service

    public IndexModel(
        IAuthorizationService authorizationService,
        IDataGroupAppService dataGroupAppService)
    {
        _authorizationService = authorizationService;
        _dataGroupAppService = dataGroupAppService;
    }

    public async Task OnGetAsync()
    {
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.DataGroups.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.DataGroups.Delete)).ToString().ToLowerInvariant();
    }

    // Phương thức OnGetParentLookupAsync đã bị xóa.
}