using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos; // AttachedDocumentExcelDto
using Aqt.CoreFW.Domain.AttachedDocuments.Entities; // AttachedDocument Entity
using Aqt.CoreFW.Domain.Procedures; // IProcedureRepository (Giả định)
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity (Giả định)
using Aqt.CoreFW.Localization; // CoreFWResource for L[]
using AutoMapper;
using Microsoft.Extensions.Localization; // Required for IStringLocalizer
using Volo.Abp.DependencyInjection; // Required for ITransientDependency
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Application.AttachedDocuments; // Namespace for AttachedDocument Application layer

/// <summary>
/// AutoMapper mapping action to handle specific logic when mapping
/// from AttachedDocument entity to AttachedDocumentExcelDto.
/// Handles localization of the Status enum.
/// **Best Practice:** Populate ProcedureCode/Name in the AppService *before* mapping.
/// </summary>
public class AttachedDocumentToExcelMappingAction
    : IMappingAction<AttachedDocument, AttachedDocumentExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    // Injecting repository here for Procedure lookup is discouraged due to potential N+1 issues.
    // private readonly IRepository<Procedure, Guid> _procedureRepository; // Giả định

    public AttachedDocumentToExcelMappingAction(
        IStringLocalizer<CoreFWResource> localizer
        /*, IRepository<Procedure, Guid> procedureRepository */) // Giả định
    {
        _localizer = localizer;
        // _procedureRepository = procedureRepository;
    }

    public void Process(AttachedDocument source, AttachedDocumentExcelDto destination, ResolutionContext context)
    {
        // Localize the Status enum
        destination.StatusText = _localizer[$"Enum:AttachedDocumentStatus.{(int)source.Status}"];

        // Fetching Procedure Name/Code here leads to N+1 query issues.
        // Handle this in the AppService GetListAsExcelAsync method instead.
    }
} 