using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.BDocuments; // Constants

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

public class ChangeBDocumentStatusInputDto
{
    [Required]
    public Guid NewStatusId { get; set; } // ID của WorkflowStatus mới

    [StringLength(BDocumentConsts.MaxRejectionOrAdditionReasonLength)]
    public string? Reason { get; set; } // Lý do (nếu có)
} 