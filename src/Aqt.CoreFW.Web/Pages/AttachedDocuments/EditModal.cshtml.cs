using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AttachedDocuments; // AppService Interface mới
using Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos; // DTOs mới
using Aqt.CoreFW.Application.Contracts.Procedures; // Procedure AppService (!! Giả định !!)
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // ProcedureLookupDto (!! Giả định !!)
using Aqt.CoreFW.Web.Pages.AttachedDocuments.ViewModels; // ViewModel mới
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.AttachedDocuments; // Namespace PageModel mới

public class EditModalModel : AbpPageModel
{
    // Lấy Id từ route hoặc query string
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public AttachedDocumentViewModel AttachedDocumentViewModel { get; set; } = new(); // ViewModel mới

    private readonly IAttachedDocumentAppService _attachedDocumentAppService; // AppService mới
    private readonly IProcedureAppService _procedureAppService; // (!! Giả định !!)

    public EditModalModel(
        IAttachedDocumentAppService attachedDocumentAppService,
        IProcedureAppService procedureAppService) // (!! Giả định !!)
    {
        _attachedDocumentAppService = attachedDocumentAppService;
        _procedureAppService = procedureAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _attachedDocumentAppService.GetAsync(Id); // Gọi service mới để lấy dữ liệu
        AttachedDocumentViewModel = ObjectMapper.Map<AttachedDocumentDto, AttachedDocumentViewModel>(dto); // Mapping mới
        await LoadProcedureLookupAsync(); // Load lookup Procedure
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validate model trước khi xử lý
        if (!ModelState.IsValid)
        {
            await LoadProcedureLookupAsync(); // Nạp lại lookup nếu validation thất bại
            return Page(); // Trả về trang với lỗi validation
        }

        var dto = ObjectMapper.Map<AttachedDocumentViewModel, CreateUpdateAttachedDocumentDto>(AttachedDocumentViewModel); // Mapping mới
        await _attachedDocumentAppService.UpdateAsync(Id, dto); // Gọi service mới để cập nhật
        return NoContent(); // Thành công
    }

    // Hàm helper để load danh sách Procedure cho dropdown
    // !! Giả định IProcedureAppService có GetLookupAsync và ProcedureLookupDto có Id, Name, Code !!
    private async Task LoadProcedureLookupAsync()
    {
        var lookupResult = await _procedureAppService.GetLookupAsync(); // (!! Giả định !!)
        AttachedDocumentViewModel.ProcedureLookupList = lookupResult.Items
            .OrderBy(x => x.Name) // Sắp xếp theo tên
            .Select(x => new SelectListItem($"{x.Name} ({x.Code ?? ""})", x.Id.ToString())) // Hiển thị Name (Code)
            .ToList();
    }
}