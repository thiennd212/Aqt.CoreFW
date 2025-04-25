using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // ProcedureExcelDto
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity
using Aqt.CoreFW.Localization; // CoreFWResource for L[]
using AutoMapper;
using Microsoft.Extensions.Localization; // Required for IStringLocalizer
using Volo.Abp.DependencyInjection; // Required for ITransientDependency

namespace Aqt.CoreFW.Application.Procedures; // Namespace for Procedure Application layer

/// <summary>
/// AutoMapper mapping action to handle specific logic when mapping
/// from Procedure entity to ProcedureExcelDto.
/// Primarily handles localization of the Status enum.
/// </summary>
public class ProcedureToExcelMappingAction
    : IMappingAction<Procedure, ProcedureExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;

    public ProcedureToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
    {
        _localizer = localizer;
    }

    public void Process(Procedure source, ProcedureExcelDto destination, ResolutionContext context)
    {
        // Localize the Status enum
        // Ví dụ key localization: "Enum:ProcedureStatus.0", "Enum:ProcedureStatus.1"
        destination.StatusText = _localizer[$"Enum:ProcedureStatus.{(int)source.Status}"];
    }
}