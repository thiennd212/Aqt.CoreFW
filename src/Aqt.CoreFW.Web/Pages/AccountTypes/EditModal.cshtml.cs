using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AccountTypes; // Cập nhật namespace
using Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos; // Cập nhật namespace
using Aqt.CoreFW.Web.Pages.AccountTypes.ViewModels; // Cập nhật namespace
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.AccountTypes; // Cập nhật namespace

public class EditModalModel : AbpPageModel
{
    [HiddenInput] // Đảm bảo thuộc tính Id vẫn được bind dù không hiển thị trực tiếp
    [BindProperty(SupportsGet = true)] // SupportsGet = true để nhận Id từ query string
    public Guid Id { get; set; }

    [BindProperty]
    public AccountTypeViewModel AccountTypeViewModel { get; set; } // Cập nhật ViewModel

    private readonly IAccountTypeAppService _accountTypeAppService; // Cập nhật AppService

    public EditModalModel(IAccountTypeAppService accountTypeAppService) // Cập nhật AppService
    {
        _accountTypeAppService = accountTypeAppService;
        AccountTypeViewModel = new AccountTypeViewModel(); // Cập nhật ViewModel
    }

    public async Task OnGetAsync()
    {
        var dto = await _accountTypeAppService.GetAsync(Id); // Cập nhật AppService
        AccountTypeViewModel = ObjectMapper.Map<AccountTypeDto, AccountTypeViewModel>(dto); // Cập nhật mapping
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = ObjectMapper.Map<AccountTypeViewModel, CreateUpdateAccountTypeDto>(AccountTypeViewModel); // Cập nhật mapping
        await _accountTypeAppService.UpdateAsync(Id, dto); // Cập nhật AppService
        return NoContent();
    }
}