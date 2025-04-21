using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Ranks;
using Aqt.CoreFW.Application.Contracts.Ranks.Dtos;
using Aqt.CoreFW.Web.Pages.Ranks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.Ranks;

namespace Aqt.CoreFW.Web.Pages.Ranks
{
    public class CreateModalModel : AbpPageModel
    {
        [BindProperty]
        public RankViewModel RankViewModel { get; set; }

        private readonly IRankAppService _rankAppService;

        public CreateModalModel(IRankAppService rankAppService)
        {
            _rankAppService = rankAppService;
            RankViewModel = new RankViewModel { Status = RankStatus.Active };
        }

        public void OnGet()
        {
            RankViewModel = new RankViewModel { Status = RankStatus.Active };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dto = ObjectMapper.Map<RankViewModel, CreateUpdateRankDto>(RankViewModel);
            await _rankAppService.CreateAsync(dto);
            return NoContent();
        }
    }
}