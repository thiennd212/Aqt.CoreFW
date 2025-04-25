using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Procedures; // Consts/Enum for Procedure
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form; // Cho các attribute UI

namespace Aqt.CoreFW.Web.Pages.Procedures.ViewModels;

public class ProcedureViewModel
{
    [HiddenInput] // Id ẩn đi trên form
    public Guid? Id { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required.")] // Sử dụng thông báo lỗi mặc định hoặc key localization
    [StringLength(ProcedureConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:Procedure.Code")] // Sử dụng key localization
    public string Code { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required.")]
    [StringLength(ProcedureConsts.MaxNameLength)]
    [Display(Name = "DisplayName:Procedure.Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "The {0} field is required.")]
    [Display(Name = "DisplayName:Procedure.Order")]
    [Range(0, int.MaxValue, ErrorMessage = "Order must be a non-negative number.")] // Thêm Range nếu cần
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:Procedure.Status")]
    [SelectItems(nameof(ProcedureStatus))] // Để tạo dropdown nếu dùng tag helper Abp
    public ProcedureStatus Status { get; set; } = ProcedureStatus.Active; // Mặc định là Active

    [StringLength(ProcedureConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:Procedure.Description")]
    [TextArea(Rows = 3)] // Hiển thị dạng TextArea
    public string? Description { get; set; }

    // Các trường Sync (LastSyncedDate, SyncRecordId, SyncRecordCode) không đưa vào đây
    // vì người dùng không nhập/sửa trực tiếp qua form CRUD thông thường.
}