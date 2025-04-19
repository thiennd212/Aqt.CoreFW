using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Provinces; // Namespace AppService Interface
using Aqt.CoreFW.Application.Contracts.Provinces.Dtos; // Namespace DTOs
using Aqt.CoreFW.Web.Pages.Provinces.ViewModels; // Namespace ViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Namespace SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.Domain.Shared.Provinces; // Namespace Enum Status

namespace Aqt.CoreFW.Web.Pages.Provinces;

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public ProvinceViewModel ProvinceViewModel { get; set; }

    private readonly IProvinceAppService _provinceAppService;

    public CreateModalModel(IProvinceAppService provinceAppService)
    {
        _provinceAppService = provinceAppService;
        // Khởi tạo ViewModel để tránh null reference
        ProvinceViewModel = new ProvinceViewModel();
    }

    public async Task OnGetAsync()
    {
        // Đặt giá trị mặc định khi mở modal
        ProvinceViewModel = new ProvinceViewModel { Status = ProvinceStatus.Active };
        await LoadCountryLookupAsync(); // Load danh sách quốc gia cho dropdown
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // ValidateModel(); // ABP tự động validate dựa trên DataAnnotations của ViewModel
        var dto = ObjectMapper.Map<ProvinceViewModel, CreateUpdateProvinceDto>(ProvinceViewModel);
        await _provinceAppService.CreateAsync(dto);
        return NoContent(); // Trả về NoContent khi thành công để đóng modal
    }

    // Load danh sách quốc gia từ AppService
    private async Task LoadCountryLookupAsync()
    {
        var countryLookup = await _provinceAppService.GetCountryLookupAsync();
        ProvinceViewModel.Countries = countryLookup.Items
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();
    }
}