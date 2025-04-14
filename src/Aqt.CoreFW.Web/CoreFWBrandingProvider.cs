using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Aqt.CoreFW.Localization;

namespace Aqt.CoreFW.Web;

[Dependency(ReplaceServices = true)]
public class CoreFWBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CoreFWResource> _localizer;

    public CoreFWBrandingProvider(IStringLocalizer<CoreFWResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
