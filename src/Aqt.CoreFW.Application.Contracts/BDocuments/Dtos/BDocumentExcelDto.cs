using System;
using MiniExcelLibs.Attributes; // Giả sử dùng MiniExcel

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

/// <summary>
/// DTO được thiết kế riêng cho việc xuất dữ liệu BDocument ra Excel.
/// </summary>
public class BDocumentExcelDto
{
    [ExcelColumnName("Mã Hồ sơ")]
    public string MaHoSo { get; set; } = string.Empty;

    [ExcelColumnName("Tên Chủ hồ sơ")]
    public string TenChuHoSo { get; set; } = string.Empty;

    [ExcelColumnName("Thủ tục")]
    public string ProcedureName { get; set; } = string.Empty; // Tên thủ tục

    [ExcelColumnName("Trạng thái")]
    public string StatusName { get; set; } = string.Empty; // Tên trạng thái

    // Bỏ các trường Tờ khai

    [ExcelColumnName("Phạm vi hoạt động")] // MỚI
    public string? PhamViHoatDong { get; set; }

    [ExcelColumnName("Đăng ký nhận qua BĐ")] // MỚI
    public string DangKyNhanQuaBuuDien { get; set; } = string.Empty; // Map sang Yes/No

    [ExcelColumnName("Ngày nộp")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? NgayNop { get; set; }

    [ExcelColumnName("Ngày tiếp nhận")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? NgayTiepNhan { get; set; }

    [ExcelColumnName("Ngày hẹn trả")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? NgayHenTra { get; set; }

    [ExcelColumnName("Ngày trả kết quả")]
    [ExcelFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime? NgayTraKetQua { get; set; }

    [ExcelIgnore]
    public Guid ProcedureId { get; set; }

    [ExcelIgnore]
    public Guid? TrangThaiHoSoId { get; set; }
} 