using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.Communes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectListItem
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form; // Required for SelectItems attribute

namespace Aqt.CoreFW.Web.Pages.Communes.ViewModels;

public class CommuneViewModel
{
    [HiddenInput]
    public Guid Id { get; set; }

    [Required]
    [StringLength(CommuneConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:Commune.Code")] // Uses localization key
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(CommuneConsts.MaxNameLength)]
    [Display(Name = "DisplayName:Commune.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:Commune.ProvinceId")]
    [SelectItems(nameof(Provinces))] // Binds to Provinces property for dropdown items
    public Guid ProvinceId { get; set; }

    // Populated in PageModel for Province dropdown
    public List<SelectListItem>? Provinces { get; set; }

    // DistrictId is optional
    [Display(Name = "DisplayName:Commune.DistrictId")]
    [SelectItems(nameof(Districts))] // Binds to Districts property for dropdown items
    public Guid? DistrictId { get; set; }

    // Populated in PageModel for District dropdown (filtered by ProvinceId)
    public List<SelectListItem>? Districts { get; set; }

    [Required]
    [Display(Name = "DisplayName:Commune.Order")]
    public int Order { get; set; } = 0; // Default value

    [Required]
    [Display(Name = "DisplayName:Commune.Status")]
    public CommuneStatus Status { get; set; } = CommuneStatus.Active; // Default value

    [StringLength(CommuneConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:Commune.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    // Sync fields are typically not shown/edited in the standard UI
}