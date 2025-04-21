using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Ranks; // Sử dụng Enum/Consts từ Domain.Shared namespace

namespace Aqt.CoreFW.Application.Contracts.Ranks.Dtos
{
    public class CreateUpdateRankDto
    {
        [Required]
        [StringLength(RankConsts.MaxCodeLength)]
        public string Code { get; set; } = string.Empty; // Code bắt buộc khi tạo, có thể disable trên UI khi cập nhật

        [Required]
        [StringLength(RankConsts.MaxNameLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public RankStatus Status { get; set; } = RankStatus.Active; // Mặc định là Active

        [Required]
        public int Order { get; set; }

        [StringLength(RankConsts.MaxDescriptionLength)]
        public string? Description { get; set; }

        // Các trường Sync thường không do người dùng nhập/sửa trực tiếp từ form CRUD cơ bản
    }
}