using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // Namespace to DTOs
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Aqt.CoreFW.Application.Contracts.WorkflowStatuses;

/// <summary>
/// Application service interface for managing Workflow Statuses.
/// Extends the standard ICrudAppService for basic CRUD operations.
/// </summary>
public interface IWorkflowStatusAppService :
    ICrudAppService< // Inherits standard CRUD functionalities
        WorkflowStatusDto,           // DTO used for displaying statuses
        Guid,                      // Primary key type of the WorkflowStatus entity
        GetWorkflowStatusesInput,  // DTO used for filtering/paging the list
        CreateUpdateWorkflowStatusDto> // DTO used for creating and updating statuses
{
    /// <summary>
    /// Gets a list of active workflow statuses suitable for lookup (e.g., dropdowns).
    /// </summary>
    /// <returns>A list of <see cref="WorkflowStatusLookupDto"/>.</returns>
    Task<ListResultDto<WorkflowStatusLookupDto>> GetLookupAsync();

    // Placeholder for potential future Excel export functionality (as mentioned in plan)
    // Task<IRemoteStreamContent> GetListAsExcelAsync(GetWorkflowStatusesInput input);
}