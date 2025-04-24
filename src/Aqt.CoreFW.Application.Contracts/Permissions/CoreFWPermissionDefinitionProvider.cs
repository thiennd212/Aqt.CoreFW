using Aqt.CoreFW.Localization;
using EasyAbp.FileManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Aqt.CoreFW.Permissions;

public class CoreFWPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var coreFwGroup = context.GetGroupOrNull(CoreFWPermissions.GroupName) ?? context.AddGroup(CoreFWPermissions.GroupName, L("Permission:CoreFW")); // Adjusted group lookup/add

        // Define permissions for Countries
        var countriesPermission = coreFwGroup.AddPermission(CoreFWPermissions.Countries.Default, L("Permission:Countries"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Create, L("Permission:Countries.Create"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Edit, L("Permission:Countries.Edit"));
        countriesPermission.AddChild(CoreFWPermissions.Countries.Delete, L("Permission:Countries.Delete"));

        // Define permissions for Provinces
        var provincesPermission = coreFwGroup.AddPermission(CoreFWPermissions.Provinces.Default, L("Permission:ProvinceManagement"));
        provincesPermission.AddChild(CoreFWPermissions.Provinces.Create, L("Permission:Provinces.Create"));
        provincesPermission.AddChild(CoreFWPermissions.Provinces.Update, L("Permission:Provinces.Update"));
        provincesPermission.AddChild(CoreFWPermissions.Provinces.Delete, L("Permission:Provinces.Delete"));
        provincesPermission.AddChild(CoreFWPermissions.Provinces.Export, L("Permission:Provinces.Export"));

        // Define permissions for Districts using localization keys from Domain.Shared
        var districtsPermission = coreFwGroup.AddPermission(CoreFWPermissions.Districts.Default, L("Permission:DistrictManagement"));
        districtsPermission.AddChild(CoreFWPermissions.Districts.Create, L("Permission:Districts.Create"));
        districtsPermission.AddChild(CoreFWPermissions.Districts.Update, L("Permission:Districts.Update"));
        districtsPermission.AddChild(CoreFWPermissions.Districts.Delete, L("Permission:Districts.Delete"));
        districtsPermission.AddChild(CoreFWPermissions.Districts.Export, L("Permission:Districts.Export"));

        // Define permissions for Communes
        var communesPermission = coreFwGroup.AddPermission(CoreFWPermissions.Communes.Default, L("Permission:CommuneManagement")); // Use localization key
        communesPermission.AddChild(CoreFWPermissions.Communes.Create, L("Permission:Communes.Create"));
        communesPermission.AddChild(CoreFWPermissions.Communes.Update, L("Permission:Communes.Update"));
        communesPermission.AddChild(CoreFWPermissions.Communes.Delete, L("Permission:Communes.Delete"));
        communesPermission.AddChild(CoreFWPermissions.Communes.Export, L("Permission:Communes.Export")); // Add export permission

        // Định nghĩa permissions cho Ranks
        var ranksPermission = coreFwGroup.AddPermission(CoreFWPermissions.Ranks.Default, L("Permission:Ranks"));
        ranksPermission.AddChild(CoreFWPermissions.Ranks.Create, L("Permission:Ranks.Create"));
        ranksPermission.AddChild(CoreFWPermissions.Ranks.Update, L("Permission:Ranks.Update"));
        ranksPermission.AddChild(CoreFWPermissions.Ranks.Delete, L("Permission:Ranks.Delete"));
        ranksPermission.AddChild(CoreFWPermissions.Ranks.Export, L("Permission:Ranks.Export"));

        // Định nghĩa permissions cho DataGroups
        // Sử dụng localization key đã định nghĩa trong Domain.Shared plan: "Permission:DataGroupManagement"
        var dataGroupsPermission = coreFwGroup.AddPermission(CoreFWPermissions.DataGroups.Default, L("Permission:DataGroupManagement")); // Key localization cho nhóm quyền
        dataGroupsPermission.AddChild(CoreFWPermissions.DataGroups.Create, L("Permission:DataGroups.Create")); // Key localization cho từng quyền
        dataGroupsPermission.AddChild(CoreFWPermissions.DataGroups.Update, L("Permission:DataGroups.Update"));
        dataGroupsPermission.AddChild(CoreFWPermissions.DataGroups.Delete, L("Permission:DataGroups.Delete"));
        dataGroupsPermission.AddChild(CoreFWPermissions.DataGroups.Export, L("Permission:DataGroups.Export")); // (Nếu có)
        // dataGroupsPermission.AddChild(CoreFWPermissions.DataGroups.ManageHierarchy, L("Permission:DataGroups.ManageHierarchy")); // Nếu có quyền riêng

        // Định nghĩa permission cho JobTitles
        var jobTitlesPermission = coreFwGroup.AddPermission(CoreFWPermissions.JobTitles.Default, L("Permission:JobTitles"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Create, L("Permission:JobTitles.Create"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Edit, L("Permission:JobTitles.Edit"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Delete, L("Permission:JobTitles.Delete"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.ExportExcel, L("Permission:JobTitles.ExportExcel"));

        var accountTypesPermission = coreFwGroup.AddPermission(CoreFWPermissions.AccountTypes.Default, L("Permission:AccountTypeManagement")); // Key localization cho nhóm quyền
        accountTypesPermission.AddChild(CoreFWPermissions.AccountTypes.Create, L("Permission:AccountTypes.Create")); // Key localization cho từng quyền
        accountTypesPermission.AddChild(CoreFWPermissions.AccountTypes.Update, L("Permission:AccountTypes.Update"));
        accountTypesPermission.AddChild(CoreFWPermissions.AccountTypes.Delete, L("Permission:AccountTypes.Delete"));
        accountTypesPermission.AddChild(CoreFWPermissions.AccountTypes.Export, L("Permission:AccountTypes.Export")); // (Nếu có)


        var workflowStatusesPermission = coreFwGroup.AddPermission(CoreFWPermissions.WorkflowStatuses.Default, L("Permission:WorkflowStatuses"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.Create, L("Permission:WorkflowStatuses.Create"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.Edit, L("Permission:WorkflowStatuses.Edit"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.Delete, L("Permission:WorkflowStatuses.Delete"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.ExportExcel, L("Permission:WorkflowStatuses.ExportExcel"));

        // Add OrganizationUnit Permissions
        var ouPermission = coreFwGroup.AddPermission(CoreFWPermissions.OrganizationUnits.Default, L("Permission:OrganizationUnitManagement"));
        ouPermission.AddChild(CoreFWPermissions.OrganizationUnits.Create, L("Permission:OrganizationUnits.Create"));
        ouPermission.AddChild(CoreFWPermissions.OrganizationUnits.Update, L("Permission:OrganizationUnits.Update"));
        ouPermission.AddChild(CoreFWPermissions.OrganizationUnits.Delete, L("Permission:OrganizationUnits.Delete"));
        ouPermission.AddChild(CoreFWPermissions.OrganizationUnits.Move, L("Permission:OrganizationUnits.Move"));
        ouPermission.AddChild(CoreFWPermissions.OrganizationUnits.ManagePermissions, L("Permission:OrganizationUnits.ManagePermissions"));

        // Thêm định nghĩa permissions cho DataCores
        var dataCorePermission = coreFwGroup.AddPermission(CoreFWPermissions.DataCores.Default, L("Permission:DataCoreManagement"));
        dataCorePermission.AddChild(CoreFWPermissions.DataCores.Create, L("Permission:DataCores.Create"));
        dataCorePermission.AddChild(CoreFWPermissions.DataCores.Update, L("Permission:DataCores.Update"));
        dataCorePermission.AddChild(CoreFWPermissions.DataCores.Delete, L("Permission:DataCores.Delete"));        
        dataCorePermission.AddChild(CoreFWPermissions.DataCores.Export, L("Permission:DataCores.Export"));        
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreFWResource>(name);
    }
}
