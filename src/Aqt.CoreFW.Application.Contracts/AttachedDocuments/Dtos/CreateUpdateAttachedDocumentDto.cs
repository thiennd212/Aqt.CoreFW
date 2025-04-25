using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.AttachedDocuments; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.AttachedDocuments.Dtos;

public class CreateUpdateAttachedDocumentDto
{
    [Required]
    [StringLength(AttachedDocumentConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty; // Bắt buộc khi tạo

    [Required]
    [StringLength(AttachedDocumentConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public AttachedDocumentStatus Status { get; set; } = AttachedDocumentStatus.Active;

    [Required]
    public int Order { get; set; }

    [StringLength(AttachedDocumentConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required] // ProcedureId là bắt buộc
    public Guid ProcedureId { get; set; }
} 