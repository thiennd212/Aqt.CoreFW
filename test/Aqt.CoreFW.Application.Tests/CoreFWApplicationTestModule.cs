using Volo.Abp.Modularity;

namespace Aqt.CoreFW;

[DependsOn(
    typeof(CoreFWApplicationModule),
    typeof(CoreFWDomainTestModule)
)]
public class CoreFWApplicationTestModule : AbpModule
{

}
