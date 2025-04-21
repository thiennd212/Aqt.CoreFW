// src/Aqt.CoreFW.Domain.Shared/Communes/CoreFWDomainErrorCodes.Communes.cs
namespace Aqt.CoreFW; // Namespace gốc của ErrorCodes

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho Commune
public static partial class CoreFWDomainErrorCodes
{
    /* Commune Error Codes */
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác (ví dụ: Province, District)
    // và sử dụng các số tiếp theo, đảm bảo chúng là DUY NHẤT !!!
    // Giả sử District kết thúc ở ...0005x, Communes có thể bắt đầu từ 00061.
    public const string CommuneCodeAlreadyExists = "CoreFW:Communes:00061";
    public const string ProvinceNotFoundForCommune = "CoreFW:Communes:00062";
    public const string DistrictNotFoundForCommune = "CoreFW:Communes:00063";
    public const string InvalidDistrictForSelectedProvince = "CoreFW:Communes:00064";
}