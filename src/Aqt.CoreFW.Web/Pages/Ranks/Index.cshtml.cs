using System.Threading.Tasks;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Ranks
{
    public class IndexModel : AbpPageModel
    {
        private readonly IAuthorizationService _authorizationService;

        public IndexModel(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task OnGetAsync()
        {
            // Kiểm tra và truyền quyền sang View
            ViewData["CanEdit"] = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Ranks.Update);
            ViewData["CanDelete"] = await _authorizationService.IsGrantedAsync(CoreFWPermissions.Ranks.Delete);
        }
    }
}