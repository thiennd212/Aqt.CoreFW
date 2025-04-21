using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Ranks;
using Microsoft.AspNetCore.Mvc;

namespace Aqt.CoreFW.Web.Pages.Ranks.ViewModels
{
    public class RankViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
        [StringLength(RankConsts.MaxCodeLength)]
        [Display(Name = "Rank:Code")] // Key localization từ bước 1

        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(RankConsts.MaxNameLength)]
        [Display(Name = "Rank:Name")] // Key localization từ bước 1
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rank:Order")] // Key localization từ bước 1
        public int Order { get; set; }

        [Required]
        [Display(Name = "Rank:Status")] // Key localization từ bước 1
        public RankStatus Status { get; set; } = RankStatus.Active;

        [StringLength(RankConsts.MaxDescriptionLength)]
        [Display(Name = "Rank:Description")] // Key localization từ bước 1
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
    }
}