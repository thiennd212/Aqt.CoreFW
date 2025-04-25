using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Procedures; // Updated AppService Interface
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // Updated DTOs
using Aqt.CoreFW.Web.Pages.Procedures.ViewModels; // Updated ViewModel
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.Procedures; // Updated Enum Status

namespace Aqt.CoreFW.Web.Pages.Procedures; // Updated namespace

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public ProcedureViewModel ProcedureViewModel { get; set; } = new(); // Updated ViewModel

    private readonly IProcedureAppService _procedureAppService; // Updated AppService

    public CreateModalModel(IProcedureAppService procedureAppService)
    {
        _procedureAppService = procedureAppService;
    }

    public void OnGet() // Không cần async vì không load lookup
    {
        ProcedureViewModel = new ProcedureViewModel { Status = ProcedureStatus.Active }; // Set default with updated enum
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Không cần load lookup ở đây
            return Page();
        }
        var dto = ObjectMapper.Map<ProcedureViewModel, CreateUpdateProcedureDto>(ProcedureViewModel); // Updated map
        await _procedureAppService.CreateAsync(dto); // Updated service call
        return NoContent(); // Thành công
    }
}