using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Aqt.CoreFW.Application.Shared.Excel;
using Aqt.CoreFW.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aqt.CoreFW;

[DependsOn(
    typeof(CoreFWDomainModule),
    typeof(CoreFWApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class CoreFWApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CoreFWApplicationModule>();
        });
        context.Services.AddTransient<IAbpExcelExportHelper, AbpExcelExportHelper>();
    }
}
