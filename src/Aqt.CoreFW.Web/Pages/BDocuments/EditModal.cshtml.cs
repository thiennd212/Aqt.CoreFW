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
using Microsoft.Extensions.Logging; // Logger
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp; // UserFriendlyException
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Content;
using Volo.Abp.Domain.Entities; // For IFileManager
using Volo.Abp.Users;
using Volo.Abp.Validation; // AbpValidationException, IValidationErrorHandler

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

    // === BEGIN ADDED PROPERTIES & SERVICES ===
    private Guid? OwnerUserId { get; set; } // Giữ lại OwnerUserId nếu logic upload cần

    // Dữ liệu chỉ đọc, dùng để render View và Partial View
    public List<ProcedureComponentDto> ProcedureComponents { get; private set; } = new(); // Danh sách components của Procedure
    // ProcedureName đã có trong BDocumentViewModel được map

    // Inject services
    private readonly IBDocumentAppService _bDocumentAppService;
    private readonly IProcedureAppService _procedureAppService; // Inject Procedure Service
    private readonly IProcedureComponentAppService _componentAppService; // Inject Component Service
    private readonly IFileAppService _fileAppService; // Inject File Service
    // === END ADDED PROPERTIES & SERVICES ===

    public EditModalModel(
        IBDocumentAppService bDocumentAppService,
        IProcedureAppService procedureAppService,
        IProcedureComponentAppService componentAppService,
        IFileAppService fileAppService,
        IFileManager fileManager,
        ICurrentUser currentUser // Inject CurrentUser
        )
    {
        _bDocumentAppService = bDocumentAppService;
        _procedureAppService = procedureAppService;
        _componentAppService = componentAppService;
        _fileAppService = fileAppService;;
        OwnerUserId = currentUser.GetId();
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
            // <<< Dữ liệu DataList (bao gồm FormData, FileName, FileSize) đã được map ở đây nhờ profile mới

            // === BEGIN SIMPLIFIED ENRICHMENT ===
            // Chỉ cần lấy definitions để enrich thông tin không có trong DTO
            var componentDefinitions = await _componentAppService.GetListByProcedureAsync(BDocumentViewModel.ProcedureId);
            ProcedureComponents = (List<ProcedureComponentDto>)componentDefinitions.Items;

            // Enrich BDocumentViewModel.DataList với thông tin component còn thiếu
            if (BDocumentViewModel.DataList != null)
            {
                foreach (var dataItem in BDocumentViewModel.DataList)
                {
                    var definition = ProcedureComponents.FirstOrDefault(d => d.Id == dataItem.ProcedureComponentId);
                    if (definition != null)
                    {
                        // Gán thông tin từ definition mà DTO không có hoặc không đủ
                        dataItem.ComponentName = definition.Name;
                        dataItem.ComponentType = definition.Type;
                        dataItem.Description = definition.Description;
                        dataItem.TempPath = definition.TempPath;
                        // dataItem.IsRequired = definition.IsRequired; // Vẫn comment

                        if (definition.Type == ComponentType.Form)
                        {
                            dataItem.FormDefinition = definition.FormDefinition; // Quan trọng cho render form
                        }
                        // Không cần lấy lại FileInfo ở đây nữa.
                        // FileName và FileSize đã được map từ BDocumentDataDto.FileInfo
                    }
                    else
                    {
                        Logger.LogWarning("Definition not found for ProcedureComponentId {ProcCompId} while enriching BDocument {BDocumentId} DataList.", dataItem.ProcedureComponentId, Id);
                        // Có thể muốn xóa item này khỏi list hoặc đánh dấu lỗi
                    }
                }
            }
            else
            {
                 Logger.LogWarning("BDocumentViewModel.DataList is null after mapping from BDocumentDto for Id {BDocumentId}. Check AppService and AutoMapper.", Id);
            }
            // === END SIMPLIFIED ENRICHMENT ===
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading BDocument data for editing. Id: {BDocumentId}", Id);
            Alerts.Danger(L["ErrorLoadingDocumentData"]);
        }
    }

    // Xử lý khi Submit Modal (POST)
    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // === BEGIN VALIDATION ===
            // Gọi validation (phần IsRequired vẫn đang comment)
            await ValidateServerSideAsync();

            if (!ModelState.IsValid)
            {
                Logger.LogWarning("Update BDocument failed due to validation errors. Id: {BDocumentId}", Id);
                // Load lại dữ liệu cần thiết cho View (FormDefinition, FileInfo) để hiển thị lại modal với lỗi
                await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId); // Dùng ProcedureId từ ViewModel
                return Page(); // Hiển thị lại modal với lỗi
            }
            // === END VALIDATION ===

            // Map từ ViewModel sang Update Input DTO
            var updateDto = ObjectMapper.Map<BDocumentViewModel, CreateUpdateBDocumentDto>(BDocumentViewModel);

            // Gọi AppService để cập nhật BDocument
            await _bDocumentAppService.UpdateAsync(Id, updateDto);

            return NoContent();
        }
        catch (AbpValidationException validationException)
        {
            Logger.LogWarning(validationException, "ABP Validation error during BDocument update. Id: {BDocumentId}", Id);
            await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId); // Load lại trước khi trả về Page
            return Page();
        }
        catch (UserFriendlyException userEx)
        {
            Logger.LogWarning(userEx, "User friendly error during BDocument update. Id: {BDocumentId}", Id);
            ModelState.AddModelError("", userEx.Message);
            await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId); // Load lại trước khi trả về Page
            return Page();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating BDocument. Id: {BDocumentId}", Id);
            ModelState.AddModelError("", L["ErrorUpdatingDocument"]);
            await ReloadDataForViewAsync(BDocumentViewModel.ProcedureId); // Load lại trước khi trả về Page
            return Page();
        }
    }

    // === BEGIN ADDED HANDLERS & METHODS ===

    // Handler Upload File (Copy từ CreateModal)
    [HttpPost]
    [IgnoreAntiforgeryToken] // Giữ lại để bỏ qua Antiforgery
    public async Task<JsonResult> OnPostUploadFileAsync(IFormFile file, Guid? parentId = null) // Sử dụng handler name mặc định
    {
        if (file == null)
        {
            Logger.LogWarning("Upload attempt failed: No file provided.");
            return new JsonResult(new { error = L["NoFileUploaded"].Value });
        }
        Logger.LogInformation("Attempting to upload file for EditModal: {FileName}, Size: {FileSize}", file.FileName, file.Length);
        try
        {
            var dto = new CreateFileWithStreamInput
            {
                FileContainerName = BDocumentConsts.FileContainerName,
                OwnerUserId = OwnerUserId, // Đã xóa
                ParentId = parentId,
            };
            dto.Content = new RemoteStreamContent(
                    stream: file.OpenReadStream(),
                    fileName: file.FileName,
                    contentType: file.ContentType);

            var result = await _fileAppService.CreateWithStreamAsync(dto);
            Logger.LogInformation("File uploaded successfully for EditModal: {FileName}, FileId: {FileId}", result.FileInfo.FileName, result.FileInfo.Id);
            var resultDto = result.FileInfo;
            return new JsonResult(resultDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "File upload failed for EditModal for file: {FileName}.", file.FileName);
            return new JsonResult(new { error = L["FileUploadFailed"].Value + (ex is UserFriendlyException ufe ? $": {ufe.Message}" : "") });
        }
    }

    // Handler Download File Template (Copy từ CreateModal - Logic vẫn là placeholder)
    public async Task<IActionResult> OnGetDownloadTemplateAsync(string templatePath)
    {
        if (string.IsNullOrEmpty(templatePath)) return NotFound("Template path is missing.");
        try
        {
            await Task.Delay(50);
            return NotFound($"Template download logic for path '{templatePath}' is not implemented yet.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error downloading template in EditModal: {TemplatePath}", templatePath);
            return NotFound($"Error occurred while trying to download template: {templatePath}");
        }
    }

    // === BEGIN ADDED HANDLER FOR UPLOADED FILE DOWNLOAD ===
    public async Task<IActionResult> OnGetDownloadUploadedFileAsync(Guid fileId)
    {
        if (fileId == Guid.Empty)
        {
            Logger.LogWarning("Download request failed: FileId is empty.");
            return NotFound("Invalid File ID.");
        }

        try
        {
            // Sử dụng IFileManager để lấy thông tin download (bao gồm cả token)
            var downloadInfo = await _fileAppService.GetDownloadInfoAsync(fileId);

            if (downloadInfo == null || string.IsNullOrEmpty(downloadInfo.DownloadUrl))
            {
                Logger.LogError("Failed to get download info or DownloadUrl is empty for FileId: {FileId}", fileId);
                 return NotFound("Could not retrieve download information.");
            }

            Logger.LogWarning("Redirecting to download URL for FileId: {FileId}", fileId);
            // DownloadUrl đã chứa đường dẫn tương đối và token, chỉ cần Redirect
            return Redirect(downloadInfo.DownloadUrl);
        }
        catch (EntityNotFoundException)
        {
            Logger.LogWarning("File not found with Id: {FileId} when trying to download.", fileId);
            return NotFound("File not found.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting download info for FileId: {FileId}", fileId);
            // Không nên trả về lỗi chi tiết cho người dùng
            return NotFound("An error occurred while preparing the file for download.");
        }
    }
    // === END ADDED HANDLER FOR UPLOADED FILE DOWNLOAD ===

    // Validate Server-side (Copy từ CreateModal)
    private async Task ValidateServerSideAsync()
    {
        Logger.LogInformation("Starting server-side validation for BDocument update. Id: {BDocumentId}", Id);
        IReadOnlyList<ProcedureComponentDto> definitions;
        try
        {
            definitions = (await _componentAppService.GetListByProcedureAsync(BDocumentViewModel.ProcedureId)).Items;
            Logger.LogDebug("Loaded {Count} component definitions for validation.", definitions.Count);
        }
        catch (Exception ex)
        {
            definitions = new List<ProcedureComponentDto>();
            Logger.LogError(ex, "Failed to get component definitions for validation. ProcedureId: {ProcedureId}", BDocumentViewModel.ProcedureId);
            ModelState.AddModelError("", L["ErrorLoadingComponentDefinitionsForValidation"]);
            return;
        }

        if (BDocumentViewModel.DataList == null || !BDocumentViewModel.DataList.Any())
        {
            Logger.LogWarning("ComponentDataList is null or empty during validation.");
            return;
        }

        for (int i = 0; i < BDocumentViewModel.DataList.Count; i++)
        {
            var dataVm = BDocumentViewModel.DataList[i];
            var definition = definitions.FirstOrDefault(d => d.Id == dataVm.ProcedureComponentId);
            if (definition == null)
            {
                Logger.LogWarning("Cannot find definition for ProcedureComponentId {ComponentId} at index {Index}.", dataVm.ProcedureComponentId, i);
                continue;
            }

            // Tạm thời comment lại do chưa có definition.IsRequired
            /*
            if (definition.IsRequired)
            {
                bool isMissing = false;
                string fieldName = string.Empty;

                if (definition.Type == ComponentType.Form)
                {
                    fieldName = $"BDocumentViewModel.DataList[{i}].FormData";
                    isMissing = string.IsNullOrWhiteSpace(dataVm.FormData) || dataVm.FormData.Trim() == "{}";
                    Logger.LogDebug("Validating Form Component (Required: {IsRequired}): Name='{ComponentName}', Index={Index}, FormData='{FormData}', IsMissing={IsMissing}",
                       definition.IsRequired, definition.Name, i, dataVm.FormData, isMissing);
                }
                else if (definition.Type == ComponentType.File)
                {
                    fieldName = $"BDocumentViewModel.DataList[{i}].FileId";
                    isMissing = !dataVm.FileId.HasValue || dataVm.FileId.Value == Guid.Empty;
                    Logger.LogDebug("Validating File Component (Required: {IsRequired}): Name='{ComponentName}', Index={Index}, FileId='{FileId}', IsMissing={IsMissing}",
                       definition.IsRequired, definition.Name, i, dataVm.FileId, isMissing);
                }

                if (isMissing)
                {
                    string errorMessage = definition.Type == ComponentType.Form
                        ? L["DeclarationFieldIsRequired", definition.Name ?? "???"].Value
                        : L["FileIsRequired", definition.Name ?? "???"].Value;
                    ModelState.AddModelError(fieldName, errorMessage);
                    Logger.LogWarning("Validation Error: Required component '{ComponentName}' (Type: {ComponentType}, Index: {Index}) is missing data.", definition.Name, definition.Type, i);
                }
            }
            */

            if (definition.Type == ComponentType.Form && !string.IsNullOrWhiteSpace(dataVm.FormData))
            {
                // ValidateJsonContent(dataVm.FormData, definition.FormDefinition, $"BDocumentViewModel.ComponentDataList[{i}]");
            }
        }
        Logger.LogInformation("Server-side validation finished for update. IsValid: {IsValid}", ModelState.IsValid);
    }

    // Reload Data For View (Copy từ CreateModal, logic tương tự)
    private async Task ReloadDataForViewAsync(Guid procedureId)
    {
        try
        {
            // Lấy lại tên Procedure (đã có trong ViewModel, không cần load lại trừ khi ProcedureId thay đổi được)
            // var procedureLookup = await _procedureAppService.GetLookupAsync();
            // BDocumentViewModel.ProcedureName = procedureLookup.Items.FirstOrDefault(p => p.Id == procedureId)?.Name ?? L["Unknown"];

            // Lấy lại danh sách components definitions
            var componentDefinitions = await _componentAppService.GetListByProcedureAsync(procedureId);
            ProcedureComponents = (List<ProcedureComponentDto>)componentDefinitions.Items;

            // Gán lại FormDefinition và thông tin file cho các item trong BDocumentViewModel.DataList
            if (BDocumentViewModel?.DataList != null)
            {
                foreach (var dataItem in BDocumentViewModel.DataList)
                {
                    var definition = ProcedureComponents.Find(d => d.Id == dataItem.ProcedureComponentId);
                    if (definition != null)
                    {
                        // Cập nhật thông tin hiển thị
                        dataItem.ComponentName = definition.Name;
                        dataItem.ComponentType = definition.Type;
                        dataItem.Description = definition.Description;
                        dataItem.IsRequired = false; // Tạm thời
                        dataItem.TempPath = definition.TempPath;
                        if (definition.Type == ComponentType.Form)
                        {
                            dataItem.FormDefinition = definition.FormDefinition; // Cần để render lại form
                        }
                        else if (definition.Type == ComponentType.File && dataItem.FileId.HasValue && string.IsNullOrEmpty(dataItem.FileName))
                        {
                            // Nếu validation fail VÀ chưa load được file name trước đó (hoặc bị mất), thử load lại
                            try
                            {
                                var fileInfo = await _fileAppService.GetAsync(dataItem.FileId.Value);
                                dataItem.FileName = fileInfo?.FileName;
                                dataItem.FileSize = fileInfo?.ByteSize;
                            }
                            catch (Exception fileEx) { /* Bỏ qua lỗi ở đây vì chỉ là reload */ Logger.LogWarning(fileEx, "Could not reload file info during ReloadDataForViewAsync for FileId {FileId}", dataItem.FileId); }
                        }
                        // Giữ nguyên dataItem.FormData và dataItem.FileId đã được bind từ lần submit lỗi trước.
                    }
                }
            }
            Logger.LogDebug("Reloaded {Count} component definitions and updated ViewModel DataList for ProcedureId {ProcedureId} in EditModal", ProcedureComponents.Count, procedureId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reloading data for EditModal after validation failed. ProcedureId: {ProcedureId}", procedureId);
            Alerts.Warning(L["ErrorReloadingViewData"]);
        }
    }

    // === END ADDED HANDLERS & METHODS ===
}