using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using Aqt.CoreFW.Web.Pages.OrganizationUnits.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Aqt.CoreFW.Web.Pages.OrganizationUnits;

public class EditModalModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public OrganizationUnitViewModel OrganizationUnitViewModel { get; set; } = new();

    private readonly IOrganizationUnitAppService _organizationUnitAppService;

    public EditModalModel(IOrganizationUnitAppService organizationUnitAppService)
    {
        _organizationUnitAppService = organizationUnitAppService;
    }

    // Nhận ID từ query string và bind vào thuộc tính Id
    public async Task OnGetAsync()
    {
        var dto = await _organizationUnitAppService.GetAsync(Id);
        OrganizationUnitViewModel = ObjectMapper.Map<OrganizationUnitDto, OrganizationUnitViewModel>(dto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Sử dụng thuộc tính Id của PageModel
         var dto = ObjectMapper.Map<OrganizationUnitViewModel, UpdateOrganizationUnitDto>(OrganizationUnitViewModel);
         await _organizationUnitAppService.UpdateAsync(Id, dto);
        return NoContent();
    }
} 