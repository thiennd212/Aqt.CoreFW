using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses; // AppService Interface
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // DTOs
using Aqt.CoreFW.Web.Pages.WorkflowStatuses.ViewModels; // ViewModel
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
// using Volo.Abp.ObjectMapping; // No longer explicitly needed if using base ObjectMapper

namespace Aqt.CoreFW.Web.Pages.WorkflowStatuses;

public class EditModalModel : AbpPageModel
{
    // Hidden input bound from the query string on GET
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    // Bind the ViewModel to the form for editing
    [BindProperty]
    public WorkflowStatusViewModel WorkflowStatusViewModel { get; set; }

    private readonly IWorkflowStatusAppService _workflowStatusAppService;

    public EditModalModel(IWorkflowStatusAppService workflowStatusAppService)
    {
        _workflowStatusAppService = workflowStatusAppService;
        // Initialize ViewModel to avoid null issues, although OnGet will overwrite it
        WorkflowStatusViewModel = new WorkflowStatusViewModel();
    }

    public virtual async Task OnGetAsync()
    {
        // Fetch the data for the specific WorkflowStatus
        var dto = await _workflowStatusAppService.GetAsync(Id);
        // Map the DTO received from the AppService to the ViewModel used by the form
        WorkflowStatusViewModel = ObjectMapper.Map<WorkflowStatusDto, WorkflowStatusViewModel>(dto);
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        // Validate the submitted form data against the ViewModel's annotations
        ValidateModel();

        // Map the edited ViewModel back to the CreateUpdate DTO
        var dto = ObjectMapper.Map<WorkflowStatusViewModel, CreateUpdateWorkflowStatusDto>(WorkflowStatusViewModel);

        // Call the Application Service to update the entity
        await _workflowStatusAppService.UpdateAsync(Id, dto);

        // Indicate success
        return NoContent();
    }
}