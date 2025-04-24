namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho DataImportant
public static partial class CoreFWDomainErrorCodes
{
    // DataImportant Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác (ví dụ: DataCores) và sử dụng số duy nhất tiếp theo !!!
    // Giả sử DataCores dùng 0009x, DataImportants có thể bắt đầu từ 00101. Cần kiểm tra lại số này!
    public const string DataImportantCodeAlreadyExists = "CoreFW:DataImportants:00101"; // Cập nhật số!
    // Thêm mã lỗi DataGroupNotFound vì DataImportant cũng tham chiếu đến DataGroup
    public const string DataGroupNotFoundForImportant = "CoreFW:DataImportants:00102"; // Cập nhật số! (Sử dụng tên khác để phân biệt nếu cần)
    // Thêm các mã lỗi khác nếu cần thiết
} 