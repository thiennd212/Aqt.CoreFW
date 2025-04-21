using System;
using System.Collections.Generic; // Added for List
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Communes;
using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Web.Pages.Communes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Added for SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
// using Volo.Abp.ObjectMapping; // Not explicitly listed in plan's using section, but needed for ObjectMapper

namespace Aqt.CoreFW.Web.Pages.Communes;

public class EditModalModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)] // Bind Id from query string on GET request
    public Guid Id { get; set; }

    [BindProperty]
    public CommuneViewModel CommuneViewModel { get; set; }
    // No explicit initialization here, done in OnGet

    private readonly ICommuneAppService _communeAppService;

    public EditModalModel(ICommuneAppService communeAppService)
    {
        _communeAppService = communeAppService;
        // No ViewModel initialization in constructor as per plan (done in OnGet)
    }

    public async Task OnGetAsync()
    {
        // Get the DTO for the specific commune
        var dto = await _communeAppService.GetAsync(Id);
        // Map the DTO to the ViewModel for editing
        CommuneViewModel = ObjectMapper.Map<CommuneDto, CommuneViewModel>(dto);
        // Load the province dropdown list
        await LoadProvinceLookupAsync();
        // Load the district dropdown list based on the commune's current province
        await LoadDistrictLookupAsync(CommuneViewModel.ProvinceId);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // ValidateModel(); // AbpPageModel handles this
        var dto = ObjectMapper.Map<CommuneViewModel, CreateUpdateCommuneDto>(CommuneViewModel);
        // Note: Code field is readonly in the form and AppService prevents its update
        await _communeAppService.UpdateAsync(Id, dto);
        return NoContent(); // Standard success response for modal
    }

    // Helper to load Province dropdown items
    private async Task LoadProvinceLookupAsync()
    {
        var provinceLookup = await _communeAppService.GetProvinceLookupAsync();
        CommuneViewModel.Provinces = provinceLookup.Items
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToList();
    }

    // Helper to load District dropdown items based on provinceId
    private async Task LoadDistrictLookupAsync(Guid? provinceId)
    {
        if (!provinceId.HasValue)
        {
            CommuneViewModel.Districts = new List<SelectListItem>(); // Empty list if no province
            return;
        }
        var districtLookup = await _communeAppService.GetDistrictLookupAsync(provinceId);
        CommuneViewModel.Districts = districtLookup.Items
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();
    }

    // AJAX Handler: Called when Province dropdown changes in the edit modal form
    // Returns a JSON list of districts for the selected provinceId
    public async Task<JsonResult> OnGetDistrictsByProvinceAsync(Guid? provinceId)
    {
        if (!provinceId.HasValue)
        {
            return new JsonResult(new List<SelectListItem>());
        }
        var districtLookup = await _communeAppService.GetDistrictLookupAsync(provinceId);
        var districts = districtLookup.Items
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();
        return new JsonResult(districts);
    }
}