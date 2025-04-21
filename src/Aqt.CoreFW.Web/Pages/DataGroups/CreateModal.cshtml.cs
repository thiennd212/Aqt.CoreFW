using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataGroups; // Namespace IDataGroupAppService
using Aqt.CoreFW.Application.Contracts.DataGroups.Dtos; // Namespace CreateUpdateDataGroupDto
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace LookupDto
using Aqt.CoreFW.Web.Pages.DataGroups.ViewModels; // Namespace ViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Namespace SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.DataGroups; // Namespace Enum Status

namespace Aqt.CoreFW.Web.Pages.DataGroups; // Namespace PageModel

public class CreateModalModel : AbpPageModel
{
    // BindProperty để nhận dữ liệu từ form POST
    [BindProperty]
    public DataGroupViewModel DataGroupViewModel { get; set; }


    private readonly IDataGroupAppService _dataGroupAppService;

    // Inject App Service
    public CreateModalModel(IDataGroupAppService dataGroupAppService)
    {
        _dataGroupAppService = dataGroupAppService;
        // Khởi tạo ViewModel với giá trị mặc định cho Status
        DataGroupViewModel = new DataGroupViewModel { Status = DataGroupStatus.Active };
    }

    // Handler cho phương thức GET (khi modal được mở lần đầu)
    public async Task OnGetAsync()
    {
        // Khởi tạo lại ViewModel để đảm bảo trạng thái sạch
        DataGroupViewModel = new DataGroupViewModel { Status = DataGroupStatus.Active };
        // Tải danh sách Parent cho dropdown
        await LoadAvailableParentsAsync();
    }

    // Handler cho phương thức POST (khi form được submit)
    public async Task<IActionResult> OnPostAsync()
    {
        // Kiểm tra xem dữ liệu từ form có hợp lệ theo các Data Annotations không
        if (!ModelState.IsValid)
        {
            // Nếu không hợp lệ, tải lại danh sách Parent và hiển thị lại modal với lỗi validation
            await LoadAvailableParentsAsync();
            return Page();
        }

        // Ánh xạ dữ liệu từ ViewModel sang CreateUpdateDataGroupDto
        var dto = ObjectMapper.Map<DataGroupViewModel, CreateUpdateDataGroupDto>(DataGroupViewModel);
        // Gọi App Service để tạo DataGroup mới
        await _dataGroupAppService.CreateAsync(dto);
        // Trả về NoContent để đóng modal và báo thành công cho JavaScript
        return NoContent();
    }

    // Phương thức private để tải và chuẩn bị danh sách Parent
    private async Task LoadAvailableParentsAsync()
    {
        // Gọi App Service lấy danh sách lookup
        var lookupResult = await _dataGroupAppService.GetLookupAsync();
        // Chuyển đổi thành SelectListItem
        DataGroupViewModel.AvailableParents = lookupResult.Items
            .OrderBy(i => i.Name) // Sắp xếp theo tên
            .Select(i => new SelectListItem($"{i.Name} ({i.Code})", i.Id.ToString()))
            .ToList();
    }
}