using System;
using Volo.Abp.Application.Dtos;

// Namespace đã được cập nhật thành thư mục dùng chung
namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups;

public class CountryLookupDto : EntityDto<Guid>
{
    public string Name { get; set; }
    // Add Code if needed
    // public string Code { get; set; }
}