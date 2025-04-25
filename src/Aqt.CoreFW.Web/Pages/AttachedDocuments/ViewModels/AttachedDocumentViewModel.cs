using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.AttachedDocuments; // Consts/Enum for AttachedDocument
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Aqt.CoreFW.Web.Pages.AttachedDocuments.ViewModels;

public class AttachedDocumentViewModel
{
    [HiddenInput]
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "DisplayName:AttachedDocument.Code")]
    [StringLength(AttachedDocumentConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:AttachedDocument.Name")]
    [StringLength(AttachedDocumentConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:AttachedDocument.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:AttachedDocument.Status")]
    public AttachedDocumentStatus Status { get; set; } = AttachedDocumentStatus.Active;

    [Display(Name = "DisplayName:AttachedDocument.Description")]
    [StringLength(AttachedDocumentConsts.MaxDescriptionLength)]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Validation:ProcedureIdRequired")] // Sử dụng key localization mới
    [Display(Name = "DisplayName:AttachedDocument.ProcedureId")]
    [SelectItems(nameof(ProcedureLookupList))] // Thuộc tính chứa danh sách SelectListItem
    public Guid ProcedureId { get; set; }

    // Dùng để đổ dữ liệu vào dropdown Procedure
    public List<SelectListItem>? ProcedureLookupList { get; set; }
}