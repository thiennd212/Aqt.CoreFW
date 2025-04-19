using System;
using Volo.Abp.Application.Dtos; // Optional: Can inherit if needed, but often not required for simple export DTOs

namespace Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;

/// <summary>
/// DTO specifically designed for exporting WorkflowStatus data to Excel.
/// Allows customization of columns, names, and formatting for the export file.
/// </summary>
public class WorkflowStatusExcelDto // No inheritance needed usually
{
    // Define properties exactly as you want them to appear as columns in Excel

    // Use DisplayName attribute if you want different column headers in Excel
    // (Requires adding using System.ComponentModel;)
    // [System.ComponentModel.DisplayName("Status Code")]
    public string Code { get; set; } = string.Empty;

    // [System.ComponentModel.DisplayName("Status Name")]
    public string Name { get; set; } = string.Empty;

    // [System.ComponentModel.DisplayName("Display Order")]
    public int Order { get; set; }

    // We'll map bool to a localized string ("Yes"/"No", "Active"/"Inactive", etc.) in the AppService
    // [System.ComponentModel.DisplayName("Is Active")]
    public string IsActiveText { get; set; } = string.Empty; // Use a string for localized boolean representation

    // [System.ComponentModel.DisplayName("Description")]
    public string? Description { get; set; }

    // [System.ComponentModel.DisplayName("Color Code")]
    public string? ColorCode { get; set; }

    // You might want to add Audit info if needed
    // public DateTime CreationTime { get; set; }
    // public string CreatorUsername { get; set; } // Need mapping logic for this
    // public DateTime? LastModificationTime { get; set; }
    // public string LastModifierUsername { get; set; } // Need mapping logic for this
}