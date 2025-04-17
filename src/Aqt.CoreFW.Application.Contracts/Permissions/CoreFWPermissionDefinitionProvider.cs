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

        // Define permissions for Countries
        var countriesPermission = myGroup.AddPermission(CoreFWPermissions.Countries.Default, L("Permission:Countries"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Create, L("Permission:Countries.Create"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Edit, L("Permission:Countries.Edit"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Delete, L("Permission:Countries.Delete"));

        // Định nghĩa permission cho JobTitles
        var jobTitlesPermission = myGroup.AddPermission(CoreFWPermissions.JobTitles.Default, L("Permission:JobTitles"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Create, L("Permission:JobTitles.Create"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Edit, L("Permission:JobTitles.Edit"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Delete, L("Permission:JobTitles.Delete"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.ExportExcel, L("Permission:JobTitles.ExportExcel"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreFWResource>(name);
    }
}
