using Aqt.CoreFW.Domain.OrganizationUnits;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using AutoMapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Aqt.CoreFW.OrganizationUnits; // For OrganizationUnit entity

namespace Aqt.CoreFW.Application.OrganizationUnits.MappingActions;

public class OrganizationUnitToTreeNodeDtoMappingAction : IMappingAction<OrganizationUnit, OrganizationUnitTreeNodeDto>, ITransientDependency
{
    public void Process(OrganizationUnit source, OrganizationUnitTreeNodeDto destination, ResolutionContext context)
    {
        // Logic moved from AfterMap in the profile
        var manualCode = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.ManualCode)
                            ? source.ExtraProperties[OrganizationUnitExtensionProperties.ManualCode] as string
                            : null;
        // Adjust Text to include ManualCode or Code
        destination.Text = $"{source.DisplayName} ({(manualCode ?? source.Code)})";

        // Populate dest.Data using ExtraProperties
        destination.Data.ManualCode = manualCode;
        destination.Data.Status = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.Status)
                            ? (OrganizationUnitStatus)source.ExtraProperties[OrganizationUnitExtensionProperties.Status]
                            : default;
        destination.Data.Order = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.Order)
                           ? (int)source.ExtraProperties[OrganizationUnitExtensionProperties.Order]
                           : 0;
    }
} 