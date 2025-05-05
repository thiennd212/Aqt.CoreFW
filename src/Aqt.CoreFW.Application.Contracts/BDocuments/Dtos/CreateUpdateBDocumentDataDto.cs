using System;
using System.ComponentModel.DataAnnotations;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// Represents the input data for a single component during BDocument creation or update.
/// Đại diện cho dữ liệu đầu vào của một thành phần đơn lẻ trong quá trình tạo hoặc cập nhật BDocument.
/// </summary>
public class CreateUpdateBDocumentDataDto
{
    [Required]
    public Guid ProcedureComponentId { get; set; }

    // Dữ liệu form (JSON Tờ khai)
    public string? InputData { get; set; }

    // ID file từ FileManagement (File upload)
    public Guid? FileId { get; set; }
} 