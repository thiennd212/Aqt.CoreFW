using System;

namespace Aqt.CoreFW.Application.Contracts.Provinces.Dtos;

/// <summary>
/// DTO specifically designed for exporting Province data to Excel.
/// </summary>
public class ProvinceExcelDto
{
    // Match property names with desired Excel column headers or use DisplayName attributes later if needed.
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int Order { get; set; }
    public string StatusText { get; set; } = string.Empty; // Export localized status text
    public string? Description { get; set; }
    public string? SyncId { get; set; }
    public string? SyncCode { get; set; }
    public DateTime? LastSyncedTime { get; set; }
    // Add Id or other audit fields if needed for export
    // public Guid Id { get; set;}
    // public DateTime CreationTime { get; set; }
}