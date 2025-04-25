namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho Procedures
public static partial class CoreFWDomainErrorCodes
{
    // Procedures Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // Số 00111 được xác định dựa trên các mã lỗi hiện có
    public const string ProcedureCodeAlreadyExists = "CoreFW:Procedures:00111";
    // Thêm các mã lỗi khác nếu cần thiết (ví dụ: liên quan đến đồng bộ)
} 