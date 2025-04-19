using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;

/// <summary>
/// Input DTO for filtering and requesting a paged list of WorkflowStatuses.
/// </summary>
public class GetWorkflowStatusesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Filter text to search in Code or Name.
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Filter by active status (null means include all).
    /// </summary>
    public bool? IsActive { get; set; }
}