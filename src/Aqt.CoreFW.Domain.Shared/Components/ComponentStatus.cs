namespace Aqt.CoreFW.Components;

/// <summary>
/// Enum định nghĩa trạng thái của Thành phần thủ tục.
/// </summary>
public enum ComponentStatus : byte
{
    /// <summary>
    /// Thành phần không còn hoạt động.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Thành phần đang hoạt động và có thể sử dụng.
    /// </summary>
    Active = 1
} 