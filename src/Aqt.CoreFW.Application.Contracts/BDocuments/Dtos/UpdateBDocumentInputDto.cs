using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.BDocuments; // Constants

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

// DTO này chỉ dùng để cập nhật thông tin chính của BDocument.
public class UpdateBDocumentInputDto
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(BDocumentConsts.MaxTenChuHoSoLength)]
    public string TenChuHoSo { get; set; } = string.Empty;

    [StringLength(BDocumentConsts.MaxSoDinhDanhChuHoSoLength)]
    public string? SoDinhDanhChuHoSo { get; set; }

    [StringLength(BDocumentConsts.MaxDiaChiChuHoSoLength)]
    public string? DiaChiChuHoSo { get; set; }

    [EmailAddress]
    [StringLength(BDocumentConsts.MaxEmailChuHoSoLength)]
    public string? EmailChuHoSo { get; set; }

    [Phone]
    [StringLength(BDocumentConsts.MaxSoDienThoaiChuHoSoLength)]
    public string? SoDienThoaiChuHoSo { get; set; }

    // --- LOẠI BỎ TRƯỜNG TỜ KHAI ---

    // Thông tin bổ sung có thể sửa
    public string? PhamViHoatDong { get; set; } // MỚI
    public bool DangKyNhanQuaBuuDien { get; set; } // MỚI
} 