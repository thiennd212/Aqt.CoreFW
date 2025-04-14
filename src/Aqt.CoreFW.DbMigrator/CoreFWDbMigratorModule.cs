using Aqt.CoreFW.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Aqt.CoreFW.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CoreFWEntityFrameworkCoreModule),
    typeof(CoreFWApplicationContractsModule)
)]
public class CoreFWDbMigratorModule : AbpModule
{
}
