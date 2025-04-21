    using System;
    using Aqt.CoreFW.Domain.Shared.Districts;
    using Volo.Abp.Application.Dtos;

    namespace Aqt.CoreFW.Application.Contracts.Districts.Dtos;

    public class GetDistrictsInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; } // General filter for Code or Name
        public DistrictStatus? Status { get; set; }
        public Guid? ProvinceId { get; set; }
    }