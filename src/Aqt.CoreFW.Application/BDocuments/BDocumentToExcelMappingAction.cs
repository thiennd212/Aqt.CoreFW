using Aqt.CoreFW.Application.Contracts.BDocuments.Dtos; // BDocumentExcelDto
using Aqt.CoreFW.Domain.BDocuments.Entities; // BDocument Entity
using Aqt.CoreFW.Domain.WorkflowStatuses; // IWorkflowStatusRepository
using Aqt.CoreFW.Domain.Procedures; // IProcedureRepository
using AutoMapper;
using Volo.Abp.DependencyInjection;
using System.Threading.Tasks;
using System.Linq;

namespace Aqt.CoreFW.Application.BDocuments;

/// <summary>
/// Optional AutoMapper mapping action for BDocument to BDocumentExcelDto.
/// Fetches related names (Procedure, Status) and formats boolean.
/// Can be replaced by enriching logic in AppService or simpler Profile mapping.
/// </summary>
public class BDocumentToExcelMappingAction
    : IMappingAction<BDocument, BDocumentExcelDto>, ITransientDependency
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IWorkflowStatusRepository _statusRepository;

    public BDocumentToExcelMappingAction(
        IProcedureRepository procedureRepository,
        IWorkflowStatusRepository statusRepository)
    {
        _procedureRepository = procedureRepository;
        _statusRepository = statusRepository;
    }

    // Synchronous version (less ideal, kept for compatibility if needed by specific AutoMapper configurations)
    public void Process(BDocument source, BDocumentExcelDto destination, ResolutionContext context)
    {
        ProcessAsync(source, destination, context).GetAwaiter().GetResult();
    }

    private async Task ProcessAsync(BDocument source, BDocumentExcelDto destination, ResolutionContext context)
    {
        // Fetch Procedure Name
        var procedure = await _procedureRepository.FindAsync(source.ProcedureId);
        destination.ProcedureName = procedure?.Name ?? string.Empty;

        // Fetch Status Name
        if (source.WorkflowStatusId.HasValue)
        {
            var status = await _statusRepository.FindAsync(source.WorkflowStatusId.Value);
            destination.StatusName = status?.Name ?? string.Empty;
        }
        else
        {
            destination.StatusName = ""; // Or a localized "Not Set" string
        }

        // Map new fields
        destination.ScopeOfActivity = source.ScopeOfActivity;
        destination.ReceiveByPost = source.ReceiveByPost ? "Yes" : "No"; // Format boolean

        // Map dates (already mapped by name convention, but can be explicit if needed)
        destination.SubmissionDate = source.SubmissionDate;
        destination.ReceptionDate = source.ReceptionDate;
        destination.AppointmentDate = source.AppointmentDate;
        destination.ResultDate = source.ResultDate;
    }
}