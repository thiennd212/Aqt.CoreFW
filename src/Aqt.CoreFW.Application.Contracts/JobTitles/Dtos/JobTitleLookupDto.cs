    using System;
    using Volo.Abp.Application.Dtos;

    namespace Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;

    /// <summary>
    /// Simplified DTO for representing a Job Title in lookup scenarios (e.g., dropdown lists).
    /// Contains only essential information for display (Id and Name).
    /// </summary>
    public class JobTitleLookupDto : EntityDto<Guid> // Kế thừa EntityDto để có thuộc tính Id
    {
        /// <summary>
        /// The name of the Job Title.
        /// </summary>
        public string Name { get; set; } = string.Empty; // Khởi tạo
        // Không cần Code, Description hay IsActive ở đây, giữ cho DTO nhẹ nhàng
    }