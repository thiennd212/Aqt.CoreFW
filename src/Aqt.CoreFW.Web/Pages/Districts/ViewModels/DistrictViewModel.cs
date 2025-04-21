using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.Districts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Aqt.CoreFW.Web.Pages.Districts.ViewModels;

public class DistrictViewModel
{
    [HiddenInput]
    public Guid Id { get; set; }

    [Required]
    [StringLength(DistrictConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:District.Code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(DistrictConsts.MaxNameLength)]
    [Display(Name = "DisplayName:District.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:District.ProvinceId")]
    [SelectItems(nameof(Provinces))] // Link to the SelectListItem property
    public Guid ProvinceId { get; set; }

    // Property to hold the dropdown list items
    public List<SelectListItem>? Provinces { get; set; }

    [Required]
    [Display(Name = "DisplayName:District.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:District.Status")]
    public DistrictStatus Status { get; set; } = DistrictStatus.Active;

    [StringLength(DistrictConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:District.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
}