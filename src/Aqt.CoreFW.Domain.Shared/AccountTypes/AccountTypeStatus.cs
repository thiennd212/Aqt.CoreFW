// src/Aqt.CoreFW.Domain.Shared/AccountTypes/AccountTypeStatus.cs
namespace Aqt.CoreFW.AccountTypes; // Đảm bảo namespace là Aqt.CoreFW.AccountTypes

/// <summary>
/// Enum định nghĩa trạng thái của Loại tài khoản.
/// </summary>
public enum AccountTypeStatus : byte // Sử dụng byte để tiết kiệm
{
    /// <summary>
    /// Loại tài khoản không còn hoạt động.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Loại tài khoản đang hoạt động và có thể sử dụng.
    /// </summary>
    Active = 1
}