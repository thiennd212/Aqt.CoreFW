using Volo.Abp.Settings;

namespace Aqt.CoreFW.Settings;

public class CoreFWSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(CoreFWSettings.MySetting1));
    }
}
