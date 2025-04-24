using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataCores;
using Aqt.CoreFW.Application.Contracts.DataCores.Dtos;
using Aqt.CoreFW.Application.Contracts.DataGroups;
using Aqt.CoreFW.Web.Pages.DataCores.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.DataCores;

namespace Aqt.CoreFW.Web.Pages.DataCores;

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public DataCoreViewModel DataCoreViewModel { get; set; } = new();

    private readonly IDataCoreAppService _dataCoreAppService;
    private readonly IDataGroupAppService _dataGroupAppService;

    public CreateModalModel(
        IDataCoreAppService dataCoreAppService,
        IDataGroupAppService dataGroupAppService)
    {
        _dataCoreAppService = dataCoreAppService;
        _dataGroupAppService = dataGroupAppService;
    }

    public async Task OnGetAsync()
    {
        DataCoreViewModel = new DataCoreViewModel { Status = DataCoreStatus.Active }; // Set default
        await LoadDataGroupLookupAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataGroupLookupAsync(); // Reload lookup if validation fails
            return Page(); // Re-render modal with errors
        }
        var dto = ObjectMapper.Map<DataCoreViewModel, CreateUpdateDataCoreDto>(DataCoreViewModel);
        await _dataCoreAppService.CreateAsync(dto);
        return NoContent(); // Success
    }

    private async Task LoadDataGroupLookupAsync()
    {
        var lookup = await _dataGroupAppService.GetLookupAsync();
        DataCoreViewModel.DataGroupLookupList = lookup.Items
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString()))
            .ToList();
    }
}