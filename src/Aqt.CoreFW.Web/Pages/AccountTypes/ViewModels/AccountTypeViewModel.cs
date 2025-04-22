using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.AccountTypes; // Namespace Enum/Consts từ Domain.Shared
using Microsoft.AspNetCore.Mvc; // Required for HiddenInput

namespace Aqt.CoreFW.Web.Pages.AccountTypes.ViewModels; // Correct namespace

public class AccountTypeViewModel
{
    [HiddenInput] // ID ẩn đi trên form
    public Guid Id { get; set; }

    [Required]
    [StringLength(AccountTypeConsts.MaxCodeLength)]
    [Display(Name = "DisplayName:AccountType.Code")] // Sử dụng key localization
                                                     // Thêm [ReadOnlyInput] nếu không cho sửa Code trên UI Edit
                                                     // [Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form.ReadOnlyInput]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(AccountTypeConsts.MaxNameLength)]
    [Display(Name = "DisplayName:AccountType.Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "DisplayName:AccountType.Order")]
    public int Order { get; set; }

    [Required]
    [Display(Name = "DisplayName:AccountType.Status")]
    public AccountTypeStatus Status { get; set; } = AccountTypeStatus.Active; // Mặc định là Active

    [StringLength(AccountTypeConsts.MaxDescriptionLength)]
    [Display(Name = "DisplayName:AccountType.Description")]
    [DataType(DataType.MultilineText)] // Hiển thị textarea cho mô tả dài
    public string? Description { get; set; }

    // Các trường Sync không hiển thị trên form CRUD cơ bản
}