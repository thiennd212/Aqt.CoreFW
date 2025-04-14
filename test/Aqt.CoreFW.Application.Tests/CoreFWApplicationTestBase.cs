using Volo.Abp.Modularity;

namespace Aqt.CoreFW;

public abstract class CoreFWApplicationTestBase<TStartupModule> : CoreFWTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
