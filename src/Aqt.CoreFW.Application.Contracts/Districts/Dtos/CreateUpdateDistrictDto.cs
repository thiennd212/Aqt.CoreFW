    using System;
    using System.ComponentModel.DataAnnotations;
    using Aqt.CoreFW.Domain.Shared.Districts;

    namespace Aqt.CoreFW.Application.Contracts.Districts.Dtos;

    public class CreateUpdateDistrictDto
    {
        [Required]
        [StringLength(DistrictConsts.MaxCodeLength)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(DistrictConsts.MaxNameLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DistrictStatus Status { get; set; } = DistrictStatus.Active;

        [Required]
        public int Order { get; set; }

        [StringLength(DistrictConsts.MaxDescriptionLength)]
        public string? Description { get; set; }

        [Required]
        public Guid ProvinceId { get; set; }

        // Sync fields removed as they are not typically edited by users
    }