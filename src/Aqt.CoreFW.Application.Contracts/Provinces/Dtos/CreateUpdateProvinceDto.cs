using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.Provinces;

namespace Aqt.CoreFW.Application.Contracts.Provinces.Dtos;

public class CreateUpdateProvinceDto
{
    [Required]
    [StringLength(ProvinceConsts.MaxCodeLength)]
    public string Code { get; set; }

    [Required]
    [StringLength(ProvinceConsts.MaxNameLength)]
    public string Name { get; set; }

    [Required]
    public ProvinceStatus Status { get; set; } = ProvinceStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(ProvinceConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public Guid CountryId { get; set; }
}