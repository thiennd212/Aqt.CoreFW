using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.BDocuments; // Namespace chứa BDocumentConsts
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form; // Namespace cho [ReadOnlyInput]

namespace Aqt.CoreFW.Web.Pages.BDocuments.ViewModels;

public class BDocumentViewModel
{
    [HiddenInput]
    public Guid? Id { get; set; } // Nullable khi tạo mới

    // --- Thông tin từ Procedure và Status (Chỉ đọc trên ViewModel) ---
    public string? ProcedureName { get; set; }
    public string? TrangThaiHoSoName { get; set; }
    public string? TrangThaiHoSoColorCode { get; set; } // Để hiển thị badge màu

    // --- Thông tin chính (Dùng để tạo/sửa) ---
    [Required]
    [HiddenInput] // ProcedureId được truyền vào khi mở modal Create, không cần người dùng chọn
    public Guid ProcedureId { get; set; }

    // MaHoSo thường được sinh tự động hoặc chỉ hiển thị khi sửa
    [Display(Name = "DisplayName:BDocument.MaHoSo")]
    [ReadOnlyInput] // Đánh dấu chỉ đọc trên UI
    public string MaHoSo { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Validation:Mandatory")]
    [StringLength(BDocumentConsts.MaxTenChuHoSoLength)]
    [Display(Name = "DisplayName:BDocument.TenChuHoSo")]
    public string TenChuHoSo { get; set; } = string.Empty;

    [StringLength(BDocumentConsts.MaxSoDinhDanhChuHoSoLength)]
    [Display(Name = "DisplayName:BDocument.SoDinhDanhChuHoSo")]
    public string? SoDinhDanhChuHoSo { get; set; }

    [StringLength(BDocumentConsts.MaxDiaChiChuHoSoLength)]
    [Display(Name = "DisplayName:BDocument.DiaChiChuHoSo")]
    public string? DiaChiChuHoSo { get; set; }

    [EmailAddress(ErrorMessage = "Validation:InvalidEmail")]
    [StringLength(BDocumentConsts.MaxEmailChuHoSoLength)]
    [Display(Name = "DisplayName:BDocument.EmailChuHoSo")]
    public string? EmailChuHoSo { get; set; }

    [Phone(ErrorMessage = "Validation:InvalidPhone")]
    [StringLength(BDocumentConsts.MaxSoDienThoaiChuHoSoLength)]
    [Display(Name = "DisplayName:BDocument.SoDienThoaiChuHoSo")]
    public string? SoDienThoaiChuHoSo { get; set; }

    // --- Thông tin bổ sung ---
    [Display(Name = "DisplayName:BDocument.PhamViHoatDong")]
    [TextArea(Rows = 3)] // Gợi ý hiển thị textarea
    public string? PhamViHoatDong { get; set; } // Trường mới

    [Display(Name = "DisplayName:BDocument.DangKyNhanQuaBuuDien")]
    public bool DangKyNhanQuaBuuDien { get; set; } // Trường mới

    // --- Danh sách dữ liệu Component ---
    // Chứa dữ liệu cho cả Tờ khai (FormData JSON) và File đính kèm (FileId)
    // Tên property "ComponentDataList" cần khớp với cấu hình AutoMapper và cách binding từ form
    public List<BDocumentDataViewModel> ComponentDataList { get; set; } = new List<BDocumentDataViewModel>();

    // --- Thông tin ngày tháng (Chỉ đọc trên ViewModel) ---
    [Display(Name = "DisplayName:BDocument.NgayNop")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayNop { get; set; }

    [Display(Name = "DisplayName:BDocument.NgayTiepNhan")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayTiepNhan { get; set; }

    [Display(Name = "DisplayName:BDocument.NgayHenTra")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayHenTra { get; set; }

    [Display(Name = "DisplayName:BDocument.NgayTraKetQua")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayTraKetQua { get; set; }

    [Display(Name = "DisplayName:BDocument.LyDoTuChoiHoacBoSung")]
    [ReadOnlyInput] // Chỉ hiển thị, không cho sửa ở form Tạo/Sửa thông thường
    public string? LyDoTuChoiHoacBoSung { get; set; }
}