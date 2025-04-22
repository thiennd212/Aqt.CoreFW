// src/Aqt.CoreFW.Domain.Shared/AccountTypes/CoreFWDomainErrorCodes.AccountTypes.cs
namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho AccountType
public static partial class CoreFWDomainErrorCodes
{
    // AccountType Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác (Province, District, Commune, Rank, ...) và sử dụng số duy nhất tiếp theo !!!
    // Giả sử Ranks dùng 0007x, AccountTypes sẽ bắt đầu từ 00081.
    public const string AccountTypeCodeAlreadyExists = "CoreFW:AccountTypes:00081";
    // Thêm các mã lỗi khác nếu cần thiết trong quá trình phát triển
}