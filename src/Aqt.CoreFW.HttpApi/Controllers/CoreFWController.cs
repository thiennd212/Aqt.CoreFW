using Aqt.CoreFW.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Aqt.CoreFW.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class CoreFWController : AbpControllerBase
{
    protected CoreFWController()
    {
        LocalizationResource = typeof(CoreFWResource);
    }
}
