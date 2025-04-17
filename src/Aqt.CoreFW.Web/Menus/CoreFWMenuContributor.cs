using System.Threading.Tasks;
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Permissions;
using Aqt.CoreFW.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Microsoft.Extensions.Localization;

namespace Aqt.CoreFW.Web.Menus;

public class CoreFWMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<CoreFWResource>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                CoreFWMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );


        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 2);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        // Add Countries menu item under Administration if user has permission
        if (await context.IsGrantedAsync(CoreFWPermissions.Countries.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Countries,
                l["Menu:Countries"],
                "/Countries",
                icon: "fa fa-globe",
                order: 3
            ).RequirePermissions(CoreFWPermissions.Countries.Default));
        }
        // Kiểm tra xem người dùng có quyền xem Job Titles không
        if (await context.IsGrantedAsync(CoreFWPermissions.JobTitles.Default))
        {
            // Nếu có quyền, thêm mục menu Job Titles vào nhóm Administration
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.JobTitles,      // Hằng số tên menu
                l["Menu:JobTitles"],        // Lấy tên hiển thị từ localization
                "/JobTitles",               // Đường dẫn URL của trang quản lý
                icon: "fas fa-briefcase",   // Chọn một icon phù hợp (Font Awesome)
                order: 1                    // Thứ tự hiển thị trong nhóm (điều chỉnh nếu cần)
            // ).RequirePermissions(CoreFWPermissions.JobTitles.Default)); // Dòng này cũng được, nhưng IsGrantedAsync ở trên đã kiểm tra rồi
            ));
        }

        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 4);
    }
}
