using System;
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

public class EditModalModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)] // Lấy Id từ query string
    public Guid Id { get; set; }

    [BindProperty]
    public ProcedureComponentViewModel ComponentViewModel { get; set; } = new();

    private readonly IProcedureComponentAppService _componentAppService;
    private readonly IProcedureAppService _procedureAppService;

    public EditModalModel(
        IProcedureComponentAppService componentAppService,
        IProcedureAppService procedureAppService)
    {
        _componentAppService = componentAppService;
        _procedureAppService = procedureAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _componentAppService.GetAsync(Id); // Lấy DTO theo Id
        ComponentViewModel = ObjectMapper.Map<ProcedureComponentDto, ProcedureComponentViewModel>(dto); // Map sang ViewModel
        await LoadAvailableProceduresAsync(); // Load lookup
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
        await _componentAppService.UpdateAsync(Id, dto); // Gọi service Update
        return NoContent(); // Signal success to ModalManager
    }

    private async Task LoadAvailableProceduresAsync()
    {
        var lookupResult = await _procedureAppService.GetLookupAsync();
        ComponentViewModel.AvailableProcedures = lookupResult.Items
            .Select(p => new SelectListItem($"{p.Code} - {p.Name}", p.Id.ToString()))
            .ToList();
        // Đảm bảo các ProcedureIds hiện có của ComponentViewModel được chọn trong SelectList
        // Việc này thường được xử lý tự động bởi TagHelper nếu `asp-for="ProcedureIds"` đúng
        // Hoặc cần xử lý thủ công nếu dùng thư viện JS phức tạp hơn.
    }

    private void ValidateViewModel()
    {
        // Validation thủ công cho sự phụ thuộc giữa Type và FormDefinition/TempPath
        if (ComponentViewModel.Type == ComponentType.Form && string.IsNullOrWhiteSpace(ComponentViewModel.FormDefinition))
        {
            ModelState.AddModelError($"{nameof(ComponentViewModel)}.{nameof(ComponentViewModel.FormDefinition)}", "Form Definition is required when Type is Form.");
        }
        if (ComponentViewModel.Type == ComponentType.File && string.IsNullOrWhiteSpace(ComponentViewModel.TempPath))
        {
            ModelState.AddModelError($"{nameof(ComponentViewModel)}.{nameof(ComponentViewModel.TempPath)}", "Template Path is required when Type is File.");
        }
        // Thêm validation khác nếu cần
    }
}