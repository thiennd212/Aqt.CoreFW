using System;
using System.Collections.Generic;
using Aqt.CoreFW.Application.Contracts.Procedures.Dtos; // ProcedureDto
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos; // WorkflowStatusDto
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

public class BDocumentDto : FullAuditedEntityDto<Guid>
{
    public Guid ProcedureId { get; set; }
    public ProcedureDto? Procedure { get; set; } // Thông tin Thủ tục liên quan

    public string MaHoSo { get; set; } = string.Empty;
    public string TenChuHoSo { get; set; } = string.Empty;
    public string? SoDinhDanhChuHoSo { get; set; }
    public string? DiaChiChuHoSo { get; set; }
    public string? EmailChuHoSo { get; set; }
    public string? SoDienThoaiChuHoSo { get; set; }

    // --- LOẠI BỎ TRƯỜNG TỜ KHAI ---

    public string? PhamViHoatDong { get; set; } // MỚI
    public bool DangKyNhanQuaBuuDien { get; set; } // MỚI

    public Guid? TrangThaiHoSoId { get; set; } // Nullable
    public WorkflowStatusDto? TrangThaiHoSo { get; set; } // Thông tin Trạng thái liên quan

    public DateTime? NgayNop { get; set; }
    public DateTime? NgayTiepNhan { get; set; }
    public DateTime? NgayHenTra { get; set; }
    public DateTime? NgayTraKetQua { get; set; }
    public string? LyDoTuChoiHoacBoSung { get; set; }

    // Danh sách dữ liệu/file của các thành phần thuộc hồ sơ
    public List<BDocumentDataDto> DocumentData { get; set; } = new List<BDocumentDataDto>();
} 