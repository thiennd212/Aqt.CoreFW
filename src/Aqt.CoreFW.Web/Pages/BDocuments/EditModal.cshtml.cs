using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.BDocuments;
using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;
using Aqt.CoreFW.Web.Pages.BDocuments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Validation; // AbpValidationException, IValidationErrorHandler
using Microsoft.Extensions.Logging; // Logger
using Volo.Abp; // UserFriendlyException

namespace Aqt.CoreFW.Web.Pages.BDocuments;

//[Authorize(CoreFWPermissions.BDocuments.Update)] // Đảm bảo có Authorize
public class EditModalModel : AbpPageModel
{
    // ID của BDocument cần sửa, lấy từ query string hoặc route
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    // ViewModel để bind dữ liệu trên form
    [BindProperty]
    public BDocumentViewModel BDocumentViewModel { get; set; } = new();

    // Inject services
    private readonly IBDocumentAppService _bDocumentAppService;

    public EditModalModel(
        IBDocumentAppService bDocumentAppService)
    {
        _bDocumentAppService = bDocumentAppService;
    }

    // Xử lý khi mở Modal (GET)
    public async Task OnGetAsync()
    {
        try
        {
            // Gọi AppService để lấy thông tin chi tiết BDocument theo Id
            var bDocumentDto = await _bDocumentAppService.GetAsync(Id);

            // Map từ DTO sang ViewModel để hiển thị lên form
            BDocumentViewModel = ObjectMapper.Map<BDocumentDto, BDocumentViewModel>(bDocumentDto);
            // Lưu ý: ComponentDataList cũng sẽ được map từ DTO, nhưng EditModal này không hiển thị/sửa chúng.
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading BDocument data for editing. Id: {BDocumentId}", Id);
            // Xử lý lỗi, ví dụ đóng modal hoặc hiển thị thông báo
            Alerts.Danger(L["ErrorLoadingDocumentData"]);
            // Thêm redirect hoặc return khác nếu cần
        }
    }

    // Xử lý khi Submit Modal (POST)
    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Không cần validate component data ở đây vì không sửa
            // Chỉ validate các trường của BDocumentViewModel (do ABP tự động làm qua DataAnnotations)

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi validation cơ bản (VD: TenChuHoSo rỗng)
                Logger.LogWarning("Update BDocument failed due to validation errors. Id: {BDocumentId}", Id);
                return Page(); // Hiển thị lại modal với lỗi
            }

            // Map từ ViewModel sang Update Input DTO
            // Chỉ map các trường cần cập nhật (AutoMapper đã cấu hình chỉ map trường chính)
            var updateDto = ObjectMapper.Map<BDocumentViewModel, UpdateBDocumentInputDto>(BDocumentViewModel);

            // Gọi AppService để cập nhật BDocument
            await _bDocumentAppService.UpdateAsync(Id, updateDto);

            // Thành công
            return NoContent();
        }
        catch (AbpValidationException validationException)
        {
            // Xử lý lỗi validation từ ABP
            Logger.LogWarning(validationException, "ABP Validation error during BDocument update. Id: {BDocumentId}", Id);            
            return Page(); // Hiển thị lại modal với lỗi
        }
        catch (UserFriendlyException userEx)
        {
            Logger.LogWarning(userEx, "User friendly error during BDocument update. Id: {BDocumentId}", Id);
            ModelState.AddModelError("", userEx.Message);
            return Page();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi không mong muốn
            Logger.LogError(ex, "Error updating BDocument. Id: {BDocumentId}", Id);
            ModelState.AddModelError("", L["ErrorUpdatingDocument"]);
            return Page(); // Hiển thị lại modal với lỗi
        }
    }
}