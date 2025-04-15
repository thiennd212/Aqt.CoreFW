using Aqt.CoreFW.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Aqt.CoreFW.Permissions;

public class CoreFWPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CoreFWPermissions.GroupName, L("Permission:CoreFWGroup"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(CoreFWPermissions.MyPermission1, L("Permission:MyPermission1"));

        // Define permissions for Countries
        var countriesPermission = myGroup.AddPermission(CoreFWPermissions.Countries.Default, L("Permission:Countries"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Create, L("Permission:Countries.Create"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Edit, L("Permission:Countries.Edit"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Delete, L("Permission:Countries.Delete"));

        // Định nghĩa quyền cho JobTitles (Sử dụng hằng số từ CoreFWPermissions)
        var jobTitlesPermission = myGroup.AddPermission(CoreFWPermissions.JobTitles.Default, L("Permission:JobTitles")); // Key localization mới
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Create, L("Permission:JobTitles.Create"));       // Key localization mới
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Edit, L("Permission:JobTitles.Edit"));           // Key localization mới
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Delete, L("Permission:JobTitles.Delete"));       // Key localization mới

        // Định nghĩa quyền cho UserProfiles (Sử dụng hằng số từ CoreFWPermissions)
        var userProfilesPermission = myGroup.AddPermission(CoreFWPermissions.UserProfiles.Default, L("Permission:UserProfiles")); // Key localization mới
        userProfilesPermission.AddChild(CoreFWPermissions.UserProfiles.Edit, L("Permission:UserProfiles.Edit")); // Key localization mới

        // Định nghĩa quyền cho Positions (Sử dụng hằng số từ CoreFWPermissions)
        var positionsPermission = myGroup.AddPermission(CoreFWPermissions.Positions.Default, L("Permission:Positions")); // Key localization mới
        positionsPermission.AddChild(CoreFWPermissions.Positions.Create, L("Permission:Positions.Create"));        // Key localization mới
        positionsPermission.AddChild(CoreFWPermissions.Positions.Edit, L("Permission:Positions.Edit"));          // Key localization mới
        positionsPermission.AddChild(CoreFWPermissions.Positions.Delete, L("Permission:Positions.Delete"));        // Key localization mới
        positionsPermission.AddChild(CoreFWPermissions.Positions.AssignRoles, L("Permission:Positions.AssignRoles")); // Key localization mới
        positionsPermission.AddChild(CoreFWPermissions.Positions.SetAsPrimary, L("Permission:Positions.SetAsPrimary")); // Key localization mới
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreFWResource>(name);
    }
}
