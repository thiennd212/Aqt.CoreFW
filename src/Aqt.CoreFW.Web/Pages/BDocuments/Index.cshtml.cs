using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Procedures;
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // Thêm DTO Namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // Thêm DTO Namespace
using Aqt.CoreFW.Permissions; // Import Permissions
using AutoMapper.Internal.Mappers; // Không cần using này
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging; // Thêm Logger
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.ObjectMapping;

namespace Aqt.CoreFW.Web.Pages.BDocuments;

public class IndexModel : AbpPageModel // Kế thừa từ AbpPageModel để có sẵn các tiện ích
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IProcedureAppService _procedureAppService;
    private readonly IWorkflowStatusAppService _statusAppService;
    // Inject ObjectMapper để map lookup DTO sang SelectListItem (nếu không dùng AutoMapper)
    // Hoặc đảm bảo AutoMapper được cấu hình đúng

    // Các danh sách để hiển thị dropdown filter
    public List<SelectListItem> AvailableProcedures { get; set; } = new();
    public List<SelectListItem> AvailableStatuses { get; set; } = new();

    public IndexModel(
        IAuthorizationService authorizationService,
        IProcedureAppService procedureAppService,
        IWorkflowStatusAppService statusAppService)
    {
        _authorizationService = authorizationService;
        _procedureAppService = procedureAppService;
        _statusAppService = statusAppService;
    }

    public async Task OnGetAsync()
    {
        // Load dữ liệu cho các dropdown filter
        await LoadLookupsAsync();

        // Kiểm tra quyền và lưu vào ViewData để JavaScript sử dụng
        ViewData["CanEdit"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.BDocuments.Update)).ToString().ToLowerInvariant(); ;
        ViewData["CanDelete"] = (await _authorizationService.IsGrantedAsync(CoreFWPermissions.BDocuments.Delete)).ToString().ToLowerInvariant(); ;
    }

    // Hàm helper để load dữ liệu lookup từ App Services
    private async Task LoadLookupsAsync()
    {
        try
        {
            // Lấy danh sách Procedure dùng cho lookup
            var procedureLookup = await _procedureAppService.GetLookupAsync();
            // Map kết quả sang List<SelectListItem> dùng AutoMapper (đã cấu hình)
            var lstItems = (List<ProcedureLookupDto>)procedureLookup.Items;
            AvailableProcedures = ObjectMapper.Map<List<ProcedureLookupDto>, List<SelectListItem>>(lstItems);

            // Lấy danh sách WorkflowStatus dùng cho lookup
            var statusLookup = await _statusAppService.GetLookupAsync();
            // Map kết quả sang List<SelectListItem> dùng AutoMapper (đã cấu hình)
            var lstItemsStatus = (List<WorkflowStatusLookupDto>)statusLookup.Items;
            AvailableStatuses = ObjectMapper.Map<List<WorkflowStatusLookupDto>, List<SelectListItem>>(lstItemsStatus);
        }
        catch (System.Exception ex)
        {
            Logger.LogError(ex, "Error loading lookups for BDocument Index page.");
            // Có thể thêm Alert vào trang nếu cần thông báo lỗi cho người dùng
            // Alerts.Danger(L["ErrorLoadingLookupData"]);
        }
    }
}