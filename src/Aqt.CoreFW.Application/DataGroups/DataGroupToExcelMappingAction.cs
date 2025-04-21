using System.Threading.Tasks; // Needed for async operations
using Aqt.CoreFW.Application.Contracts.DataGroups.Dtos; // DataGroupExcelDto
using Aqt.CoreFW.Domain.DataGroups; // IDataGroupRepository
using Aqt.CoreFW.Domain.DataGroups.Entities; // DataGroup Entity
using Aqt.CoreFW.Localization; // CoreFWResource for L[]
using AutoMapper;
using Microsoft.Extensions.Localization; // Required for IStringLocalizer
using Volo.Abp.DependencyInjection; // Required for ITransientDependency

namespace Aqt.CoreFW.Application.DataGroups; // Namespace for DataGroup Application layer

/// <summary>
/// AutoMapper mapping action to handle specific logic when mapping
/// from DataGroup entity to DataGroupExcelDto.
/// Handles localization of the Status enum and populates Parent details.
/// </summary>
public class DataGroupToExcelMappingAction
    : IMappingAction<DataGroup, DataGroupExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IDataGroupRepository _dataGroupRepository; // Inject repository to fetch parent

    public DataGroupToExcelMappingAction(
        IStringLocalizer<CoreFWResource> localizer,
        IDataGroupRepository dataGroupRepository)
    {
        _localizer = localizer;
        _dataGroupRepository = dataGroupRepository;
    }

    // Process method cannot be async directly in IMappingAction.
    // Fetching parent data here synchronously is not ideal.
    // Alternative: Populate ParentCode/Name in the AppService *before* mapping to Excel DTO,
    // or adjust the Excel DTO structure if sync fetching is acceptable/cached.
    // For simplicity in this plan, we'll assume parent info is fetched elsewhere (e.g., AppService GetList)
    // and passed via context or mapped directly *before* this action.
    // Let's focus on StatusText localization here.
    public void Process(DataGroup source, DataGroupExcelDto destination, ResolutionContext context)
    {
        // Localize the Status enum
        destination.StatusText = _localizer[$"Enum:DataGroupStatus.{(int)source.Status}"];

        // Fetching Parent Name/Code here can lead to N+1 query issues.
        // It's better handled in the AppService GetListAsExcelAsync method
        // by fetching all necessary data upfront or using includes.
        // destination.ParentCode = ...;
        // destination.ParentName = ...;
    }
} 