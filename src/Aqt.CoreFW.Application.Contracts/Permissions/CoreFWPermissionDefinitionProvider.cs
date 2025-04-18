﻿using Aqt.CoreFW.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Aqt.CoreFW.Permissions;

public class CoreFWPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var coreFwGroup = context.AddGroup(CoreFWPermissions.GroupName, L("Permission:CoreFWGroup"));

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

        // Định nghĩa permission cho JobTitles
        var jobTitlesPermission = coreFwGroup.AddPermission(CoreFWPermissions.JobTitles.Default, L("Permission:JobTitles"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Create, L("Permission:JobTitles.Create"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Edit, L("Permission:JobTitles.Edit"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.Delete, L("Permission:JobTitles.Delete"));
        jobTitlesPermission.AddChild(CoreFWPermissions.JobTitles.ExportExcel, L("Permission:JobTitles.ExportExcel"));

        var workflowStatusesPermission = coreFwGroup.AddPermission(CoreFWPermissions.WorkflowStatuses.Default, L("Permission:WorkflowStatuses"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.Create, L("Permission:WorkflowStatuses.Create"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.Edit, L("Permission:WorkflowStatuses.Edit"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.Delete, L("Permission:WorkflowStatuses.Delete"));
        workflowStatusesPermission.AddChild(CoreFWPermissions.WorkflowStatuses.ExportExcel, L("Permission:WorkflowStatuses.ExportExcel"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreFWResource>(name);
    }
}
