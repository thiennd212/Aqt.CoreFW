using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Ranks;
using Aqt.CoreFW.Application.Contracts.Ranks.Dtos;
using Aqt.CoreFW.Web.Pages.Ranks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Ranks
{
    public class EditModalModel : AbpPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public RankViewModel RankViewModel { get; set; }

        private readonly IRankAppService _rankAppService;

        public EditModalModel(IRankAppService rankAppService)
        {
            _rankAppService = rankAppService;
            RankViewModel = new RankViewModel(); // Khởi tạo để tránh null reference
        }

        // Đổi tên tham số OnGetAsync để nhận Id từ route/query string
        public async Task OnGetAsync(Guid id)
        {
            var dto = await _rankAppService.GetAsync(id);
            RankViewModel = ObjectMapper.Map<RankDto, RankViewModel>(dto);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Lấy Id từ ViewModel đã được bind
            var dto = ObjectMapper.Map<RankViewModel, CreateUpdateRankDto>(RankViewModel);
            await _rankAppService.UpdateAsync(RankViewModel.Id, dto);
            return NoContent();
        }
    }
}