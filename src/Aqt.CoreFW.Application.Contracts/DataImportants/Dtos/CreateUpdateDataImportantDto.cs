using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.DataImportants; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.DataImportants.Dtos;

public class CreateUpdateDataImportantDto
{
    [Required]
    [StringLength(DataImportantConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty; // Bắt buộc khi tạo

    [Required]
    [StringLength(DataImportantConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DataImportantStatus Status { get; set; } = DataImportantStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(DataImportantConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required] // DataGroupId là bắt buộc
    public Guid DataGroupId { get; set; }
} 