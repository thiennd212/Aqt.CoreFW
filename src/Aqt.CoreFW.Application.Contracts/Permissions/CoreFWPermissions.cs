namespace Aqt.CoreFW.Permissions;

public static class CoreFWPermissions
{
    public const string GroupName = "CoreFW";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

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

    // Nhóm quyền con cho Chức vụ (Organization Structure)
    public static class JobTitles
    {
        public const string Default = GroupName + ".JobTitles"; // Quyền xem mặc định
        public const string Create = Default + ".Create";       // Quyền tạo
        public const string Edit = Default + ".Edit";         // Quyền sửa
        public const string Delete = Default + ".Delete";       // Quyền xóa
    }

    // Nhóm quyền con cho Hồ sơ người dùng (Organization Structure)
    public static class UserProfiles
    {
        public const string Default = GroupName + ".UserProfiles"; // Quyền xem mặc định
        public const string Edit = Default + ".Edit";          // Quyền sửa hồ sơ
    }

    // Nhóm quyền con cho Vị trí công tác (Organization Structure)
    public static class Positions
    {
        public const string Default = GroupName + ".Positions";     // Quyền xem mặc định
        public const string Create = Default + ".Create";        // Quyền tạo
        public const string Edit = Default + ".Edit";          // Quyền sửa (VD: IsPrimary, Roles)
        public const string Delete = Default + ".Delete";        // Quyền xóa
        public const string AssignRoles = Default + ".AssignRoles"; // Quyền gán vai trò
        public const string SetAsPrimary = Default + ".SetAsPrimary"; // Quyền đặt làm vị trí chính
    }
}
