using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.BDocuments; // Constants

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

public class CreateBDocumentInputDto
{
    [Required]
    public Guid ProcedureId { get; set; }

    // Thông tin chủ hồ sơ
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

    // Thông tin bổ sung
    public string? PhamViHoatDong { get; set; } // MỚI

    public bool DangKyNhanQuaBuuDien { get; set; } // MỚI

    // Danh sách dữ liệu/tham chiếu file cho các thành phần
    // Bao gồm cả dữ liệu JSON của "Tờ khai" trong FormData của item tương ứng
    [Required]
    public List<CreateBDocumentComponentDataInputDto> ComponentData { get; set; } = new List<CreateBDocumentComponentDataInputDto>();

    // MaHoSo tự sinh, TrangThaiHoSoId ban đầu null
} 