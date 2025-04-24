using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataImportants; // AppService Interface
using Aqt.CoreFW.Application.Contracts.DataImportants.Dtos; // DTOs
using Aqt.CoreFW.Application.Contracts.DataGroups; // DataGroup AppService
using Aqt.CoreFW.Web.Pages.DataImportants.ViewModels; // ViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.DataImportants; // Namespace PageModel

public class EditModalModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public DataImportantViewModel DataImportantViewModel { get; set; } = new(); // ViewModel tương ứng

    private readonly IDataImportantAppService _dataImportantAppService; // AppService tương ứng
    private readonly IDataGroupAppService _dataGroupAppService;

    public EditModalModel(
        IDataImportantAppService dataImportantAppService,
        IDataGroupAppService dataGroupAppService)
    {
        _dataImportantAppService = dataImportantAppService;
        _dataGroupAppService = dataGroupAppService;
    }

    public async Task OnGetAsync()
    {
        var dto = await _dataImportantAppService.GetAsync(Id);
        DataImportantViewModel = ObjectMapper.Map<DataImportantDto, DataImportantViewModel>(dto);
        await LoadDataGroupLookupAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataGroupLookupAsync();
            return Page();
        }
        var dto = ObjectMapper.Map<DataImportantViewModel, CreateUpdateDataImportantDto>(DataImportantViewModel);
        await _dataImportantAppService.UpdateAsync(Id, dto);
        return NoContent();
    }

    private async Task LoadDataGroupLookupAsync()
    {
        var lookup = await _dataGroupAppService.GetLookupAsync();
        DataImportantViewModel.DataGroupLookupList = lookup.Items
            .OrderBy(x => x.Name) // Sắp xếp theo tên
            .Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString()))
            .ToList();
    }
}