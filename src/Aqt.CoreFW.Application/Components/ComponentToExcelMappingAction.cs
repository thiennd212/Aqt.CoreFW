    using Aqt.CoreFW.Application.Contracts.Components.Dtos; // ProcedureComponentExcelDto
    using Aqt.CoreFW.Domain.Components.Entities; // ProcedureComponent Entity
    using Aqt.CoreFW.Localization; // CoreFWResource for L[]
    using AutoMapper;
    using Microsoft.Extensions.Localization; // Required for IStringLocalizer
    using Volo.Abp.DependencyInjection; // Required for ITransientDependency

    namespace Aqt.CoreFW.Application.Components; // Namespace for Component Application layer

    /// <summary>
    /// AutoMapper mapping action to handle specific logic when mapping
    /// from ProcedureComponent entity to ProcedureComponentExcelDto.
    /// Primarily handles localization of Status and Type enums.
    /// </summary>
    public class ComponentToExcelMappingAction
        : IMappingAction<ProcedureComponent, ProcedureComponentExcelDto>, ITransientDependency
    {
        private readonly IStringLocalizer<CoreFWResource> _localizer;

        public ComponentToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
        {
            _localizer = localizer;
        }

        public void Process(ProcedureComponent source, ProcedureComponentExcelDto destination, ResolutionContext context)
        {
            // Localize the Status enum
            destination.StatusText = _localizer[$"Enum:ComponentStatus.{(int)source.Status}"];
            // Localize the Type enum
            destination.TypeText = _localizer[$"Enum:ComponentType.{(int)source.Type}"];
        }
    }