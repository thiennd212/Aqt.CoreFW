namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho Components
public static partial class CoreFWDomainErrorCodes
{
    // Components Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // Số được xác định dựa trên các mã lỗi hiện có (ví dụ: Procedures dùng 0011x).
    public const string ComponentCodeAlreadyExists = "CoreFW:Components:00121";
    // Thêm các mã lỗi khác nếu cần (ví dụ: validate FormDefinition, TempPath, liên kết...)
} 