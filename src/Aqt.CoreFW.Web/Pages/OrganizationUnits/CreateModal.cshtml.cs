using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using Aqt.CoreFW.Web.Pages.OrganizationUnits.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Aqt.CoreFW.OrganizationUnits; // Enum

namespace Aqt.CoreFW.Web.Pages.OrganizationUnits;

public class CreateModalModel : AbpPageModel
{
    [BindProperty]
    public OrganizationUnitViewModel OrganizationUnitViewModel { get; set; } = new();

    private readonly IOrganizationUnitAppService _organizationUnitAppService;

    public CreateModalModel(IOrganizationUnitAppService organizationUnitAppService)
    {
        _organizationUnitAppService = organizationUnitAppService;
    }

    // Nhận parentId từ query string khi JS mở modal
    public void OnGet(Guid? parentId)
    {
        OrganizationUnitViewModel = new OrganizationUnitViewModel
        {
            ParentId = parentId,
            Status = OrganizationUnitStatus.Active // Mặc định trạng thái
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = ObjectMapper.Map<OrganizationUnitViewModel, CreateOrganizationUnitDto>(OrganizationUnitViewModel);
        await _organizationUnitAppService.CreateAsync(dto);
        return NoContent();
    }
} 