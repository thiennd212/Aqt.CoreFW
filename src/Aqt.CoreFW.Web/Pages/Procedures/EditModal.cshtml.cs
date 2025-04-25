using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Procedures; // Updated AppService Interface
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // Updated DTOs
using Aqt.CoreFW.Web.Pages.Procedures.ViewModels; // Updated ViewModel
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Procedures; // Updated namespace

public class EditModalModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public ProcedureViewModel ProcedureViewModel { get; set; } = new(); // Updated ViewModel

    private readonly IProcedureAppService _procedureAppService; // Updated AppService

    public EditModalModel(IProcedureAppService procedureAppService)
    {
        _procedureAppService = procedureAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _procedureAppService.GetAsync(Id); // Updated service call
        ProcedureViewModel = ObjectMapper.Map<ProcedureDto, ProcedureViewModel>(dto); // Updated map
                                                                                      // Không cần load lookup
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Không cần load lookup
            return Page();
        }
        var dto = ObjectMapper.Map<ProcedureViewModel, CreateUpdateProcedureDto>(ProcedureViewModel); // Updated map
        await _procedureAppService.UpdateAsync(Id, dto); // Updated service call
        return NoContent();
    }
}