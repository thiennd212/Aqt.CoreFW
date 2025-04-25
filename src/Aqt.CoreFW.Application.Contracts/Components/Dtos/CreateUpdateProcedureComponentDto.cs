using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Components; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.Components.Dtos;

public class CreateUpdateProcedureComponentDto
{
    // Code chỉ bắt buộc và cho phép nhập khi tạo mới (AppService sẽ xử lý logic này)
    [Required(AllowEmptyStrings = false)] // Bắt buộc khi Create
    [StringLength(ComponentConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(ComponentConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ComponentStatus Status { get; set; } = ComponentStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(ComponentConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public ComponentType Type { get; set; }

    // FormDefinition: Bắt buộc nếu Type là Form. Validation logic sẽ ở AppService.
    // Có thể thêm [RequiredIf] hoặc Custom Validation nếu dùng thư viện hỗ trợ.
    public string? FormDefinition { get; set; }

    // TempPath: Bắt buộc nếu Type là File. Validation logic sẽ ở AppService.
    [StringLength(ComponentConsts.MaxTempPathLength)]
    public string? TempPath { get; set; }

    // Danh sách ID các Thủ tục hành chính cần liên kết với Component này.
    // AppService sẽ dùng danh sách này để gọi ProcedureComponentManager.UpdateProcedureLinksAsync
    [Required] // Yêu cầu phải có danh sách (có thể rỗng)
    public List<Guid> ProcedureIds { get; set; } = new List<Guid>();
} 