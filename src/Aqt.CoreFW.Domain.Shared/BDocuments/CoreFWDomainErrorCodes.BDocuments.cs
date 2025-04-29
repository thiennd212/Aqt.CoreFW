namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho BDocuments
public static partial class CoreFWDomainErrorCodes
{
    // BDocuments Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng và sử dụng số duy nhất tiếp theo !!!
    // Giả sử Components dùng 0012x, BDocuments bắt đầu từ 00131.
    public const string BDocumentCodeAlreadyExists = "CoreFW:BDocuments:00131";
    public const string ProcedureNotFoundForDocument = "CoreFW:BDocuments:00132";
    public const string WorkflowStatusNotFoundForDocument = "CoreFW:BDocuments:00133";
    public const string RequiredComponentDataMissing = "CoreFW:BDocuments:00134";
    public const string InvalidDocumentStatusTransition = "CoreFW:BDocuments:00135";
    public const string CannotUpdateDocumentInCurrentStatus = "CoreFW:BDocuments:00136";
    public const string CannotDeleteDocumentInCurrentStatus = "CoreFW:BDocuments:00137";
    public const string FileManagementInteractionFailed = "CoreFW:BDocuments:00138";
    public const string DeclarationFormComponentNotFound = "CoreFW:BDocuments:00139"; // Khi không tìm thấy component Tờ khai
    public const string FormDataGenerationFailed = "CoreFW:BDocuments:00140"; // Lỗi khi tạo JSON từ form động
    public const string DeclarationFileGenerationFailed = "CoreFW:BDocuments:00141"; // Lỗi khi tạo file từ tờ khai
    public const string InvalidFormDataForDeclaration = "CoreFW:BDocuments:00142"; // Dữ liệu JSON của tờ khai không hợp lệ
    // Thêm các mã lỗi khác nếu cần
} 