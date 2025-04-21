using System;
using System.Collections.Generic; // Added for List<SelectListItem>
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Communes;
using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Web.Pages.Communes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Keep for SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
// using Volo.Abp.ObjectMapping; // Not explicitly listed in plan's using section, but needed for ObjectMapper

namespace Aqt.CoreFW.Web.Pages.Communes;

public class EditModalModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)] // SupportsGet allows Id to be bound from query string
    public Guid Id { get; set; }

    [BindProperty]
    public CommuneViewModel CommuneViewModel { get; set; }

    private readonly ICommuneAppService _communeAppService;

    public EditModalModel(ICommuneAppService communeAppService)
    {
        _communeAppService = communeAppService;
        CommuneViewModel = new CommuneViewModel(); // Initialize ViewModel
    }

    public async Task OnGetAsync()
    {
        // 1. Get the existing Commune data
        var dto = await _communeAppService.GetAsync(Id);
        CommuneViewModel = ObjectMapper.Map<CommuneDto, CommuneViewModel>(dto);

        // 2. Load the Province lookup list
        var provinceLookup = await _communeAppService.GetProvinceLookupAsync();
        CommuneViewModel.Provinces = provinceLookup.Items
            .Select(p => new SelectListItem(p.Name, p.Id.ToString(), p.Id == CommuneViewModel.ProvinceId)) // Mark current province as selected
            .ToList();

        // 3. Load the initial District lookup list FOR THE CURRENT PROVINCE
        //    JavaScript will handle subsequent updates if the province changes.
        if (CommuneViewModel.ProvinceId != Guid.Empty) // Check if a province is actually selected
        {
            var districtLookup = await _communeAppService.GetDistrictLookupAsync(CommuneViewModel.ProvinceId);
            CommuneViewModel.Districts = districtLookup.Items
                .Select(d => new SelectListItem(d.Name, d.Id.ToString(), d.Id == CommuneViewModel.DistrictId)) // Mark current district as selected
                .ToList();
        }
        else
        {   
            // If no province is linked (shouldn't happen based on SRS but good practice)
            CommuneViewModel.Districts = new List<SelectListItem>();
        }
        // Example with placeholder if needed by UI rendering:
        // CommuneViewModel.Districts.Insert(0, new SelectListItem("--- Chọn Quận/Huyện ---", ""));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ValidateModel(); // Ensure model state is valid
        var dto = ObjectMapper.Map<CommuneViewModel, CreateUpdateCommuneDto>(CommuneViewModel);
        await _communeAppService.UpdateAsync(Id, dto);
        return NoContent(); // Standard success response for modals
    }
}