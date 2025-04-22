using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace cập nhật theo vị trí mới

// Dùng cho việc lookup AccountType ở các module khác nếu cần
public class AccountTypeLookupDto : EntityDto<Guid> // Kế thừa EntityDto<Guid> đã bao gồm thuộc tính 'Id'
{
    public string Name { get; set; } = string.Empty;
    // Thêm Code nếu cần thiết cho việc hiển thị hoặc logic trong lookup
    // public string Code { get; set; } = string.Empty;
}