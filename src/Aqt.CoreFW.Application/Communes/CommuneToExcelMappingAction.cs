using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Localization; // Ensure CoreFWResource is accessible
using AutoMapper;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Application.Communes;

public class CommuneToExcelMappingAction
    : IMappingAction<Commune, CommuneExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;

    public CommuneToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
    {
        _localizer = localizer;
    }

    public void Process(Commune source, CommuneExcelDto destination, ResolutionContext context)
    {
        destination.StatusText = _localizer[$"Enum:CommuneStatus.{(int)source.Status}"];
        // ProvinceName and DistrictName are set manually in AppService before this action
    }
}