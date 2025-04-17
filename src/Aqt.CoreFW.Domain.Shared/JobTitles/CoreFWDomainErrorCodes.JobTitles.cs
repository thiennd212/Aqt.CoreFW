    namespace Aqt.CoreFW;

    // Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho JobTitle
    public static partial class CoreFWDomainErrorCodes
    {
        // JobTitle Error Codes (Ví dụ: bắt đầu từ 00021)
        public const string JobTitleCodeAlreadyExists = "CoreFW:JobTitle:00021"; // Kiểm tra và đảm bảo mã là duy nhất
        public const string CannotDeleteJobTitleWithEmployees = "CoreFW:JobTitle:00022"; // Mã tiếp theo
        public const string ExportExcelFailed = "CoreFW:JobTitle:00023"; // Lỗi khi xuất Excel
    }