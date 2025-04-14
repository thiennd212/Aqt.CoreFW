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
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreFWResource>(name);
    }
}
