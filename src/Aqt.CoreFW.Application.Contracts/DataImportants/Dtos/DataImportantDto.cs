using System;
using Aqt.CoreFW.DataImportants; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.DataImportants.Dtos;

public class DataImportantDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DataImportantStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public Guid DataGroupId { get; set; } // ID của nhóm dữ liệu

    // Thông tin bổ sung về nhóm dữ liệu (optional, được fill bởi AppService)
    public string? DataGroupName { get; set; } // Tên của DataGroup
    public string? DataGroupCode { get; set; } // Mã của DataGroup
} 