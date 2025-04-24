using System;
using Aqt.CoreFW.DataCores; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.DataCores.Dtos;

public class DataCoreDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DataCoreStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public Guid DataGroupId { get; set; } // ID của nhóm dữ liệu

    // Thông tin bổ sung về nhóm dữ liệu (optional, được fill bởi AppService)
    public string? DataGroupName { get; set; } // Tên của DataGroup
    public string? DataGroupCode { get; set; } // Mã của DataGroup (có thể hữu ích)
} 