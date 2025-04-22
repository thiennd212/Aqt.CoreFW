// src/Aqt.CoreFW.Domain.Shared/AccountTypes/AccountTypeConsts.cs
namespace Aqt.CoreFW.AccountTypes; // Đảm bảo namespace là Aqt.CoreFW.AccountTypes

public static class AccountTypeConsts
{
    // Độ dài tối đa cho các thuộc tính AccountType, dựa trên accouttype-srs.md và kế hoạch tổng thể
    public const int MaxCodeLength = 50;
    public const int MaxNameLength = 250;
    public const int MaxDescriptionLength = 500;
    public const int MaxSyncRecordCodeLength = 50; // Giữ nguyên từ kế hoạch trước
}