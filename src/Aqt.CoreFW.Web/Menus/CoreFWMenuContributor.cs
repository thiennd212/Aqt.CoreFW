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

        // Add OrganizationUnits menu item if user has permission
        if (await context.IsGrantedAsync(CoreFWPermissions.OrganizationUnits.Default)) // Sử dụng quyền xem mặc định
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.OrganizationUnits,
                l["Menu:OrganizationUnits"], // Sử dụng key localization
                "/OrganizationUnits",        // Đường dẫn tới trang Index
                icon: "fas fa-sitemap",      // Icon gợi ý cho cây tổ chức
                order: 2 // Ví dụ: sau AccountTypes
            ));
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
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Provinces.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Provinces,
                l["Menu:Provinces"], // Sử dụng key localization
                "/Provinces",       // Đường dẫn URL
                icon: "fas fa-map-marked-alt", // Icon ví dụ
                order: 4 // Điều chỉnh thứ tự nếu cần
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Districts.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Districts,
                l["Menu:Districts"], // Use localization key
                "/Districts",        // Path to Index page
                icon: "fas fa-map-marker-alt", // Choose an icon
                order: 5           // Adjust order as needed
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Communes.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Communes,
                l["Menu:Communes"], // Key localization
                "/Communes",        // Đường dẫn đến trang Index
                icon: "fas fa-map-marker-alt", // Icon ví dụ, có thể đổi
                order: 6             // Thứ tự trong menu con
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.Ranks.Default))
        {
            administration.AddItem(new ApplicationMenuItem(
                CoreFWMenus.Ranks,
                l["Menu:RankManagement"], // Sử dụng key localization đã định nghĩa
                "/Ranks",
                icon: "fas fa-layer-group",
                order: 7 // Điều chỉnh order nếu cần
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
                order: 8                  
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
                order: 10
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
                order: 11
            ));
        }

        if (await context.IsGrantedAsync(CoreFWPermissions.Procedures.Default))
        {
            // Thêm vào menu Administration (hoặc menu phù hợp khác)
            administration.AddItem(
                 new ApplicationMenuItem(
                     CoreFWMenus.Procedures,       // Sử dụng hằng số đã định nghĩa
                     l["Menu:Procedures"],          // Key localization cho tên menu
                     "/Procedures",                 // Đường dẫn URL của trang Index
                     icon: "fas fa-tasks",           // Chọn icon (ví dụ: fa-tasks, fa-file-alt)
                     order: 13                     // Điều chỉnh thứ tự hiển thị nếu cần
                 )
             );
        }

        if (await context.IsGrantedAsync(CoreFWPermissions.Components.Default)) // Kiểm tra quyền xem Components
        {
            administration.AddItem(
                 new ApplicationMenuItem(
                     CoreFWMenus.Components,        // Menu constant vừa thêm
                     l["Menu:Components"],           // Key localization từ Domain.Shared
                     "/Components",                 // Đường dẫn tới trang Index.cshtml
                     icon: "fas fa-puzzle-piece",    // Icon (chọn icon phù hợp từ Font Awesome)
                     order: 14                 // Thứ tự hiển thị (điều chỉnh nếu cần)
                 )
             );
        }

        if (await context.IsGrantedAsync(CoreFWPermissions.BDocuments.Default))
        {
            context.Menu.AddItem(
                 new ApplicationMenuItem(
                     CoreFWMenus.BDocuments,        // Tên định danh menu
                     l["Menu:BDocuments"],         // Key localization cho tên hiển thị
                     "/BDocuments",                // Đường dẫn Razor Page
                     icon: "fas fa-folder-open",   // Icon Font Awesome
                     order: 15                      // Thứ tự hiển thị (sau Home, Administration...)
                 ).RequirePermissions(CoreFWPermissions.BDocuments.Default) // Yêu cầu quyền này để thấy menu
             );
        }

        var catalogManagementMenu = new ApplicationMenuItem(
                   CoreFWMenus.CatalogManagement,
                   l["Menu:CatalogManagement"], // Key localization cho "Catalog Management"
                   icon: "fa fa-book",
                   order: 12 // Điều chỉnh thứ tự nếu cần
               );
        administration.AddItem(catalogManagementMenu);

        if (await context.IsGrantedAsync(CoreFWPermissions.DataGroups.Default))
        {
            catalogManagementMenu.AddItem(new ApplicationMenuItem(
                CoreFWMenus.DataGroups,
                l["Menu:DataGroups"],
                "/DataGroups",
                icon: "fas fa-folder-tree",
                order: 1 // Điều chỉnh thứ tự nếu cần
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.DataCores.Default))
        {
            catalogManagementMenu.AddItem(new ApplicationMenuItem(
                CoreFWMenus.DataCores,
                l["Menu:DataCores"], // Key: "Core Data Catalogs"
                "/DataCores",        // Đường dẫn tới trang Index
                icon: "fa fa-database",
                order: 2 // Thứ tự trong CatalogManagement
            ));
        }
        if (await context.IsGrantedAsync(CoreFWPermissions.DataImportants.Default)) // Check DataImportant permission
        {
            // Giả sử menu cha tên là catalogManagementMenu
            catalogManagementMenu.AddItem(new ApplicationMenuItem(
                CoreFWMenus.DataImportants, // Menu constant
                l["Menu:DataImportants"],    // Localization key: "Important Data Catalogs"
                "/DataImportants",         // Đường dẫn tới trang Index
                icon: "fa fa-exclamation-triangle", // Chọn icon phù hợp (ví dụ: fa-exclamation-triangle)
                order: 3 // Điều chỉnh thứ tự trong CatalogManagement (ví dụ: sau DataCore)
            ));
        }
    }
}
