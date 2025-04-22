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
using EasyAbp.FileManagement.Permissions;

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
        administration.Order = 2;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);
        administration.TryRemoveMenuItem(SettingManagementMenuNames.GroupName);
        administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);

        //if (MultiTenancyConsts.IsEnabled)
        //{
        //    administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 2);
        //}
        //else
        //{
        //    administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        //}

        // Add Countries menu item under Administration if user has permission
        if (await context.IsGrantedAsync(CoreFWPermissions.Countries.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Countries,
                l["Menu:Countries"],
                "/Countries",
                icon: "fa fa-globe",
                order: 1
            ));            
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Provinces.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Provinces,
                l["Menu:Provinces"], // Sử dụng key localization
                "/Provinces",       // Đường dẫn URL
                icon: "fas fa-map-marked-alt", // Icon ví dụ
                order: 2 // Điều chỉnh thứ tự nếu cần
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Districts.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Districts,
                l["Menu:Districts"], // Use localization key
                "/Districts",        // Path to Index page
                icon: "fas fa-map-marker-alt", // Choose an icon
                order: 3           // Adjust order as needed
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Communes.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Communes,
                l["Menu:Communes"], // Key localization
                "/Communes",        // Đường dẫn đến trang Index
                icon: "fas fa-map-marker-alt", // Icon ví dụ, có thể đổi
                order: 4             // Thứ tự trong menu con
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Ranks.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Ranks,
                l["Menu:RankManagement"], // Sử dụng key localization đã định nghĩa
                "/Ranks",
                icon: "fas fa-layer-group",
                order: 5 // Điều chỉnh order nếu cần
            ));
        }
        // Kiểm tra xem người dùng có quyền xem Job Titles không
        if (await context.IsGrantedAsync(CoreFWPermissions.JobTitles.Default))
        {
            // Nếu có quyền, thêm mục menu Job Titles vào nhóm Administration
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.JobTitles,      
                l["Menu:JobTitles"],        
                "/JobTitles",               
                icon: "fas fa-briefcase",   
                order: 6                  
            ));
        }        
        if (await context.IsGrantedAsync(CoreFWPermissions.DataGroups.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.DataGroups,
                l["Menu:DataGroups"],
                "/DataGroups",
                icon: "fas fa-folder-tree",
                order: 7 // Điều chỉnh thứ tự nếu cần
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.AccountTypes.Default))
        {
            // Ví dụ: Thêm vào nhóm Administration
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.AccountTypes,
                l["Menu:AccountTypes"], 
                "/AccountTypes",       
                icon: "fas fa-id-card", 
                order: 8
            ));
        }
        // Kiểm tra xem người dùng có quyền xem WorkflowStatuses không
        if (await context.IsGrantedAsync(CoreFWPermissions.WorkflowStatuses.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.WorkflowStatuses,
                l["Menu:WorkflowStatuses"],
                "/WorkflowStatuses",
                icon: "fas fa-tasks",
                order: 9
            ));
        }
        if (await context.IsGrantedAsync(FileManagementPermissions.File.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                "FileManagement",
                l["Menu:FileManagement"],
                url: "/FileManagement",
                icon: "fa fa-file",
                order: 10
            ));
        }
    }
}
