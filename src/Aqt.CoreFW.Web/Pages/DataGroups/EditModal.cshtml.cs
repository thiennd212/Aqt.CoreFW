using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataGroups; // Namespace IDataGroupAppService
using Aqt.CoreFW.Application.Contracts.DataGroups.Dtos; // Namespace DTOs (DataGroupDto, CreateUpdateDataGroupDto)
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace LookupDto
using Aqt.CoreFW.Web.Pages.DataGroups.ViewModels; // Namespace ViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Namespace SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.ObjectMapping; // Namespace ObjectMapper

namespace Aqt.CoreFW.Web.Pages.DataGroups; // Namespace PageModel

public class EditModalModel : AbpPageModel
{
    // Id được bind từ query string của URL khi modal được mở (GET)
    [HiddenInput] // Thêm HiddenInput ở đây hoặc trong ViewModel đều được
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    // Bind dữ liệu từ form submit (POST) vào ViewModel
    [BindProperty]
    public DataGroupViewModel DataGroupViewModel { get; set; } = new(); // Khởi tạo để tránh null

    // Danh sách Parent hợp lệ để hiển thị trong dropdown
    public List<SelectListItem> AvailableParents { get; set; } = new();

    private readonly IDataGroupAppService _dataGroupAppService;

    // Inject App Service
    public EditModalModel(IDataGroupAppService dataGroupAppService)
    {
        _dataGroupAppService = dataGroupAppService;
    }

    // Handler cho phương thức GET (khi modal được mở)
    public async Task OnGetAsync()
    {
        // Gọi App Service để lấy DTO của DataGroup cần sửa dựa vào Id
        var dto = await _dataGroupAppService.GetAsync(Id);
        // Ánh xạ từ DTO sang ViewModel để hiển thị trên form
        DataGroupViewModel = ObjectMapper.Map<DataGroupDto, DataGroupViewModel>(dto);
        // Tải danh sách Parent hợp lệ, loại trừ chính nó và các con cháu (nếu có logic)
        await LoadAvailableParentsAsync(Id);
    }

    // Handler cho phương thức POST (khi form được submit)
    public async Task<IActionResult> OnPostAsync()
    {
        // Kiểm tra validation
        if (!ModelState.IsValid)
        {
            // Nếu lỗi, tải lại danh sách Parent và hiển thị lại modal với lỗi
            await LoadAvailableParentsAsync(Id);
            return Page();
        }

        // Ánh xạ dữ liệu đã sửa từ ViewModel sang CreateUpdateDataGroupDto
        var dto = ObjectMapper.Map<DataGroupViewModel, CreateUpdateDataGroupDto>(DataGroupViewModel);
        // Gọi App Service để cập nhật DataGroup
        await _dataGroupAppService.UpdateAsync(Id, dto);
        // Trả về NoContent để đóng modal và báo thành công
        return NoContent();
    }

    // Phương thức private để tải danh sách Parent hợp lệ
    private async Task LoadAvailableParentsAsync(Guid? currentDataGroupId = null)
    {
        // Lấy tất cả các lookup item
        var lookupResult = await _dataGroupAppService.GetLookupAsync();

        // --- Logic loại trừ Parent không hợp lệ ---
        var excludedIds = new List<Guid>();
        if (currentDataGroupId.HasValue)
        {
            // Hiện tại chỉ loại trừ chính nó.
            // TODO: Trong tương lai, cần gọi AppService để lấy danh sách ID của TẤT CẢ các con cháu (descendants)
            // của currentDataGroupId và thêm chúng vào excludedIds để ngăn chặn việc chọn cha là con cháu của chính nó (tạo vòng lặp).
            excludedIds.Add(currentDataGroupId.Value);
        }
        // --- Kết thúc logic loại trừ ---

        // Tạo danh sách SelectListItem, loại bỏ các mục không hợp lệ
        AvailableParents = lookupResult.Items
            .Where(i => !excludedIds.Contains(i.Id)) // Lọc bỏ các Id trong danh sách loại trừ
            .OrderBy(i => i.Name) // Sắp xếp
            .Select(i => new SelectListItem(
                $"{i.Name} ({i.Code})",
                i.Id.ToString(),
                // Đánh dấu selected cho ParentId hiện tại của ViewModel
                i.Id == DataGroupViewModel.ParentId
            ))
            .ToList();

        // Thêm lựa chọn "None" (Root) vào đầu danh sách
        AvailableParents.Insert(0, new SelectListItem(L["NullParentSelection"], string.Empty));
        // TODO: Đảm bảo key localization "NullParentSelection" đã được định nghĩa
    }
}