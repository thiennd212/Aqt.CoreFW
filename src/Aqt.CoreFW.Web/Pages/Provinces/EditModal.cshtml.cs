using System;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Provinces;
using Aqt.CoreFW.Application.Contracts.Provinces.Dtos;
using Aqt.CoreFW.Web.Pages.Provinces.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Provinces;

public class EditModalModel : AbpPageModel
{
    // Id được truyền qua query string khi mở modal và bind vào đây
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public ProvinceViewModel ProvinceViewModel { get; set; }

    private readonly IProvinceAppService _provinceAppService;

    public EditModalModel(IProvinceAppService provinceAppService)
    {
        _provinceAppService = provinceAppService;
        ProvinceViewModel = new ProvinceViewModel(); // Khởi tạo
    }

    public async Task OnGetAsync()
    {
        // Lấy thông tin Province từ AppService bằng Id
        var dto = await _provinceAppService.GetAsync(Id);
        // Map DTO sang ViewModel để hiển thị trên form
        ProvinceViewModel = ObjectMapper.Map<ProvinceDto, ProvinceViewModel>(dto);
        await LoadCountryLookupAsync(); // Load danh sách quốc gia
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // ValidateModel(); // ABP tự động validate
        var dto = ObjectMapper.Map<ProvinceViewModel, CreateUpdateProvinceDto>(ProvinceViewModel);
        // Nếu Code không được sửa, cần đảm bảo không gửi giá trị mới
        // var existingDto = await _provinceAppService.GetAsync(Id);
        // dto.Code = existingDto.Code; // Giữ lại Code cũ
        await _provinceAppService.UpdateAsync(Id, dto);
        return NoContent();
    }

    // Load danh sách quốc gia từ AppService
    private async Task LoadCountryLookupAsync()
    {
        var countryLookup = await _provinceAppService.GetCountryLookupAsync();
        ProvinceViewModel.Countries = countryLookup.Items
            .Select(c => new SelectListItem(c.Name, c.Id.ToString(), ProvinceViewModel.CountryId == c.Id)) // Đánh dấu mục được chọn
            .ToList();
    }
}