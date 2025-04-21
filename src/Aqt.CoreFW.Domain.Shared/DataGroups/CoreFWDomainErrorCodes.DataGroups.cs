using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho DataGroup
public static partial class CoreFWDomainErrorCodes
{
    // DataGroup Error Codes
    // Định dạng: CoreFW:[ModuleName]:<Số thứ tự 5 chữ số>
    // !!! Quan trọng: Kiểm tra số thứ tự cuối cùng trong các file partial khác (Province, District, Commune, Rank) và sử dụng số duy nhất tiếp theo !!!
    // Giả sử Ranks dùng 0007x, DataGroups sẽ bắt đầu từ 00081. Cập nhật số này!
    public const string DataGroupCodeAlreadyExists = "CoreFW:DataGroups:00081"; // Cập nhật số!
    public const string CannotDeleteDataGroupWithChildren = "CoreFW:DataGroups:00082"; // Cập nhật số!
    public const string CannotSetParentToSelfOrChild = "CoreFW:DataGroups:00083"; // Cập nhật số!
    // Thêm các mã lỗi khác nếu cần thiết trong quá trình phát triển
}
