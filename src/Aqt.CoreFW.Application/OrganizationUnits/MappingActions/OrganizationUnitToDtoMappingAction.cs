using System;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using Aqt.CoreFW.Domain.OrganizationUnits;
using Aqt.CoreFW.OrganizationUnits;
using AutoMapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity; // For OrganizationUnit entity

namespace Aqt.CoreFW.Application.OrganizationUnits.MappingActions;

public class OrganizationUnitToDtoMappingAction : IMappingAction<OrganizationUnit, OrganizationUnitDto>, ITransientDependency
{
    public void Process(OrganizationUnit source, OrganizationUnitDto destination, ResolutionContext context)
    {
        // Logic moved from AfterMap in the profile
        destination.ManualCode = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.ManualCode)
                            ? source.ExtraProperties[OrganizationUnitExtensionProperties.ManualCode] as string
                            : null;
        destination.Status = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.Status)
                        ? (OrganizationUnitStatus)source.ExtraProperties[OrganizationUnitExtensionProperties.Status]
                        : default; // Or handle default/null case as needed
        destination.Order = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.Order)
                        ? (int)source.ExtraProperties[OrganizationUnitExtensionProperties.Order]
                        : 0;
        destination.Description = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.Description)
                             ? source.ExtraProperties[OrganizationUnitExtensionProperties.Description] as string
                             : null;
        destination.LastSyncedTime = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.LastSyncedTime)
                                ? source.ExtraProperties[OrganizationUnitExtensionProperties.LastSyncedTime] as DateTime?
                                : null;
        destination.SyncRecordId = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.SyncRecordId)
                              ? source.ExtraProperties[OrganizationUnitExtensionProperties.SyncRecordId] as string
                              : null;
        destination.SyncRecordCode = source.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.SyncRecordCode)
                                ? source.ExtraProperties[OrganizationUnitExtensionProperties.SyncRecordCode] as string
                                : null;
    }
} 