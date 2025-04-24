namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho DataCore
public static partial class CoreFWDomainErrorCodes
{
    // DataCore Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác và sử dụng số duy nhất tiếp theo !!!
    // Giả sử DataGroups dùng 0008x, DataCores sẽ bắt đầu từ 00091. Cần kiểm tra lại số này!
    public const string DataCoreCodeAlreadyExists = "CoreFW:DataCores:00091"; // Cập nhật số!
    // Thêm các mã lỗi khác nếu cần thiết, ví dụ: không tìm thấy DataGroup tham chiếu
    public const string DataGroupNotFound = "CoreFW:DataCores:00092"; // Cập nhật số!
} 