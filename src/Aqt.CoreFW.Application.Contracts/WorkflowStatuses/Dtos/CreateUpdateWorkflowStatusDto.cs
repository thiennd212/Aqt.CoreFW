using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.WorkflowStatuses; // Using constants from Domain.Shared

namespace Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;

/// <summary>
/// DTO for creating or updating a WorkflowStatus.
/// Includes validation attributes based on WorkflowStatusConsts.
/// </summary>
public class CreateUpdateWorkflowStatusDto
{
    [Required]
    [StringLength(WorkflowStatusConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty; // Initialize to avoid null warnings

    [Required]
    [StringLength(WorkflowStatusConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty; // Initialize to avoid null warnings

    [StringLength(WorkflowStatusConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required] // Order is typically required
    public int Order { get; set; }

    [StringLength(WorkflowStatusConsts.MaxColorCodeLength)]
    public string? ColorCode { get; set; }

    // No [Required] needed as bool defaults to false,
    // but we default to true conceptually for new statuses.
    public bool IsActive { get; set; } = true;
}