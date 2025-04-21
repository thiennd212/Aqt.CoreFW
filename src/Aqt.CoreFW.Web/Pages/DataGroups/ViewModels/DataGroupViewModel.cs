using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.DataGroups; // Cần using namespace chứa Enum/Consts
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

// *** Namespace đã được cập nhật theo vị trí file mới ***
namespace Aqt.CoreFW.Web.Pages.DataGroups.ViewModels;

public class DataGroupViewModel
{
    [HiddenInput]
    public Guid Id { get; set; }

    [Required]
    [StringLength(DataGroupConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:DataGroup.Code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(DataGroupConsts.MaxNameLength)]
    [Display(Name = "DisplayName:DataGroup.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:DataGroup.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:DataGroup.Status")]
    public DataGroupStatus Status { get; set; } = DataGroupStatus.Active;

    [StringLength(DataGroupConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:DataGroup.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "DisplayName:DataGroup.ParentId")]
    [SelectItems(nameof(AvailableParents))]
    public Guid? ParentId { get; set; }

    public List<SelectListItem>? AvailableParents { get; set; }
}