using System;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Districts;
using Aqt.CoreFW.Application.Contracts.Districts.Dtos;
using Aqt.CoreFW.Web.Pages.Districts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Using cho SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Districts;

public class EditModalModel : AbpPageModel
{
    [HiddenInput] // Input ẩn trên form
    [BindProperty(SupportsGet = true)] // Nhận giá trị Id từ query string hoặc route data
    public Guid Id { get; set; }

    [BindProperty] // Bind dữ liệu form vào ViewModel khi POST
    public DistrictViewModel DistrictViewModel { get; set; }

    private readonly IDistrictAppService _districtAppService;

    public EditModalModel(IDistrictAppService districtAppService)
    {
        _districtAppService = districtAppService;
        // Khởi tạo ViewModel trong constructor theo kế hoạch
        DistrictViewModel = new DistrictViewModel();
    }

    // Xử lý khi GET request (mở modal)
    public async Task OnGetAsync()
    {
        // Gọi AppService để lấy dữ liệu DTO của District cần sửa
        var dto = await _districtAppService.GetAsync(Id);
        // Ánh xạ từ DTO sang ViewModel để hiển thị trên form
        DistrictViewModel = ObjectMapper.Map<DistrictDto, DistrictViewModel>(dto);
        // Load danh sách Province cho dropdown
        await LoadProvinceLookupAsync();
    }

    // Xử lý khi POST request (nhấn nút Save)
    public async Task<IActionResult> OnPostAsync()
    {
        ValidateModel(); // Kiểm tra validation của ViewModel
                         // Ánh xạ từ ViewModel sang DTO để gửi lên AppService
        var dto = ObjectMapper.Map<DistrictViewModel, CreateUpdateDistrictDto>(DistrictViewModel);
        // Gọi AppService để cập nhật
        await _districtAppService.UpdateAsync(Id, dto);
        // Trả về thành công (không có nội dung) cho AJAX form
        return NoContent();
    }

    // Phương thức private để load danh sách Province
    private async Task LoadProvinceLookupAsync()
    {
        var provinceLookup = await _districtAppService.GetProvinceLookupAsync();
        // Map kết quả lookup sang SelectListItem
        DistrictViewModel.Provinces = provinceLookup.Items
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToList();
        // Lưu ý: Kế hoạch không có logic set selected item ở đây, nhưng bạn có thể thêm nếu cần:
        // .Select(p => new SelectListItem(p.Name, p.Id.ToString()){ Selected = p.Id == DistrictViewModel.ProvinceId })
    }
}