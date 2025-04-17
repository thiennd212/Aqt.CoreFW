using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.JobTitles; // Tham chiếu đến JobTitleConsts
using Microsoft.AspNetCore.Mvc; // Cần cho [HiddenInput]

// Namespace theo cấu trúc thư mục mới
namespace Aqt.CoreFW.Web.Pages.JobTitles.ViewModels;

/// <summary>
/// ViewModel for Job Title Create/Edit modals.
/// </summary>
public class JobTitleViewModel
{
    /// <summary> ID of the Job Title (for editing). </summary>
    [HiddenInput] // Thuộc tính này sẽ không hiển thị trên form nhưng vẫn được bind
    public Guid Id { get; set; }

    /// <summary> Job Title Code. </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(JobTitleConsts.MaxCodeLength)]
    [Display(Name = "JobTitleCode")] // Sử dụng localization key cho label
    public string Code { get; set; } = string.Empty;

    /// <summary> Job Title Name. </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(JobTitleConsts.MaxNameLength)]
    [Display(Name = "JobTitleName")] // Sử dụng localization key
    public string Name { get; set; } = string.Empty;

    /// <summary> Optional description. </summary>
    [StringLength(JobTitleConsts.MaxDescriptionLength)]
    [Display(Name = "JobTitleDescription")] // Sử dụng localization key
    [DataType(DataType.MultilineText)] // Gợi ý cho UI render thành textarea
    public string? Description { get; set; }

    /// <summary> Active status. </summary>
    // [Required] // Không cần [Required] vì bool không thể null, nhưng vẫn cần input
    [Display(Name = "JobTitleIsActive")] // Sử dụng localization key
    public bool IsActive { get; set; } = true; // Mặc định là true khi tạo mới
}