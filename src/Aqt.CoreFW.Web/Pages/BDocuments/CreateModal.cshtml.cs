using Aqt.CoreFW.Application.Contracts.BDocuments;
using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;
using Aqt.CoreFW.Application.Contracts.Components;
using Aqt.CoreFW.Application.Contracts.Components.Dtos;
using Aqt.CoreFW.Application.Contracts.Procedures;
using Aqt.CoreFW.BDocuments; // Constants
using Aqt.CoreFW.Components; // Enum ComponentType
using Aqt.CoreFW.Web.Pages.BDocuments.ViewModels;
using EasyAbp.FileManagement.Files; // FileAppService từ EasyAbp
using EasyAbp.FileManagement.Files.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp; // UserFriendlyException
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Content;
using Volo.Abp.Users;
using Volo.Abp.Validation; // AbpValidationException, IValidationErrorHandler

namespace Aqt.CoreFW.Web.Pages.BDocuments;

//[Authorize(CoreFWPermissions.BDocuments.Create)] // Đảm bảo có Authorize nếu cần
public class CreateModalModel : AbpPageModel // Kế thừa AbpPageModel
{
    // ViewModel chính, được bind từ form
    [BindProperty]
    public BDocumentViewModel BDocumentViewModel { get; set; } = new();

    private Guid? OwnerUserId { get; set; }

    // Dữ liệu chỉ đọc, dùng để render View và Partial View
    public List<ProcedureComponentDto> ProcedureComponents { get; private set; } = new(); // Danh sách components của Procedure
    public string ProcedureName { get; private set; } = string.Empty; // Tên thủ tục để hiển thị

    // Inject các services cần thiết
    private readonly IBDocumentAppService _bDocumentAppService;
    private readonly IProcedureAppService _procedureAppService;
    private readonly IProcedureComponentAppService _componentAppService;
    private readonly IFileAppService _fileAppService; // Service từ FileManagement để upload/download

    public CreateModalModel(
        IBDocumentAppService bDocumentAppService,
        IProcedureAppService procedureAppService,
        IProcedureComponentAppService componentAppService,
        IFileAppService fileAppService,
        ICurrentUser currentUser)
    {
        _bDocumentAppService = bDocumentAppService;
        _procedureAppService = procedureAppService;
        _componentAppService = componentAppService;
        _fileAppService = fileAppService;
        OwnerUserId = currentUser.GetId();
    }

    // Xử lý khi mở Modal (GET)
    public async Task<IActionResult> OnGetAsync(Guid procedureId)
    {
        if (procedureId == Guid.Empty)
        {
            Logger.LogWarning("ProcedureId is required to open Create BDocument Modal.");
            // Có thể trả về lỗi hoặc trang trống tùy yêu cầu
            return BadRequest("ProcedureId is required.");
        }

        BDocumentViewModel.ProcedureId = procedureId; // Gán ProcedureId vào ViewModel

        try
        {
            // Load thông tin cần thiết để hiển thị modal
            await ReloadDataForViewAsync(procedureId);

            // Khởi tạo ComponentDataList trong ViewModel chính dựa trên các components của procedure
            InitializeComponentDataList(ProcedureComponents);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error preparing Create BDocument Modal for ProcedureId {ProcedureId}.", procedureId);
            // Hiển thị thông báo lỗi cho người dùng
            Alerts.Danger(L["ErrorLoadingInitialData"]);
            // Có thể đóng modal hoặc hiển thị trang lỗi
        }
        return Page();
    }

    // Xử lý khi Submit Modal (POST)
    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // 2. Validate Server-side (Kiểm tra các trường bắt buộc, file đính kèm...)
            await ValidateServerSideAsync();

