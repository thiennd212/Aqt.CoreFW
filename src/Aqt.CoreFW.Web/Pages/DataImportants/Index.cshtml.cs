using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataGroups; // Using cho IDataGroupAppService
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Permissions; // Using cho Permissions
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.DataImportants; // Namespace của PageModel

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IDataGroupAppService _dataGroupAppService; // Inject service để lấy lookup

    public IndexModel(
        IAuthorizationService authorizationService,
        IDataGroupAppService dataGroupAppService)
    {
        _authorizationService = authorizationService;
        _dataGroupAppService = dataGroupAppService;
    }

    public async Task OnGetAsync()
    {
        // Truyền permissions vào JS qua ViewData
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.DataImportants.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.DataImportants.Delete)).ToString().ToLowerInvariant();
    }

    // Page Handler để cung cấp DataGroup lookup cho filter dropdown trong JS
    public async Task<JsonResult> OnGetDataGroupLookupAsync()
    {
        var lookupResult = await _dataGroupAppService.GetLookupAsync();
        // Trả về danh sách dạng { value = Guid, text = "Name (Code)" }
        var selectListItems = lookupResult.Items
            .OrderBy(i => i.Name) // Sắp xếp theo tên
            .Select(i => new { value = i.Id, text = $"{i.Name} ({i.Code})" })
            .ToList();
        return new JsonResult(selectListItems);
    }
}