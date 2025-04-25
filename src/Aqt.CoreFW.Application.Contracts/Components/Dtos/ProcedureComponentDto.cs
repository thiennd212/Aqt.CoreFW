using System;
using System.Collections.Generic; // For List<Guid>
using Aqt.CoreFW.Components; // Sử dụng Enum/Consts từ Domain.Shared namespace
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.Components.Dtos;

public class ProcedureComponentDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ComponentStatus Status { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }
    public ComponentType Type { get; set; }
    public string? FormDefinition { get; set; } // Hiển thị nếu Type là Form
    public string? TempPath { get; set; } // Hiển thị nếu Type là File

    // Danh sách ID các Thủ tục hành chính liên kết (để hiển thị hoặc xử lý ở client)
    public List<Guid> ProcedureIds { get; set; } = new List<Guid>();
} 