using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;

/// <summary>
/// DTO used for displaying WorkflowStatus options in lookup scenarios (e.g., dropdowns).
/// Contains minimal information needed for selection.
/// </summary>
public class WorkflowStatusLookupDto : EntityDto<Guid>
{
    // Include Code and Name as per plan
    public string Code { get; set; } = string.Empty; // Initialize to avoid null warnings
    public string Name { get; set; } = string.Empty; // Initialize to avoid null warnings
                                                     // Include ColorCode as per plan
    public string? ColorCode { get; set; }
}