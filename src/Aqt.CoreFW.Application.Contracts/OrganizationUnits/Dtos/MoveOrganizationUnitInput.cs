using System;
using System.ComponentModel.DataAnnotations;

namespace Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;

public class MoveOrganizationUnitInput
{
    [Required]
    public Guid Id { get; set; }

    public Guid? NewParentId { get; set; }
} 