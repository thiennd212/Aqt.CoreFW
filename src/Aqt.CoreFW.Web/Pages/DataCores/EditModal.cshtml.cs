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

namespace Aqt.CoreFW.Web.Pages.DataCores;

public class EditModalModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public DataCoreViewModel DataCoreViewModel { get; set; } = new();

    private readonly IDataCoreAppService _dataCoreAppService;
    private readonly IDataGroupAppService _dataGroupAppService;

    public EditModalModel(
        IDataCoreAppService dataCoreAppService,
        IDataGroupAppService dataGroupAppService)
    {
        _dataCoreAppService = dataCoreAppService;
        _dataGroupAppService = dataGroupAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _dataCoreAppService.GetAsync(Id);
        DataCoreViewModel = ObjectMapper.Map<DataCoreDto, DataCoreViewModel>(dto);
        await LoadDataGroupLookupAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataGroupLookupAsync();
            return Page();
        }
        var dto = ObjectMapper.Map<DataCoreViewModel, CreateUpdateDataCoreDto>(DataCoreViewModel);
        await _dataCoreAppService.UpdateAsync(Id, dto);
        return NoContent();
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