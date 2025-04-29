using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// DTO simplified for listing BDocuments.
/// </summary>
public class BDocumentListDto : EntityDto<Guid>
{
    public string MaHoSo { get; set; } = string.Empty;
    public string TenChuHoSo { get; set; } = string.Empty;

    public Guid ProcedureId { get; set; }
    public string ProcedureName { get; set; } = string.Empty; // Tên thủ tục

    public Guid? TrangThaiHoSoId { get; set; } // Nullable
    public string TrangThaiHoSoName { get; set; } = string.Empty; // Tên trạng thái (có thể là "Chưa có")
    public string? TrangThaiHoSoColorCode { get; set; } // Màu trạng thái

    public DateTime CreationTime { get; set; } // Ngày tạo (hoặc NgayNop nếu dùng)

    // Có thể thêm cột khác nếu cần, ví dụ:
    // public bool DangKyNhanQuaBuuDien { get; set;}
} 