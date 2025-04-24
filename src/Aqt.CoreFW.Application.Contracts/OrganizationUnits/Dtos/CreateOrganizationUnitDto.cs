using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.OrganizationUnits;
// using Volo.Abp.Identity; // No longer needed for consts here

namespace Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;

public class CreateOrganizationUnitDto
{
    public Guid? ParentId { get; set; }

    [Required]
    // Hardcode the length based on Volo.Abp.Identity.OrganizationUnitConsts.MaxDisplayNameLength
    [StringLength(128)]
    public string DisplayName { get; set; } = string.Empty;

    // --- Extended Properties ---
    // Hardcode the length based on Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxManualCodeLength
    [StringLength(50)]
    public string? ManualCode { get; set; }

    [Required]
    public OrganizationUnitStatus Status { get; set; } = OrganizationUnitStatus.Active;

    [Required]
    public int Order { get; set; } = 0;

    // Hardcode the length based on Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxDescriptionLength
    [StringLength(500)]
    public string? Description { get; set; }

    // Sync fields are usually not set during manual creation
    // public DateTime? LastSyncedTime { get; set; }
    // public string? SyncRecordId { get; set; }
    // public string? SyncRecordCode { get; set; }
} 