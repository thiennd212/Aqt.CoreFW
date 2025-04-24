namespace Aqt.CoreFW.OrganizationUnits; // Namespace mới

/// <summary>
/// Enum định nghĩa trạng thái của Đơn vị/Phòng ban.
/// </summary>
public enum OrganizationUnitStatus : byte // Sử dụng byte để tiết kiệm
{
    /// <summary>
    /// Đơn vị/Phòng ban không còn hoạt động.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Đơn vị/Phòng ban đang hoạt động.
    /// </summary>
    Active = 1
} 