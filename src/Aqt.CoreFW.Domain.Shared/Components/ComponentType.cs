namespace Aqt.CoreFW.Components;

/// <summary>
/// Enum định nghĩa loại của Thành phần thủ tục.
/// </summary>
public enum ComponentType : byte
{
    /// <summary>
    /// Thành phần yêu cầu định nghĩa Form (ví dụ: JSON).
    /// </summary>
    Form = 0,

    /// <summary>
    /// Thành phần yêu cầu đường dẫn đến file template.
    /// </summary>
    File = 1
} 