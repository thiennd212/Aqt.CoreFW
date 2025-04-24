using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.DataCores; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.DataCores.Dtos;

public class CreateUpdateDataCoreDto
{
    [Required]
    [StringLength(DataCoreConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty; // Bắt buộc khi tạo

    [Required]
    [StringLength(DataCoreConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DataCoreStatus Status { get; set; } = DataCoreStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(DataCoreConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required] // DataGroupId là bắt buộc
    public Guid DataGroupId { get; set; }
} 