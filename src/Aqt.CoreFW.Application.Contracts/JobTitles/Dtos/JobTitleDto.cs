    using System;
    using Volo.Abp.Application.Dtos;

    namespace Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;

    /// <summary>
    /// DTO representing a Job Title entity, including audit information.
    /// </summary>
    public class JobTitleDto : FullAuditedEntityDto<Guid> // Kế thừa FullAuditedEntityDto để có thông tin audit
    {
        /// <summary>
        /// Job Title Code.
        /// </summary>
        public string Code { get; set; } = string.Empty; // Khởi tạo để tránh cảnh báo nullable

        /// <summary>
        /// Job Title Name.
        /// </summary>
        public string Name { get; set; } = string.Empty; // Khởi tạo để tránh cảnh báo nullable

        /// <summary>
        /// Optional description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Active status.
        /// </summary>
        public bool IsActive { get; set; }
    }