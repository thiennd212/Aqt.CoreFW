namespace Aqt.CoreFW.BDocuments;

/// <summary>
/// Defines constants related to the BDocument entity.
/// </summary>
public static class BDocumentConsts
{
    // Độ dài tối đa cho các trường của BDocument
    public const int MaxCodeLength = 50;
    public const int MaxApplicantNameLength = 250;
    public const int MaxApplicantIdentityNumberLength = 50;
    public const int MaxApplicantAddressLength = 500;
    public const int MaxApplicantEmailLength = 100;
    public const int MaxApplicantPhoneNumberLength = 20;
    // MaxLength cho ScopeOfActivity nên dựa vào kiểu dữ liệu DB (NCLOB/text thì không cần)
    // public const int MaxScopeOfActivityLength = 4000; // Ví dụ nếu dùng nvarchar
    public const int MaxRejectionOrAdditionReasonLength = 1000;

    // Hằng số cho tên component đặc biệt (Tờ khai) nếu cần tham chiếu trong code
    public const string DeclarationFormComponentCode = "TO_KHAI";

    // Hằng số cho tên container quản lý file (EasyAbp.FileManagement)
    public const string FileContainerName = "bdocuments";
} 