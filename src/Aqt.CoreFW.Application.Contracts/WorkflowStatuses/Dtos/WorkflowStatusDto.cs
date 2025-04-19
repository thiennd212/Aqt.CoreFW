using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;

/// <summary>
/// DTO for displaying WorkflowStatus information.
/// Inherits auditing properties from FullAuditedEntityDto.
/// </summary>
public class WorkflowStatusDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty; // Initialize to avoid null warnings
    public string Name { get; set; } = string.Empty; // Initialize to avoid null warnings
    public string? Description { get; set; }
    public int Order { get; set; }
    public string? ColorCode { get; set; }
    public bool IsActive { get; set; }
}