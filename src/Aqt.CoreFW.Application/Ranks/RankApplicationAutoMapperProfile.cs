using Aqt.CoreFW.Application.Contracts.Ranks.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace mới cho RankLookupDto
using Aqt.CoreFW.Domain.Ranks.Entities;
using AutoMapper;

namespace Aqt.CoreFW.Application.Ranks
{
    public class RankApplicationAutoMapperProfile : Profile
    {
        public RankApplicationAutoMapperProfile()
        {
            CreateMap<Rank, RankDto>();
            CreateMap<RankDto, CreateUpdateRankDto>();
            CreateMap<Rank, RankLookupDto>();

            CreateMap<Rank, RankExcelDto>()
                .ForMember(dest => dest.StatusText, opt => opt.Ignore())
                .AfterMap<RankToExcelMappingAction>();
        }
    }
}