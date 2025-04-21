using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Districts;
using Aqt.CoreFW.Application.Contracts.Districts.Dtos;
using Aqt.CoreFW.Web.Pages.Districts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.Domain.Shared.Districts; // Using cho DistrictStatus

namespace Aqt.CoreFW.Web.Pages.Districts;

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public DistrictViewModel DistrictViewModel { get; set; }

    private readonly IDistrictAppService _districtAppService;

    public CreateModalModel(IDistrictAppService districtAppService)
    {
        _districtAppService = districtAppService;
        // Khởi tạo ViewModel trong constructor theo kế hoạch
        DistrictViewModel = new DistrictViewModel();
    }

    public async Task OnGetAsync()
    {
        // Đặt giá trị mặc định khi mở modal
        DistrictViewModel = new DistrictViewModel { Status = DistrictStatus.Active };
        await LoadProvinceLookupAsync(); // Load danh sách Province cho dropdown
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ValidateModel(); // Kiểm tra validation của ViewModel
                         // Ánh xạ ViewModel sang DTO
        var dto = ObjectMapper.Map<DistrictViewModel, CreateUpdateDistrictDto>(DistrictViewModel);
        // Gọi AppService để tạo mới
        await _districtAppService.CreateAsync(dto);
        // Trả về thành công (không có nội dung) cho AJAX form
        return NoContent();
    }

    // Phương thức private để load danh sách Province
    private async Task LoadProvinceLookupAsync()
    {
        var provinceLookup = await _districtAppService.GetProvinceLookupAsync();
        // Map kết quả lookup sang SelectListItem để dùng trong tag helper <abp-select>
        DistrictViewModel.Provinces = provinceLookup.Items
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToList();
    }
}