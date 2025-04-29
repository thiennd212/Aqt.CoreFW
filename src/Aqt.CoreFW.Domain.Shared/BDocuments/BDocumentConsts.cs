namespace Aqt.CoreFW.BDocuments;

public static class BDocumentConsts
{
    // Độ dài tối đa cho các trường của BDocument
    public const int MaxMaHoSoLength = 50;
    public const int MaxTenChuHoSoLength = 250;
    public const int MaxSoDinhDanhChuHoSoLength = 50;
    public const int MaxDiaChiChuHoSoLength = 500;
    public const int MaxEmailChuHoSoLength = 100;
    public const int MaxSoDienThoaiChuHoSoLength = 20;
    // MaxLength cho PhamViHoatDong nên dựa vào kiểu dữ liệu DB (nvarchar(max)/text thì không cần)
    // public const int MaxPhamViHoatDongLength = 4000; // Ví dụ nếu dùng nvarchar
    public const int MaxLyDoTuChoiHoacBoSungLength = 1000;

    // Hằng số cho tên component đặc biệt (Tờ khai) nếu cần tham chiếu trong code
    public const string DeclarationFormComponentCode = "TO_KHAI"; // Ví dụ mã code
    public const string FileContainerName = "Documents";
} 