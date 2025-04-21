    using Aqt.CoreFW.Application.Contracts.Districts.Dtos;
    using Aqt.CoreFW.Domain.Districts.Entities; // Verify namespace
    using Aqt.CoreFW.Localization;
    using AutoMapper;
    using Microsoft.Extensions.Localization;
    using Volo.Abp.DependencyInjection;

    namespace Aqt.CoreFW.Application.Districts;

    public class DistrictToExcelMappingAction
        : IMappingAction<District, DistrictExcelDto>, ITransientDependency
    {
        private readonly IStringLocalizer<CoreFWResource> _localizer;

        public DistrictToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
        {
            _localizer = localizer;
        }

        public void Process(District source, DistrictExcelDto destination, ResolutionContext context)
        {
            // Assumes ProvinceName is populated *before* this action runs (e.g., in AppService query)
            // If not, it needs to be looked up here or handled differently.

            destination.StatusText = _localizer[$"Enum:DistrictStatus.{(int)source.Status}"];
        }
    }