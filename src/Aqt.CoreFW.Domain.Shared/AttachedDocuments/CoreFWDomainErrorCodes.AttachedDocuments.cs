namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho AttachedDocument
public static partial class CoreFWDomainErrorCodes
{
    // AttachedDocument Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác (ví dụ: DataImportants, DataCores) và sử dụng số duy nhất tiếp theo !!!
    // Giả sử DataImportants dùng đến 0010x, AttachedDocuments có thể bắt đầu từ 00111. Cần kiểm tra lại số này!
    public const string AttachedDocumentCodeAlreadyExists = "CoreFW:AttachedDocuments:00111"; // Cập nhật số!
    public const string ProcedureNotFoundForAttachedDocument = "CoreFW:AttachedDocuments:00112"; // Cập nhật số!
    // Thêm các mã lỗi khác nếu cần thiết
} 