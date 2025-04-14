using Aqt.CoreFW.Localization;
using Volo.Abp.Application.Services;

namespace Aqt.CoreFW;

/* Inherit your application services from this class.
 */
public abstract class CoreFWAppService : ApplicationService
{
    protected CoreFWAppService()
    {
        LocalizationResource = typeof(CoreFWResource);
    }
}
