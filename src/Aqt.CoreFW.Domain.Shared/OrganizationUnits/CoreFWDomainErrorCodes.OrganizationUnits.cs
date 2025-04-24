namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho OrganizationUnit
public static partial class CoreFWDomainErrorCodes
{
    // OrganizationUnit Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác (AccountTypes, Ranks, ...) và sử dụng số duy nhất tiếp theo !!!
    // Giả sử AccountTypes dùng 0008x, OrganizationUnits sẽ bắt đầu từ 00091.
    public const string OrganizationUnitManualCodeAlreadyExists = "CoreFW:OrganizationUnits:00091";
    public const string OrganizationUnitCannotMoveToChild = "CoreFW:OrganizationUnits:00092"; // Ví dụ mã lỗi cho việc di chuyển không hợp lệ
    // Thêm các mã lỗi khác nếu cần thiết (ví dụ: liên quan đến validate thuộc tính mở rộng)
} 