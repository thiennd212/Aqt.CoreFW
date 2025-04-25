namespace Aqt.CoreFW.Procedures;

/// <summary>
/// Enum định nghĩa trạng thái của Thủ tục hành chính.
/// </summary>
public enum ProcedureStatus : byte
{
    /// <summary>
    /// Thủ tục không còn hoạt động.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Thủ tục đang hoạt động và có thể sử dụng.
    /// </summary>
    Active = 1
} 