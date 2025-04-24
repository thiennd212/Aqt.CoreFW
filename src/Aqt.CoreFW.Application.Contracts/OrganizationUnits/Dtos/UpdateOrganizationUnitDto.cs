using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.OrganizationUnits;

namespace Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;

public class UpdateOrganizationUnitDto
{
    [Required]
    [StringLength(128)] // Hardcoded based on Volo.Abp.Identity.OrganizationUnitConsts.MaxDisplayNameLength
    public string DisplayName { get; set; } = string.Empty;

    // --- Extended Properties ---
    [StringLength(50)] // Hardcoded based on Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxManualCodeLength
    public string? ManualCode { get; set; }

    [Required]
    public OrganizationUnitStatus Status { get; set; }

    [Required]
    public int Order { get; set; }

    [StringLength(500)] // Hardcoded based on Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxDescriptionLength
    public string? Description { get; set; }

    // Sync fields are usually not updated via this UI
    // public DateTime? LastSyncedTime { get; set; }
    // public string? SyncRecordId { get; set; }
    // public string? SyncRecordCode { get; set; }
} 