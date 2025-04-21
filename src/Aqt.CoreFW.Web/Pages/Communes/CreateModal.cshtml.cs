using System.Collections.Generic; // Added for List<SelectListItem>
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Communes;
using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Web.Pages.Communes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.Domain.Shared.Communes;
using System; // For CommuneStatus enum

namespace Aqt.CoreFW.Web.Pages.Communes;

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public CommuneViewModel CommuneViewModel { get; set; }
    // No explicit initialization here, done in OnGet as per plan

    private readonly ICommuneAppService _communeAppService;

    public CreateModalModel(ICommuneAppService communeAppService)
    {
        _communeAppService = communeAppService;
        // No ViewModel initialization in constructor as per plan (done in OnGet)
    }

    public async Task OnGetAsync()
    {
        // Initialize ViewModel with defaults as per plan
        CommuneViewModel = new CommuneViewModel { Status = CommuneStatus.Active, Order = 0 }; // Set default Status and Order
        await LoadProvinceLookupAsync();
        // Initialize Districts as empty list as per plan
        CommuneViewModel.Districts = new List<SelectListItem>();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // ValidateModel(); // Called implicitly by framework, but plan has it, keep for clarity if needed.
        // AbpPageModel handles ModelState validation automatically.
        var dto = ObjectMapper.Map<CommuneViewModel, CreateUpdateCommuneDto>(CommuneViewModel);
        await _communeAppService.CreateAsync(dto);
        return NoContent(); // Standard response for successful modal form submission
    }

    // Helper method to load provinces into the ViewModel's SelectList
    private async Task LoadProvinceLookupAsync()
    {
        var provinceLookup = await _communeAppService.GetProvinceLookupAsync();
        CommuneViewModel.Provinces = provinceLookup.Items
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToList();
    }

    // AJAX handler called from JavaScript when the Province dropdown changes
    // Returns a JSON list of districts for the specified provinceId
    public async Task<JsonResult> OnGetDistrictsByProvinceAsync(Guid? provinceId)
    {
        if (!provinceId.HasValue)
        {
            // Return an empty list if no province is selected
            return new JsonResult(new List<SelectListItem>());
        }
        // Fetch districts filtered by the selected provinceId
        var districtLookup = await _communeAppService.GetDistrictLookupAsync(provinceId);
        var districts = districtLookup.Items
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();
        // Return the list as JSON for the AJAX call
        return new JsonResult(districts);
    }
}