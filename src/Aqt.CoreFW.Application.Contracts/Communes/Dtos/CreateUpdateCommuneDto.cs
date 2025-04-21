using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.Communes;

namespace Aqt.CoreFW.Application.Contracts.Communes.Dtos;

public class CreateUpdateCommuneDto
{
    [Required]
    [StringLength(CommuneConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(CommuneConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public CommuneStatus Status { get; set; } = CommuneStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(CommuneConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public Guid ProvinceId { get; set; }

    public Guid? DistrictId { get; set; }
}