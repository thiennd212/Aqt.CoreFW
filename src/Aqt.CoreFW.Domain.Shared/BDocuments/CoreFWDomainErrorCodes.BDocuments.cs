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
    public const string DeclarationFormComponentNotFound = "CoreFW:BDocuments:00139";
    public const string InputDataGenerationFailed = "CoreFW:BDocuments:00140";
    public const string DeclarationFileGenerationFailed = "CoreFW:BDocuments:00141";
    public const string InvalidInputDataForDeclaration = "CoreFW:BDocuments:00142";
    public const string CannotAssociateFileWithFormComponent = "CoreFW:BDocuments:00143";
    public const string CannotSaveFormDataForFileComponent = "CoreFW:BDocuments:00144";
    public const string ProcedureComponentNotFound = "CoreFW:BDocuments:00145";
    public const string ProcedureComponentNotLinked = "CoreFW:BDocuments:00146";
    public const string CannotUpdateComponentDataInCurrentStatus = "CoreFW:BDocuments:00147";
    // Thêm các mã lỗi khác nếu cần
} 