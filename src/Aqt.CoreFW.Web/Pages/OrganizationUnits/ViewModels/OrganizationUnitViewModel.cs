using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.OrganizationUnits; // Namespace Enum/Consts từ Domain.Shared
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace Aqt.CoreFW.Web.Pages.OrganizationUnits.ViewModels;

public class OrganizationUnitViewModel
{
    // Giả sử MaxDisplayNameLength = 128
    private const int MaxDisplayNameLength = 128; 
    // Lấy từ kế hoạch 1
    private const int MaxManualCodeLength = 50;
    private const int MaxDescriptionLength = 500;

    [HiddenInput]
    public Guid Id { get; set; }

    [HiddenInput] // ParentId sẽ được truyền ngầm khi mở modal từ context menu
    public Guid? ParentId { get; set; }

    [Required]
    [StringLength(MaxDisplayNameLength)] // Sử dụng giá trị số trực tiếp
    [Display(Name = "DisplayName:OrganizationUnit.DisplayName")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(MaxManualCodeLength)] // Sử dụng giá trị số trực tiếp
    [Display(Name = "DisplayName:OrganizationUnit.ManualCode")]
    public string? ManualCode { get; set; }

    [Required]
    [Display(Name = "DisplayName:OrganizationUnit.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:OrganizationUnit.Status")]
    public OrganizationUnitStatus Status { get; set; } = OrganizationUnitStatus.Active;

    [StringLength(MaxDescriptionLength)] // Sử dụng giá trị số trực tiếp
    [Display(Name = "DisplayName:OrganizationUnit.Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
} 