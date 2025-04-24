using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.DataImportants.Dtos; // DataImportantExcelDto
using Aqt.CoreFW.Domain.DataImportants.Entities; // DataImportant Entity
using Aqt.CoreFW.Domain.DataGroups; // IDataGroupRepository
using Aqt.CoreFW.Domain.DataGroups.Entities;
using Aqt.CoreFW.Localization; // CoreFWResource for L[]
using AutoMapper;
using Microsoft.Extensions.Localization; // Required for IStringLocalizer
using Volo.Abp.DependencyInjection; // Required for ITransientDependency
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Application.DataImportants; // Namespace for DataImportant Application layer

/// <summary>
/// AutoMapper mapping action to handle specific logic when mapping
/// from DataImportant entity to DataImportantExcelDto.
/// Handles localization of the Status enum.
/// **Best Practice:** Populate DataGroupCode/Name in the AppService *before* mapping.
/// </summary>
public class DataImportantToExcelMappingAction
    : IMappingAction<DataImportant, DataImportantExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    // Injecting repository here for DataGroup lookup is discouraged due to potential N+1 issues.
    // private readonly IRepository<DataGroup, Guid> _dataGroupRepository;

    public DataImportantToExcelMappingAction(
        IStringLocalizer<CoreFWResource> localizer
        /*, IRepository<DataGroup, Guid> dataGroupRepository */)
    {
        _localizer = localizer;
        // _dataGroupRepository = dataGroupRepository;
    }

    public void Process(DataImportant source, DataImportantExcelDto destination, ResolutionContext context)
    {
        // Localize the Status enum
        destination.StatusText = _localizer[$"Enum:DataImportantStatus.{(int)source.Status}"];

        // Fetching DataGroup Name/Code here leads to N+1 query issues.
        // Handle this in the AppService GetListAsExcelAsync method instead.
    }
} 