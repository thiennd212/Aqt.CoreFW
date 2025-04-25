using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Procedures; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.Procedures.Dtos;

public class CreateUpdateProcedureDto
{
    // Code chỉ bắt buộc và cho phép nhập khi tạo mới (AppService sẽ xử lý logic này)
    // Khi cập nhật, Code thường không được gửi lên hoặc bị bỏ qua
    [Required(AllowEmptyStrings = false)] // Bắt buộc khi Create
    [StringLength(ProcedureConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(ProcedureConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ProcedureStatus Status { get; set; } = ProcedureStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(ProcedureConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    // Lưu ý: Các trường Sync (LastSyncedDate, SyncRecordId, SyncRecordCode) không được quản lý qua DTO này.
    // Chúng sẽ được cập nhật thông qua quy trình đồng bộ riêng biệt nếu cần.
} 