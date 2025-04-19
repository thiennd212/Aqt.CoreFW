using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // Excel DTO
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities; // Entity
using Aqt.CoreFW.Localization; // Resource for L[]
using AutoMapper;
using Microsoft.Extensions.Localization; // Required for IStringLocalizer
using Volo.Abp.DependencyInjection; // Required for ITransientDependency

namespace Aqt.CoreFW.Application.WorkflowStatuses;

/// <summary>
/// AutoMapper mapping action to handle specific logic when mapping
/// from WorkflowStatus entity to WorkflowStatusExcelDto.
/// Specifically handles localization of boolean values.
/// </summary>
public class WorkflowStatusToExcelMappingAction
    : IMappingAction<WorkflowStatus, WorkflowStatusExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;

    // Inject the localizer to get localized strings
    public WorkflowStatusToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
    {
        _localizer = localizer;
    }

    public void Process(WorkflowStatus source, WorkflowStatusExcelDto destination, ResolutionContext context)
    {
        // Perform the custom mapping logic here
        destination.IsActiveText = source.IsActive ? _localizer["Active"] : _localizer["Inactive"];

        // Other potential logic:
        // - Fetching related data (like creator username) if needed in the DTO
        // - Formatting dates/numbers specifically for Excel
    }
}