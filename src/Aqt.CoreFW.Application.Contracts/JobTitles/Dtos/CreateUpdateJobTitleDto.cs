    using System.ComponentModel.DataAnnotations;
    using Aqt.CoreFW.Domain.Shared.JobTitles; // Sử dụng hằng số từ Domain.Shared

    namespace Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;

    /// <summary>
    /// DTO used for creating or updating a Job Title.
    /// Includes validation attributes.
    /// </summary>
    public class CreateUpdateJobTitleDto
    {
        /// <summary>
        /// Job Title Code. Required.
        /// </summary>
        [Required(AllowEmptyStrings = false)] // Không cho phép chuỗi rỗng
        [StringLength(JobTitleConsts.MaxCodeLength, MinimumLength = 1)] // Thêm MinimumLength nếu cần
        public string Code { get; set; } = string.Empty; // Khởi tạo

        /// <summary>
        /// Job Title Name. Required.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(JobTitleConsts.MaxNameLength, MinimumLength = 1)] // Thêm MinimumLength nếu cần
        public string Name { get; set; } = string.Empty; // Khởi tạo

        /// <summary>
        /// Optional description.
        /// </summary>
        [StringLength(JobTitleConsts.MaxDescriptionLength)]
        public string? Description { get; set; }

        /// <summary>
        /// Active status. Required. Defaults to true for new entries.
        /// </summary>
        [Required] // Thêm Required vì đây là bool, không có giá trị null mặc định
        public bool IsActive { get; set; } = true; // Mặc định là active khi tạo mới
    }