namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho Rank
public static partial class CoreFWDomainErrorCodes
{
    // Rank Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác (Province, District, Commune) và sử dụng số duy nhất tiếp theo !!!
    // Giả sử Communes dùng 0006x, Ranks sẽ bắt đầu từ 00071.
    public const string RankCodeAlreadyExists = "CoreFW:Ranks:00071";
    // Thêm các mã lỗi khác nếu cần thiết trong quá trình phát triển
}