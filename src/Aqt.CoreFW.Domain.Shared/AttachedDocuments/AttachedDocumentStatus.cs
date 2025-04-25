namespace Aqt.CoreFW.AttachedDocuments;

/// <summary>
/// Enum định nghĩa trạng thái của Attached Document.
/// </summary>
public enum AttachedDocumentStatus : byte
{
    /// <summary>
    /// Attached document không còn hoạt động.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Attached document đang hoạt động và có thể sử dụng.
    /// </summary>
    Active = 1
} 