using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.DataGroups; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.DataGroups.Dtos;

public class CreateUpdateDataGroupDto
{
    [Required]
    [StringLength(DataGroupConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty; // Bắt buộc khi tạo

    [Required]
    [StringLength(DataGroupConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DataGroupStatus Status { get; set; } = DataGroupStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(DataGroupConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    // Cho phép chọn nhóm cha (null nếu là nhóm gốc)
    public Guid? ParentId { get; set; }

    // Sync fields không cần thiết trong DTO tạo/sửa cơ bản
    // public DateTime? LastSyncDate { get; set; }
    // public Guid? SyncRecordId { get; set; }
    // [StringLength(DataGroupConsts.MaxSyncRecordCodeLength)]
    // public string? SyncRecordCode { get; set; }
} 