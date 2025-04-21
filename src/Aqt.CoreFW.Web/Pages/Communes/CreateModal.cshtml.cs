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

    private readonly ICommuneAppService _communeAppService;

    public CreateModalModel(ICommuneAppService communeAppService)
    {
        _communeAppService = communeAppService;
        CommuneViewModel = new CommuneViewModel();
    }

    public async Task OnGetAsync()
    {
        CommuneViewModel = new CommuneViewModel { Status = CommuneStatus.Active };
        
        // Load provinces for the dropdown
        var provinceLookup = await _communeAppService.GetProvinceLookupAsync();
        CommuneViewModel.Provinces = provinceLookup.Items
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToList();

        // SRS Requirement: Select the first province if available
        if (CommuneViewModel.Provinces != null && CommuneViewModel.Provinces.Any())
        {
            var firstProvinceValue = CommuneViewModel.Provinces.First().Value;
            if (Guid.TryParse(firstProvinceValue, out Guid firstProvinceId))
            {
                CommuneViewModel.ProvinceId = firstProvinceId;
                // NOTE: No need to load districts here anymore, JS will handle it.
            }
        }
        
        // Initialize Districts list as empty (or with just a placeholder if needed by UI rendering)
        // The actual options will be loaded by JavaScript.
        CommuneViewModel.Districts = new List<SelectListItem>();
        // Example with placeholder:
        // CommuneViewModel.Districts = new List<SelectListItem> { new SelectListItem("--- Chọn Quận/Huyện ---", "") };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Basic validation and mapping
        ValidateModel();
        var dto = ObjectMapper.Map<CommuneViewModel, CreateUpdateCommuneDto>(CommuneViewModel);
        await _communeAppService.CreateAsync(dto);
        return NoContent();
    }
}