namespace Aqt.CoreFW.DataGroups;

/// <summary>
/// Enum định nghĩa trạng thái của Nhóm Dữ liệu.
/// </summary>
public enum DataGroupStatus : byte
{
    /// <summary>
    /// Nhóm Dữ liệu không còn hoạt động.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Nhóm Dữ liệu đang hoạt động và có thể sử dụng.
    /// </summary>
    Active = 1
}