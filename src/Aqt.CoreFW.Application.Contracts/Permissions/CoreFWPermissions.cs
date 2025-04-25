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
    /// Permissions for District management.
    /// </summary>
    public static class Districts
    {
        public const string Default = GroupName + ".Districts";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
    }

    // Add permission definitions for Communes
    public static class Communes
    {
        public const string Default = GroupName + ".Communes";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export"; // Permission for exporting data
    }

    // Thêm định nghĩa permission cho Ranks
    public static class Ranks
    {
        public const string Default = GroupName + ".Ranks";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
    }

    // Thêm định nghĩa permission cho DataGroups
    public static class DataGroups // Sử dụng tên class khớp với module
    {
        // Sử dụng convention đặt tên quyền: CoreFW.DataGroups
        public const string Default = GroupName + ".DataGroups"; // Quyền xem mặc định
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export"; // Quyền xuất Excel (nếu có)
        // Có thể thêm quyền quản lý cấu trúc cây nếu cần:
        // public const string ManageHierarchy = Default + ".ManageHierarchy";
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

    public static class AccountTypes // Sử dụng tên class khớp với module
    {
        // Sử dụng convention tên Module: CoreFW.AccountTypes
        public const string Default = GroupName + ".AccountTypes"; // Tên quyền xem mặc định
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export"; // Quyền xuất Excel (nếu có)
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

    // Thêm định nghĩa permission cho OrganizationUnits
    public static class OrganizationUnits
    {
        public const string Default = GroupName + ".OrganizationUnits";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Move = Default + ".Move";
        public const string ManagePermissions = Default + ".ManagePermissions";
    }

    // Thêm định nghĩa permission cho DataCores
    public static class DataCores // Sử dụng tên class khớp với module
    {
        // Sử dụng convention đặt tên quyền: CoreFW.DataCores
        public const string Default = GroupName + ".DataCores"; // Quyền xem mặc định
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export"; // Quyền xuất Excel (nếu có)
    }

    // Thêm định nghĩa permission cho DataImportants
    public static class DataImportants // Sử dụng tên class khớp với module
    {
        // Sử dụng convention đặt tên quyền: CoreFW.DataImportants
        public const string Default = GroupName + ".DataImportants"; // Quyền xem mặc định
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
    }

    // Thêm định nghĩa permission cho Procedures
    public static class Procedures // Sử dụng tên class khớp với module
    {
        // Sử dụng convention đặt tên quyền: CoreFW.Procedures
        public const string Default = GroupName + ".Procedures"; // Quyền xem mặc định
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
    }

    // Thêm định nghĩa permission cho AttachedDocuments
    public static class AttachedDocuments // Sử dụng tên class khớp với module
    {
        // Sử dụng convention đặt tên quyền: CoreFW.AttachedDocuments
        public const string Default = GroupName + ".AttachedDocuments"; // Quyền xem mặc định
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
    }
}
