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
using Aqt.CoreFW.DataImportants; // Enum Status

namespace Aqt.CoreFW.Web.Pages.DataImportants; // Namespace PageModel

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public DataImportantViewModel DataImportantViewModel { get; set; } = new(); // ViewModel tương ứng

    private readonly IDataImportantAppService _dataImportantAppService; // AppService tương ứng
    private readonly IDataGroupAppService _dataGroupAppService;

    public CreateModalModel(
        IDataImportantAppService dataImportantAppService,
        IDataGroupAppService dataGroupAppService)
    {
        _dataImportantAppService = dataImportantAppService;
        _dataGroupAppService = dataGroupAppService;
    }

    public async Task OnGetAsync()
    {
        DataImportantViewModel = new DataImportantViewModel { Status = DataImportantStatus.Active }; // Set default
        await LoadDataGroupLookupAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataGroupLookupAsync(); // Reload lookup nếu validation thất bại
            return Page();
        }
        var dto = ObjectMapper.Map<DataImportantViewModel, CreateUpdateDataImportantDto>(DataImportantViewModel);
        await _dataImportantAppService.CreateAsync(dto);
        return NoContent(); // Thành công
    }

    private async Task LoadDataGroupLookupAsync()
    {
        var lookup = await _dataGroupAppService.GetLookupAsync();
        DataImportantViewModel.DataGroupLookupList = lookup.Items
            .OrderBy(x => x.Name) // Sắp xếp theo tên cho dễ chọn
            .Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString()))
            .ToList();
    }
}