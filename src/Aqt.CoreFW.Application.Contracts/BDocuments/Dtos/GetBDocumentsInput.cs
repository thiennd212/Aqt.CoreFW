using System;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.Application.Contracts.BDocuments.Dtos;

public class GetBDocumentsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; } // Filter theo MaHoSo hoặc TenChuHoSo
    public Guid? ProcedureId { get; set; } // Lọc theo Thủ tục
    public Guid? StatusId { get; set; } // Lọc theo Trạng thái (WorkflowStatusId)
    public DateTime? SubmissionDateFrom { get; set; } // Lọc theo ngày nộp từ
    public DateTime? SubmissionDateTo { get; set; } // Lọc theo ngày nộp đến
    // Thêm filter nếu cần
    // public bool? DangKyNhanQuaBuuDien { get; set; }
} 