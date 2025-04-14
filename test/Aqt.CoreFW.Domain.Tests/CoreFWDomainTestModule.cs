using Volo.Abp.Modularity;

namespace Aqt.CoreFW;

[DependsOn(
    typeof(CoreFWDomainModule),
    typeof(CoreFWTestBaseModule)
)]
public class CoreFWDomainTestModule : AbpModule
{

}
