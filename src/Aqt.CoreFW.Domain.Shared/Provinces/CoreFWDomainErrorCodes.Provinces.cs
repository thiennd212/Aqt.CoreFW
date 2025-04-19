// src/Aqt.CoreFW.Domain.Shared/CoreFWDomainErrorCodes.Provinces.cs
namespace Aqt.CoreFW; // Namespace gốc của partial class

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho Province
public static partial class CoreFWDomainErrorCodes
{
    // Province Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác và sử dụng số duy nhất tiếp theo !!!
    // Ví dụ dưới đây sử dụng số bắt đầu từ 00041 cho module Provinces (giả sử 0003x đã được dùng).
    public const string ProvinceCodeAlreadyExists = "CoreFW:Provinces:00041";
    // Có thể thêm các mã lỗi khác sau này nếu cần.
}