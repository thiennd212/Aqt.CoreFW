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
using Aqt.CoreFW.AttachedDocuments; // Enum Status mới

namespace Aqt.CoreFW.Web.Pages.AttachedDocuments; // Namespace PageModel mới

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public AttachedDocumentViewModel AttachedDocumentViewModel { get; set; } = new(); // ViewModel mới

    private readonly IAttachedDocumentAppService _attachedDocumentAppService; // AppService mới
    private readonly IProcedureAppService _procedureAppService; // (!! Giả định !!)

    public CreateModalModel(
        IAttachedDocumentAppService attachedDocumentAppService,
        IProcedureAppService procedureAppService) // (!! Giả định !!)
    {
        _attachedDocumentAppService = attachedDocumentAppService;
        _procedureAppService = procedureAppService;
    }

    public async Task OnGetAsync()
    {
        // Khởi tạo ViewModel với giá trị mặc định
        AttachedDocumentViewModel = new AttachedDocumentViewModel { Status = AttachedDocumentStatus.Active };
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
        await _attachedDocumentAppService.CreateAsync(dto); // Gọi service mới
        return NoContent(); // Thành công, không trả về nội dung
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