            // Nếu ModelState không hợp lệ sau khi validate
            if (!ModelState.IsValid)
            {
                Logger.LogWarning("Create BDocument failed due to validation errors. ProcedureId: {ProcedureId}", BDocumentViewModel.ProcedureId);
                // Load lại dữ liệu cần thiết cho View (như FormDefinition) để hiển thị lại modal với lỗi
                await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId);
                return Page(); // Trả về Page để hiển thị lại Modal với lỗi validation
            }

            // 3. Map ViewModel sang Create Input DTO
            var createDto = ObjectMapper.Map<BDocumentViewModel, CreateUpdateBDocumentDto>(BDocumentViewModel);

            // 4. Gọi AppService để tạo BDocument
            await _bDocumentAppService.CreateAsync(createDto);

            // Thành công, không cần trả về nội dung gì vì modal sẽ tự đóng
            return NoContent();
        }
        catch (AbpValidationException validationException)
        {
            // Xử lý lỗi validation từ ABP Framework
            Logger.LogWarning(validationException, "ABP Validation error during BDocument creation.");
            await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId);
            return Page();
        }
        catch (UserFriendlyException userEx) // Bắt lỗi thân thiện với người dùng
        {
            Logger.LogWarning(userEx, "User friendly error during BDocument creation.");
            ModelState.AddModelError("", userEx.Message); // Hiển thị lỗi cho người dùng
            await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId);
            return Page();
        }
        catch (Exception ex)
        {
            // Bắt các lỗi khác không mong muốn
            Logger.LogError(ex, "Error creating BDocument for ProcedureId {ProcedureId}.", BDocumentViewModel.ProcedureId);
            ModelState.AddModelError("", L["ErrorCreatingDocument"]); // Thông báo lỗi chung
            await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId);
            return Page();
        }
    }

    // === Các Action Handler khác cho AJAX (Upload, Download) ===

    // Handler Upload File (AJAX từ createModal.js)
    [HttpPost] // Chỉ định phương thức POST
    // [Route("api/app/file/upload")] // <<< XÓA HOẶC COMMENT DÒNG NÀY
    [IgnoreAntiforgeryToken] // Giữ lại để bỏ qua Antiforgery
    public async Task<JsonResult> OnPostUploadFileAsync(IFormFile file, Guid? parentId = null)
    {
        if (file == null)
        {
            Logger.LogWarning("Upload attempt failed: No file provided.");
            return new JsonResult(new { error = L["NoFileUploaded"].Value });
        }
        Logger.LogInformation("Attempting to upload file: {FileName}, Size: {FileSize}", file.FileName, file.Length);
        try
        {
            var dto = new CreateFileWithStreamInput
            {
                FileContainerName = BDocumentConsts.FileContainerName,
                OwnerUserId = OwnerUserId,
                ParentId = parentId,
            };
            dto.Content = new RemoteStreamContent(
                    stream: file.OpenReadStream(),
                    fileName: file.FileName,
                    contentType: file.ContentType);

            var result = await _fileAppService.CreateWithStreamAsync(dto);
            Logger.LogInformation("File uploaded successfully: {FileName}, FileId: {FileId}", result.FileInfo.FileName, result.FileInfo.Id);
            // Map sang DTO của Application Contracts để trả về các thông tin cần thiết cho JS
            var resultDto = result.FileInfo;// ObjectMapper.Map<EasyAbp.FileManagement.Files.Dtos.FileInfoDto, Aqt.CoreFW.Application.Contracts.Files.FileInfoDto>(result);
            return new JsonResult(resultDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "File upload failed for file: {FileName}.", file.FileName);
            // Trả về lỗi dưới dạng JSON để JS xử lý
            return new JsonResult(new { error = L["FileUploadFailed"].Value + (ex is UserFriendlyException ufe ? $": {ufe.Message}" : "") });
        }
    }

    // Handler Download File Template (AJAX/Link từ createModal.js)
    public async Task<IActionResult> OnGetDownloadTemplateAsync(string templatePath)
    {
        if (string.IsNullOrEmpty(templatePath)) return NotFound("Template path is missing.");
        try
        {
            // --- Logic thực tế để lấy file template ---
            // Giả sử templatePath là BlobName trong container 'templates'
            // Cần inject IBlobContainer<TemplateContainer> hoặc tương tự
            // var stream = await _templateBlobContainer.GetAsync(templatePath);
            // var fileDto = await _fileAppService.FindByBlobNameAsync(templatePath, BDocumentConsts.TemplateContainerName); // Lấy thông tin file nếu cần

            // --- Placeholder ---
            await Task.Delay(50); // Giả lập độ trễ
                                  // return File(stream, fileDto?.MimeType ?? "application/octet-stream", fileDto?.FileName ?? templatePath);
            return NotFound($"Template download logic for path '{templatePath}' is not implemented yet.");
            // --- End Placeholder ---
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error downloading template: {TemplatePath}", templatePath);
            // Có thể trả về trang lỗi hoặc thông báo NotFound
            return NotFound($"Error occurred while trying to download template: {templatePath}");
        }
    }

    // === Các phương thức Helper ===

    // Validate dữ liệu phía server trước khi tạo
    private async Task ValidateServerSideAsync()
    {
        Logger.LogInformation("Starting server-side validation for BDocument creation. ProcedureId: {ProcedureId}", BDocumentViewModel.ProcedureId);
        IReadOnlyList<ProcedureComponentDto> definitions;
        try
        {
            // Lấy định nghĩa các component của procedure để kiểm tra IsRequired
            var datas = await _componentAppService.GetListByProcedureAsync(BDocumentViewModel.ProcedureId);
            definitions = datas.Items;
            Logger.LogDebug("Loaded {Count} component definitions for validation.", definitions.Count);
        }
        catch (Exception ex)
        {
            definitions = new List<ProcedureComponentDto>(); // Không có definition thì không validate được component
            Logger.LogError(ex, "Failed to get component definitions for validation. ProcedureId: {ProcedureId}", BDocumentViewModel.ProcedureId);
            ModelState.AddModelError("", L["ErrorLoadingComponentDefinitionsForValidation"]);
            return; // Dừng validation nếu không lấy được định nghĩa
        }

        // Duyệt qua danh sách component data đã được bind từ form
        if (BDocumentViewModel.DataList == null || !BDocumentViewModel.DataList.Any())
        {
            Logger.LogWarning("ComponentDataList is null or empty during validation.");
            // Có thể thêm lỗi nếu procedure yêu cầu component mà list lại rỗng
            return;
        }

        for (int i = 0; i < BDocumentViewModel.DataList.Count; i++)
        {
            var dataVm = BDocumentViewModel.DataList[i];
            // Tìm định nghĩa tương ứng
            var definition = definitions.FirstOrDefault(d => d.Id == dataVm.ProcedureComponentId);
            if (definition == null)
            {
                Logger.LogWarning("Cannot find definition for ProcedureComponentId {ComponentId} at index {Index}.", dataVm.ProcedureComponentId, i);
                continue; // Bỏ qua nếu không tìm thấy định nghĩa (bất thường)
            }

            // Kiểm tra IsRequired
            //if (definition.IsRequired)
            //{
            //    bool isMissing = false;
            //    string fieldName = string.Empty;

            //    if (definition.Type == ComponentType.Form) // Nếu là Tờ khai
            //    {
            //        fieldName = $"BDocumentViewModel.ComponentDataList[{i}].FormData";
            //        isMissing = string.IsNullOrWhiteSpace(dataVm.FormData) || dataVm.FormData.Trim() == "{}"; // Coi JSON rỗng là thiếu
            //        Logger.LogDebug("Validating Form Component (Required: {IsRequired}): Name='{ComponentName}', Index={Index}, FormData='{FormData}', IsMissing={IsMissing}",
            //           definition.IsRequired, definition.Name, i, dataVm.FormData, isMissing);
            //    }
            //    else if (definition.Type == ComponentType.File) // Nếu là File đính kèm
            //    {
            //        fieldName = $"BDocumentViewModel.ComponentDataList[{i}].FileId";
            //        isMissing = !dataVm.FileId.HasValue || dataVm.FileId.Value == Guid.Empty;
            //        Logger.LogDebug("Validating File Component (Required: {IsRequired}): Name='{ComponentName}', Index={Index}, FileId='{FileId}', IsMissing={IsMissing}",
            //           definition.IsRequired, definition.Name, i, dataVm.FileId, isMissing);
            //    }

            //    // Nếu bắt buộc mà bị thiếu, thêm lỗi vào ModelState
            //    if (isMissing)
            //    {
            //        string errorMessage = definition.Type == ComponentType.Form
            //            ? L["DeclarationFieldIsRequired", definition.Name].Value // Lỗi cụ thể cho tờ khai
            //            : L["FileIsRequired", definition.Name].Value;           // Lỗi cụ thể cho file
            //        ModelState.AddModelError(fieldName, errorMessage);
            //        Logger.LogWarning("Validation Error: Required component '{ComponentName}' (Type: {ComponentType}, Index: {Index}) is missing data.", definition.Name, definition.Type, i);
            //    }
            //}

            // TODO: Thêm logic validate cấu trúc/nội dung JSON của FormData nếu cần
            // Ví dụ: Kiểm tra các trường bắt buộc bên trong JSON tờ khai
            if (definition.Type == ComponentType.Form && !string.IsNullOrWhiteSpace(dataVm.FormData))
            {
                // ValidateJsonContent(dataVm.FormData, definition.FormDefinition, $"BDocumentViewModel.ComponentDataList[{i}]");
            }
        }
        Logger.LogInformation("Server-side validation finished. IsValid: {IsValid}", ModelState.IsValid);
    }

    // Nạp lại dữ liệu cần thiết cho View khi validation thất bại hoặc có lỗi
    private async Task ReloadDataForViewAsync(Guid procedureId)
    {
        try
        {
            // Lấy lại tên Procedure
            var procedureLookup = await _procedureAppService.GetLookupAsync();
            ProcedureName = procedureLookup.Items.FirstOrDefault(p => p.Id == procedureId)?.Name ?? L["Unknown"];

            // Lấy lại danh sách components
            var definitions = await _componentAppService.GetListByProcedureAsync(procedureId);
            ProcedureComponents = (List<ProcedureComponentDto>)definitions.Items;

            // Quan trọng: Gán lại FormDefinition và các thông tin hiển thị khác
            // cho các item trong BDocumentViewModel.DataList đã được bind từ lần submit lỗi trước.
            if (BDocumentViewModel?.DataList != null)
            {
                foreach(var dataItem in BDocumentViewModel.DataList)
                {
                    // Tìm definition tương ứng trong danh sách vừa load lại
                    var definition = ProcedureComponents.Find(d => d.Id == dataItem.ProcedureComponentId);
                    if(definition != null)
                    {
                        // Cập nhật thông tin hiển thị và definition
                        dataItem.ComponentName = definition.Name;
                        dataItem.ComponentType = definition.Type;
                        dataItem.Description = definition.Description;
                        dataItem.IsRequired = false;
                        dataItem.TempPath = definition.TempPath;
                        if(definition.Type == ComponentType.Form)
                        { 
                           dataItem.FormDefinition = definition.FormDefinition; // Cần để render lại form 
                        }
                        // Giữ nguyên dataItem.FormData và dataItem.FileId đã được bind
                    }
                }
            }
            Logger.LogDebug("Reloaded {Count} component definitions and updated ViewModel DataList for ProcedureId {ProcedureId}", ProcedureComponents.Count, procedureId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reloading data for CreateModal after validation failed. ProcedureId: {ProcedureId}", procedureId);
            // Nếu lỗi ở đây, modal có thể không hiển thị đúng
            Alerts.Warning(L["ErrorReloadingViewData"]);
        }
    }

    // Khởi tạo ComponentDataList trong ViewModel chính
    private void InitializeComponentDataList(List<ProcedureComponentDto> components)
    {
        BDocumentViewModel.DataList = new List<BDocumentDataViewModel>();
        if (components == null) return;

        foreach (var comp in components)
        {
            BDocumentViewModel.DataList.Add(new BDocumentDataViewModel
            {
                // Gán các giá trị ban đầu cần thiết cho View
                ProcedureComponentId = comp.Id,
                ComponentName = comp.Name,
                ComponentType = comp.Type,
                Description = comp.Description,
                IsRequired = false,
                TempPath = comp.TempPath,
                FormDefinition = comp.Type == ComponentType.Form ? comp.FormDefinition : null // Chỉ gán definition cho form
                // FormData và FileId sẽ được bind từ form khi submit
            });
        }
        Logger.LogDebug("Initialized ComponentDataList with {Count} items based on procedure components.", BDocumentViewModel.DataList.Count);
    }

    // Optional: Helper để validate nội dung JSON
    // private void ValidateJsonContent(string? jsonData, string? formDefinition, string parentFieldName)
    // {
    //     if (string.IsNullOrWhiteSpace(jsonData) || string.IsNullOrWhiteSpace(formDefinition)) return;
    //     try { /* Parse JSON definition và data, kiểm tra các trường required bên trong JSON */ }
    //     catch (Exception ex) { Logger.LogError(ex, "Error validating JSON content."); }
    // }
}