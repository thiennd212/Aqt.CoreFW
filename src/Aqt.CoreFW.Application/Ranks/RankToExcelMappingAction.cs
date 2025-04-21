using Aqt.CoreFW.Application.Contracts.Ranks.Dtos;
using Aqt.CoreFW.Domain.Ranks.Entities;
using Aqt.CoreFW.Localization;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Application.Ranks
{
    /// <summary>
    /// AutoMapper mapping action to handle specific logic when mapping
    /// from Rank entity to RankExcelDto.
    /// Specifically handles localization of the Status enum.
    /// </summary>
    public class RankToExcelMappingAction
        : IMappingAction<Rank, RankExcelDto>, ITransientDependency
    {
        private readonly IStringLocalizer<CoreFWResource> _localizer;

        public RankToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
        {
            _localizer = localizer;
        }

        public void Process(Rank source, RankExcelDto destination, ResolutionContext context)
        {
            destination.StatusText = _localizer[$"Enum:RankStatus.{(int)source.Status}"];
        }
    }
}