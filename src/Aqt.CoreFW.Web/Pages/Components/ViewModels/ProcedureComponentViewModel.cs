using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Components; // Consts/Enum for Component
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Aqt.CoreFW.Web.Pages.Components.ViewModels;

public class ProcedureComponentViewModel
{
    [HiddenInput]
    public Guid? Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(ComponentConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:Component.Code")]
    //[ReadOnlyInput] // Thuộc tính của ABP Tag Helper để vô hiệu hóa input khi edit
    public string Code { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(ComponentConsts.MaxNameLength)]
    [Display(Name = "DisplayName:Component.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:Component.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:Component.Status")]
    public ComponentStatus Status { get; set; } = ComponentStatus.Active;

    [StringLength(ComponentConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:Component.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "DisplayName:Component.Type")]
    [InputInfoText("Changing the type will clear the corresponding content field.")] // Thông báo tùy chọn
    public ComponentType Type { get; set; }

    [Display(Name = "DisplayName:Component.FormDefinition")]
    [TextArea(Rows = 3)] // Hiển thị dạng TextArea
    public string? FormDefinition { get; set; }

    [Display(Name = "DisplayName:Component.TempPath")]
    [StringLength(ComponentConsts.MaxTempPathLength)]
    public string? TempPath { get; set; }

    [Required] // Luôn yêu cầu có danh sách, dù là rỗng
    [Display(Name = "DisplayName:Component.Procedures")]
    [SelectItems(nameof(AvailableProcedures))]
    public List<Guid> ProcedureIds { get; set; } = new List<Guid>();

    // Property để giữ danh sách Procedures cho việc lựa chọn (không bind khi POST)
    public List<SelectListItem> AvailableProcedures { get; set; } = new List<SelectListItem>();
}