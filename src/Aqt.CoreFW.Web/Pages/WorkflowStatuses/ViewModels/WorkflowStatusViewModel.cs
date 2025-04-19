using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.WorkflowStatuses;
using Microsoft.AspNetCore.Mvc; // Required for [HiddenInput]

namespace Aqt.CoreFW.Web.Pages.WorkflowStatuses.ViewModels;

public class WorkflowStatusViewModel
{
    [HiddenInput] // This field will be hidden in the form but bound
    public Guid Id { get; set; }

    [Required]
    [StringLength(WorkflowStatusConsts.MaxCodeLength)]
    [Display(Name = "WorkflowStatusCode")] // Maps to localization key
    public string Code { get; set; } = string.Empty; // Initialize

    [Required]
    [StringLength(WorkflowStatusConsts.MaxNameLength)]
    [Display(Name = "WorkflowStatusName")]
    public string Name { get; set; } = string.Empty; // Initialize

    [StringLength(WorkflowStatusConsts.MaxDescriptionLength)]
    [Display(Name = "WorkflowStatusDescription")]
    [DataType(DataType.MultilineText)] // Suggests a textarea input
    public string? Description { get; set; }

    [Required]
    [Display(Name = "WorkflowStatusOrder")]
    public int Order { get; set; }

    [StringLength(WorkflowStatusConsts.MaxColorCodeLength)]
    [Display(Name = "WorkflowStatusColorCode")]
    // [DataType(DataType.Color)] // Optional: Uncomment for basic browser color picker
    public string? ColorCode { get; set; }

    // Boolean doesn't strictly need [Required] as it defaults to false,
    // but explicitly adding it aligns with other properties.
    // The default value is handled in PageModel/C# logic.
    [Required]
    [Display(Name = "WorkflowStatusIsActive")]
    public bool IsActive { get; set; } = true; // Default to true for new items conceptually
}