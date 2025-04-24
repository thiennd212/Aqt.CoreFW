using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.DataCores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Aqt.CoreFW.Web.Pages.DataCores.ViewModels;

public class DataCoreViewModel
{
    [HiddenInput]
    public Guid? Id { get; set; }

    [Required]
    [StringLength(DataCoreConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:DataCore.Code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(DataCoreConsts.MaxNameLength)]
    [Display(Name = "DisplayName:DataCore.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:DataCore.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:DataCore.Status")]
    public DataCoreStatus Status { get; set; } = DataCoreStatus.Active;

    [StringLength(DataCoreConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:DataCore.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Validation:DataGroupIdRequired")] // Thêm key localization "Validation:DataGroupIdRequired"
    [Display(Name = "DisplayName:DataCore.DataGroupId")]
    [SelectItems(nameof(DataGroupLookupList))]
    public Guid DataGroupId { get; set; }

    // For DataGroup dropdown
    public List<SelectListItem>? DataGroupLookupList { get; set; }
}