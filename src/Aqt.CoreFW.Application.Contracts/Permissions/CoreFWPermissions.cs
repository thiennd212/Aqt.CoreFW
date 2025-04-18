namespace Aqt.CoreFW.Permissions;

public static class CoreFWPermissions
{
    public const string GroupName = "CoreFW";

    /// <summary>
    /// Permissions for Country management.
    /// </summary>
    public static class Countries
    {
        public const string Default = GroupName + ".Countries";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// Permissions for Province management.
    /// </summary>
    public static class Provinces
    {
        public const string Default = GroupName + ".Provinces";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
    }

    /// <summary>
    /// Defines permissions related to Job Title management.
    /// </summary>
    public static class JobTitles
    {
        public const string Default = GroupName + ".JobTitles";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ExportExcel = Default + ".ExportExcel";
    }

    /// <summary>
    /// Permissions for WorkflowStatus management.
    /// </summary>
    public static class WorkflowStatuses // Permission group for WorkflowStatus
    {
        public const string Default = GroupName + ".WorkflowStatuses"; // e.g., "CoreFW.WorkflowStatuses"
        public const string Create = Default + ".Create";             // e.g., "CoreFW.WorkflowStatuses.Create"
        public const string Edit = Default + ".Edit";                 // e.g., "CoreFW.WorkflowStatuses.Edit"
        public const string Delete = Default + ".Delete";             // e.g., "CoreFW.WorkflowStatuses.Delete"
        public const string ExportExcel = Default + ".ExportExcel";
    }
}
