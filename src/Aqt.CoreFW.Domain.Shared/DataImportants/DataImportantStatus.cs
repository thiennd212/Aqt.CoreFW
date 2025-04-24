namespace Aqt.CoreFW.DataImportants;

/// <summary>
/// Enum định nghĩa trạng thái của Danh mục Dữ liệu Quan trọng.
/// </summary>
public enum DataImportantStatus : byte
{
    /// <summary>
    /// Danh mục không còn hoạt động.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Danh mục đang hoạt động và có thể sử dụng.
    /// </summary>
    Active = 1
} 