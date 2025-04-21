// src/Aqt.CoreFW.Domain.Shared/Districts/CoreFWDomainErrorCodes.Districts.cs
namespace Aqt.CoreFW;

public static partial class CoreFWDomainErrorCodes
{
    // District Error Codes - Bắt đầu từ 00051 (xác nhận lại số này)
    public const string DistrictCodeAlreadyExists = "CoreFW:Districts:00051";
    public const string ProvinceNotFound = "CoreFW:Districts:00053"; // Mã 00052 được bỏ qua theo kế hoạch gốc
}