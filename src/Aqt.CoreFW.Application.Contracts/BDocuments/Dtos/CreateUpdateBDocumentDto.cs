using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.BDocuments; // Constants

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// DTO dùng chung cho việc tạo và cập nhật BDocument.
/// </summary>
public class CreateUpdateBDocumentDto
{
    [Required]
    public Guid ProcedureId { get; set; }

    // Mã hồ sơ không cần thiết ở đây vì sẽ được tạo tự động hoặc lấy từ URL khi update.
    // public string Code { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [StringLength(BDocumentConsts.MaxApplicantNameLength)]
    public string ApplicantName { get; set; } = string.Empty;

    [StringLength(BDocumentConsts.MaxApplicantIdentityNumberLength)]
    public string? ApplicantIdentityNumber { get; set; }

    [StringLength(BDocumentConsts.MaxApplicantAddressLength)]
    public string? ApplicantAddress { get; set; }

    [EmailAddress]
    [StringLength(BDocumentConsts.MaxApplicantEmailLength)]
    public string? ApplicantEmail { get; set; }

    [Phone]
    [StringLength(BDocumentConsts.MaxApplicantPhoneNumberLength)]
    public string? ApplicantPhoneNumber { get; set; }

    // Độ dài ScopeOfActivity phụ thuộc vào DB (NCLOB/Text)
    public string? ScopeOfActivity { get; set; }

    public bool ReceiveByPost { get; set; }

    // Ngày nộp chỉ có ý nghĩa khi tạo? Hoặc cho phép cập nhật?
    // Tạm thời cho phép cập nhật
    public DateTime? SubmissionDate { get; set; }

    // Danh sách dữ liệu/tham chiếu file cho các thành phần
    // Sử dụng DTO CreateUpdateBDocumentDataDto
    public List<CreateUpdateBDocumentDataDto> DocumentData { get; set; } = new List<CreateUpdateBDocumentDataDto>();
} 