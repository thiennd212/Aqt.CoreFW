using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.AccountTypes; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos;

public class CreateUpdateAccountTypeDto
{
    [Required]
    [StringLength(AccountTypeConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty; // Code bắt buộc khi tạo, có thể disable trên UI khi cập nhật

    [Required]
    [StringLength(AccountTypeConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public AccountTypeStatus Status { get; set; } = AccountTypeStatus.Active; // Mặc định là Active

    [Required]
    public int Order { get; set; }

    [StringLength(AccountTypeConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    // Các trường Sync thường không do người dùng nhập/sửa trực tiếp từ form CRUD cơ bản
    // public DateTime? LastSyncDate { get; set; }
    // public Guid? SyncRecordId { get; set; }
    // [StringLength(AccountTypeConsts.MaxSyncRecordCodeLength)]
    // public string? SyncRecordCode { get; set; }
}