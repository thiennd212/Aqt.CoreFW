using Aqt.CoreFW.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages;

public abstract class CoreFWPageModel : AbpPageModel
{
    protected CoreFWPageModel()
    {
        LocalizationResourceType = typeof(CoreFWResource);
    }
}
