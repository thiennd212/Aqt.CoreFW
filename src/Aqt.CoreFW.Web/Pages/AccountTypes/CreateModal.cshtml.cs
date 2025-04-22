using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AccountTypes; // Cập nhật namespace
using Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos; // Cập nhật namespace
using Aqt.CoreFW.Web.Pages.AccountTypes.ViewModels; // Cập nhật namespace
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.AccountTypes; // Namespace Enum

namespace Aqt.CoreFW.Web.Pages.AccountTypes; // Cập nhật namespace

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public AccountTypeViewModel AccountTypeViewModel { get; set; } // Cập nhật ViewModel

    private readonly IAccountTypeAppService _accountTypeAppService; // Cập nhật AppService

    public CreateModalModel(IAccountTypeAppService accountTypeAppService) // Cập nhật AppService
    {
        _accountTypeAppService = accountTypeAppService;
        AccountTypeViewModel = new AccountTypeViewModel { Status = AccountTypeStatus.Active }; // Cập nhật ViewModel và Enum
    }

    public void OnGet()
    {
        AccountTypeViewModel = new AccountTypeViewModel { Status = AccountTypeStatus.Active }; // Cập nhật ViewModel và Enum
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = ObjectMapper.Map<AccountTypeViewModel, CreateUpdateAccountTypeDto>(AccountTypeViewModel); // Cập nhật mapping
        await _accountTypeAppService.CreateAsync(dto); // Cập nhật AppService
        return NoContent();
    }
}