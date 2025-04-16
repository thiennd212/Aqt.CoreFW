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
}
