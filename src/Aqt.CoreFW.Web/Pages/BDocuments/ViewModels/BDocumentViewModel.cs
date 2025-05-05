using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.BDocuments; // Namespace chứa BDocumentConsts
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form; // Namespace cho [ReadOnlyInput]

namespace Aqt.CoreFW.Web.Pages.BDocuments.ViewModels;

/// <summary>
/// ViewModel cho trang tạo/sửa Hồ sơ (BDocument).
/// Chứa các thông tin cần thiết cho giao diện người dùng.
/// </summary>
public class BDocumentViewModel
{
    [HiddenInput]
    [DynamicFormIgnore]
    public Guid? Id { get; set; } // Nullable khi tạo mới

    [HiddenInput]
    [DynamicFormIgnore]
    public string HiddenDataFormInput { get; set; } = string.Empty; // Dữ liệu JSON cho form động (được bind từ hidden input)

    // --- Thông tin từ Procedure và WorkflowStatus (Chỉ đọc trên ViewModel, được map từ DTO) ---
    [DynamicFormIgnore]
    public string? ProcedureName { get; set; } // Tên thủ tục

    [DynamicFormIgnore]
    public string? WorkflowStatusName { get; set; } // Tên trạng thái

    [DynamicFormIgnore]
    public string? WorkflowStatusColorCode { get; set; } // Mã màu của trạng thái để hiển thị badge

    // --- Thông tin chính (Dùng để tạo/sửa) ---
    [Required]
    [HiddenInput] // ProcedureId thường được truyền vào khi mở modal Create, không cần người dùng chọn
    [DynamicFormIgnore]
    public Guid ProcedureId { get; set; }

    // Code (Mã hồ sơ) thường được sinh tự động hoặc chỉ hiển thị khi sửa
    [Display(Name = "DisplayName:BDocument.Code")] // Cập nhật key localization
    [ReadOnlyInput] // Đánh dấu chỉ đọc trên UI
    [StringLength(BDocumentConsts.MaxCodeLength)] // Sử dụng hằng số đã đổi tên
    [DynamicFormIgnore]
    public string Code { get; set; } = string.Empty; // Đổi tên: MaHoSo -> Code

    [Required(AllowEmptyStrings = false, ErrorMessage = "Validation:Mandatory")]
    [StringLength(BDocumentConsts.MaxApplicantNameLength)] // Sử dụng hằng số đã đổi tên
    [Display(Name = "DisplayName:BDocument.ApplicantName")] // Cập nhật key localization
    [DynamicFormIgnore]
    public string ApplicantName { get; set; } = string.Empty; // Đổi tên: TenChuHoSo -> ApplicantName

    [StringLength(BDocumentConsts.MaxApplicantIdentityNumberLength)] // Sử dụng hằng số đã đổi tên
    [Display(Name = "DisplayName:BDocument.ApplicantIdentityNumber")] // Cập nhật key localization
    [DynamicFormIgnore]
    public string? ApplicantIdentityNumber { get; set; } // Đổi tên: SoDinhDanhChuHoSo -> ApplicantIdentityNumber

    [StringLength(BDocumentConsts.MaxApplicantAddressLength)] // Sử dụng hằng số đã đổi tên
    [Display(Name = "DisplayName:BDocument.ApplicantAddress")] // Cập nhật key localization
    [DynamicFormIgnore]
    public string? ApplicantAddress { get; set; } // Đổi tên: DiaChiChuHoSo -> ApplicantAddress

    [EmailAddress(ErrorMessage = "Validation:InvalidEmail")]
    [StringLength(BDocumentConsts.MaxApplicantEmailLength)] // Sử dụng hằng số đã đổi tên
    [Display(Name = "DisplayName:BDocument.ApplicantEmail")] // Cập nhật key localization
    [DynamicFormIgnore]
    public string? ApplicantEmail { get; set; } // Đổi tên: EmailChuHoSo -> ApplicantEmail

    [Phone(ErrorMessage = "Validation:InvalidPhone")]
    [StringLength(BDocumentConsts.MaxApplicantPhoneNumberLength)] // Sử dụng hằng số đã đổi tên
    [Display(Name = "DisplayName:BDocument.ApplicantPhoneNumber")] // Cập nhật key localization
    [DynamicFormIgnore]
    public string? ApplicantPhoneNumber { get; set; } // Đổi tên: SoDienThoaiChuHoSo -> ApplicantPhoneNumber

    // --- Thông tin bổ sung ---
    [Display(Name = "DisplayName:BDocument.ScopeOfActivity")] // Cập nhật key localization
    [TextArea(Rows = 3)] // Gợi ý hiển thị textarea
    [DynamicFormIgnore]
    public string? ScopeOfActivity { get; set; } // Đổi tên: PhamViHoatDong -> ScopeOfActivity

    [Display(Name = "DisplayName:BDocument.ReceiveByPost")] // Cập nhật key localization
    [DynamicFormIgnore]
    public bool ReceiveByPost { get; set; } // Đổi tên: DangKyNhanQuaBuuDien -> ReceiveByPost

    // --- Danh sách dữ liệu Component ---
    // Chứa dữ liệu chi tiết cho từng thành phần (dạng form hoặc file)
    [DynamicFormIgnore]
    public List<BDocumentDataViewModel> DataList { get; set; } = new List<BDocumentDataViewModel>(); // Đổi tên: ComponentDataList -> DataList

    // --- Thông tin ngày tháng (Chỉ đọc trên ViewModel, được map từ DTO) ---
    [Display(Name = "DisplayName:BDocument.SubmissionDate")] // Cập nhật key localization
    [DataType(DataType.DateTime)]
    [DynamicFormIgnore]
    public DateTime? SubmissionDate { get; set; } // Đổi tên: NgayNop -> SubmissionDate

    [Display(Name = "DisplayName:BDocument.ReceptionDate")] // Cập nhật key localization
    [DataType(DataType.DateTime)]
    [DynamicFormIgnore]
    public DateTime? ReceptionDate { get; set; } // Đổi tên: NgayTiepNhan -> ReceptionDate

    [Display(Name = "DisplayName:BDocument.AppointmentDate")] // Cập nhật key localization
    [DataType(DataType.DateTime)]
    [DynamicFormIgnore]
    public DateTime? AppointmentDate { get; set; } // Đổi tên: NgayHenTra -> AppointmentDate

    [Display(Name = "DisplayName:BDocument.ResultDate")] // Cập nhật key localization
    [DataType(DataType.DateTime)]
    [DynamicFormIgnore]
    public DateTime? ResultDate { get; set; } // Đổi tên: NgayTraKetQua -> ResultDate

    [Display(Name = "DisplayName:BDocument.RejectionOrSupplementReason")] // Cập nhật key localization
    [ReadOnlyInput] // Chỉ hiển thị
    [DynamicFormIgnore]
    public string? RejectionOrSupplementReason { get; set; } // Đổi tên: LyDoTuChoiHoacBoSung -> RejectionOrSupplementReason
}