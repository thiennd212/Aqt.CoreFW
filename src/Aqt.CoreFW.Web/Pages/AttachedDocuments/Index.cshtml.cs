using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Procedures; // Using cho IProcedureAppService (!! Giả định !!)
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Using cho ProcedureLookupDto (!! Giả định !!)
using Aqt.CoreFW.Permissions; // Using cho Permissions
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.AttachedDocuments; // Namespace của PageModel mới

public class IndexModel : AbpPageModel
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IProcedureAppService _procedureAppService; // Inject service Procedure để lấy lookup (!! Giả định !!)

    public IndexModel(
        IAuthorizationService authorizationService,
        IProcedureAppService procedureAppService) // (!! Giả định !!)
    {
        _authorizationService = authorizationService;
        _procedureAppService = procedureAppService;
    }

    public async Task OnGetAsync()
    {
        // Truyền permissions vào JS qua ViewData
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.AttachedDocuments.Update)).ToString().ToLowerInvariant();
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.AttachedDocuments.Delete)).ToString().ToLowerInvariant();
    }

    // Page Handler để cung cấp Procedure lookup cho filter dropdown trong JS
    // !! Giả định IProcedureAppService có phương thức GetLookupAsync trả về ListResultDto<ProcedureLookupDto> !!
    // !! Giả định ProcedureLookupDto có Id, Name, Code !!
    public async Task<JsonResult> OnGetProcedureLookupAsync()
    {
        var lookupResult = await _procedureAppService.GetLookupAsync(); // (!! Giả định phương thức GetLookupAsync !!)
        var selectList = lookupResult.Items
            .OrderBy(i => i.Name) // Sắp xếp theo tên
            .Select(i => new SelectListItem($"{i.Name} ({i.Code ?? ""})", i.Id.ToString())) // Hiển thị Name (Code)
            .ToList();
        return new JsonResult(selectList);
    }
}