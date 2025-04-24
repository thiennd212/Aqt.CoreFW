using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.DataImportants; // Consts/Enum for DataImportant
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Aqt.CoreFW.Web.Pages.DataImportants.ViewModels;

public class DataImportantViewModel
{
    [HiddenInput]
    public Guid? Id { get; set; }

    [Required]
    [StringLength(DataImportantConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:DataImportant.Code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(DataImportantConsts.MaxNameLength)]
    [Display(Name = "DisplayName:DataImportant.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:DataImportant.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:DataImportant.Status")]
    public DataImportantStatus Status { get; set; } = DataImportantStatus.Active;

    [StringLength(DataImportantConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:DataImportant.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Validation:DataGroupIdRequired")] // Sử dụng key localization chung nếu có
    [Display(Name = "DisplayName:DataImportant.DataGroupId")]
    [SelectItems(nameof(DataGroupLookupList))] // Thuộc tính chứa danh sách SelectListItem
    public Guid DataGroupId { get; set; }

    // Dùng để populate dropdown DataGroup
    public List<SelectListItem>? DataGroupLookupList { get; set; }
}