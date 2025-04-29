using System;
using System.ComponentModel.DataAnnotations;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// Represents the input data for a single component during BDocument creation.
/// </summary>
public class CreateBDocumentComponentDataInputDto
{
    [Required]
    public Guid ProcedureComponentId { get; set; }

    // Dữ liệu form (JSON Tờ khai)
    public string? FormData { get; set; }

    // ID file từ FileManagement (File upload)
    public Guid? FileId { get; set; }
} 