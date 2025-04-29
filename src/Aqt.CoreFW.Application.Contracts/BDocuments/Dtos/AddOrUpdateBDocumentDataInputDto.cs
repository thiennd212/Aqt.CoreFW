using System;
using System.ComponentModel.DataAnnotations;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

public class AddOrUpdateBDocumentDataInputDto
{
    [Required]
    public Guid ProcedureComponentId { get; set; }

    // Dùng để cập nhật JSON Tờ khai hoặc các Form khác
    public string? FormData { get; set; }

    // Dùng để cập nhật FileId cho component loại File
    public Guid? FileId { get; set; }
} 