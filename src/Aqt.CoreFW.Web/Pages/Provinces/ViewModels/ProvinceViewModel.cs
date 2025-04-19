using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.Provinces; // Namespace Enum và Consts
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Aqt.CoreFW.Web.Pages.Provinces.ViewModels;

public class ProvinceViewModel
{
    [HiddenInput]
    public Guid Id { get; set; }

    [Required]
    [StringLength(ProvinceConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:Province.Code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(ProvinceConsts.MaxNameLength)]
    [Display(Name = "DisplayName:Province.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:Province.CountryId")]
    [SelectItems(nameof(Countries))]
    public Guid CountryId { get; set; }

    // Dùng để hiển thị dropdown Quốc gia
    public List<SelectListItem>? Countries { get; set; }

    [Required]
    [Display(Name = "DisplayName:Province.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:Province.Status")]
    public ProvinceStatus Status { get; set; } = ProvinceStatus.Active;

    [StringLength(ProvinceConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:Province.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
}