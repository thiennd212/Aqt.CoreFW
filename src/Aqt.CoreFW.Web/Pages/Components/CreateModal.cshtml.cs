using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Components; // AppService Interface
using Aqt.CoreFW.Application.Contracts.Components.Dtos; // DTOs
using Aqt.CoreFW.Application.Contracts.Procedures; // Interface Procedure AppService
using Aqt.CoreFW.Web.Pages.Components.ViewModels; // ViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.Components; // Enum

namespace Aqt.CoreFW.Web.Pages.Components;

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public ProcedureComponentViewModel ComponentViewModel { get; set; } = new();

    private readonly IProcedureComponentAppService _componentAppService;
    private readonly IProcedureAppService _procedureAppService; // Để lấy lookup Procedures

    public CreateModalModel(
        IProcedureComponentAppService componentAppService,
        IProcedureAppService procedureAppService)
    {
        _componentAppService = componentAppService;
        _procedureAppService = procedureAppService;
    }

    public async Task OnGetAsync()
    {
        ComponentViewModel = new ProcedureComponentViewModel { Status = ComponentStatus.Active }; // Giá trị mặc định
        await LoadAvailableProceduresAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        //ValidateViewModel(); // Kiểm tra logic Type/Content

        if (!ModelState.IsValid)
        {
            await LoadAvailableProceduresAsync(); // Load lại lookup khi validation fail
            return Page();
        }
        var dto = ObjectMapper.Map<ProcedureComponentViewModel, CreateUpdateProcedureComponentDto>(ComponentViewModel);
        await _componentAppService.CreateAsync(dto);
        return NoContent(); // Signal success to ModalManager
    }

    private async Task LoadAvailableProceduresAsync()
    {
        var lookupResult = await _procedureAppService.GetLookupAsync(); // Giả sử có GetLookupAsync trả về ListResultDto<ProcedureLookupDto>
        ComponentViewModel.AvailableProcedures = lookupResult.Items
            .Select(p => new SelectListItem($"{p.Code} - {p.Name}", p.Id.ToString())) // Tạo SelectListItem
            .ToList();
    }

    private void ValidateViewModel()
    {
        // Validation thủ công cho sự phụ thuộc giữa Type và FormDefinition/TempPath
        if (ComponentViewModel.Type == ComponentType.Form && string.IsNullOrWhiteSpace(ComponentViewModel.FormDefinition))
        {
            // Lấy key localization cho thông báo lỗi nếu muốn
            ModelState.AddModelError($"{nameof(ComponentViewModel)}.{nameof(ComponentViewModel.FormDefinition)}", "Form Definition is required when Type is Form.");
        }
        if (ComponentViewModel.Type == ComponentType.File && string.IsNullOrWhiteSpace(ComponentViewModel.TempPath))
        {
            ModelState.AddModelError($"{nameof(ComponentViewModel)}.{nameof(ComponentViewModel.TempPath)}", "Template Path is required when Type is File.");
        }
        // Thêm validation khác nếu cần
    }
}