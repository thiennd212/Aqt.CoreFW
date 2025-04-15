using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Countries;
using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectMapping;

namespace Aqt.CoreFW.Web.Pages.Countries;

/// <summary>
/// PageModel for the Create Country modal.
/// </summary>
public class CreateModalModel : CoreFWPageModel
{
    /// <summary>
    /// ViewModel bound to the form.
    /// </summary>
    [BindProperty]
    public CountryViewModel CountryViewModel { get; set; }

    private readonly ICountryAppService _countryAppService;

    public CreateModalModel(
        ICountryAppService countryAppService)
    {
        _countryAppService = countryAppService;
        CountryViewModel = new CountryViewModel(); // Initialize the ViewModel
    }

    /// <summary>
    /// Called when the modal is requested via GET.
    /// </summary>
    public void OnGet()
    {
        // No specific logic needed for a blank creation form.
    }

    /// <summary>
    /// Called when the modal form is submitted via POST.
    /// Maps the ViewModel to a DTO and calls the Application Service.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var dto = ObjectMapper.Map<CountryViewModel, CreateUpdateCountryDto>(CountryViewModel);
        await _countryAppService.CreateAsync(dto);
        return NoContent(); // Indicates success with no content to return
    }
} 