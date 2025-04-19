using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses; // AppService Interface
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // DTOs
using Aqt.CoreFW.Web.Pages.WorkflowStatuses.ViewModels; // ViewModel
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.WorkflowStatuses;

public class CreateModalModel : AbpPageModel
{
    // Bind the ViewModel to the form
    [BindProperty]
    public WorkflowStatusViewModel WorkflowStatusViewModel { get; set; }

    private readonly IWorkflowStatusAppService _workflowStatusAppService;

    public CreateModalModel(IWorkflowStatusAppService workflowStatusAppService)
    {
        _workflowStatusAppService = workflowStatusAppService;
        // Initialize the ViewModel to avoid null reference issues in the form
        WorkflowStatusViewModel = new WorkflowStatusViewModel();
    }

    public virtual void OnGet()
    {
        // Set default values when the modal is opened
        WorkflowStatusViewModel = new WorkflowStatusViewModel { IsActive = true };
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        // Validate the ViewModel based on DataAnnotations
        ValidateModel();

        // Map the ViewModel to the Create/Update DTO
        var dto = ObjectMapper.Map<WorkflowStatusViewModel, CreateUpdateWorkflowStatusDto>(WorkflowStatusViewModel);

        // Call the Application Service to create the entity
        await _workflowStatusAppService.CreateAsync(dto);

        // Indicate success without returning content (JS handles modal closing)
        return NoContent();
    }
}