namespace Aqt.CoreFW.DataCores;

/// <summary>
/// Enum định nghĩa trạng thái của Danh mục Dữ liệu Cốt lõi.
/// </summary>
public enum DataCoreStatus : byte
{
    /// <summary>
    /// Danh mục không còn hoạt động.
    /// </summary>
    Inactive = 0, // Giữ giá trị 0

    /// <summary>
    /// Danh mục đang hoạt động và có thể sử dụng.
    /// </summary>
    Active = 1 // Giữ giá trị 1
} 