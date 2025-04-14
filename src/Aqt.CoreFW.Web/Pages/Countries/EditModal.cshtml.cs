using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Countries;
using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectMapping;

namespace Aqt.CoreFW.Web.Pages.Countries;

/// <summary>
/// PageModel for the Edit Country modal.
/// </summary>
public class EditModalModel : CoreFWPageModel
{
    /// <summary>
    /// The Id of the Country to edit, received from the query string.
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    /// <summary>
    /// ViewModel bound to the form.
    /// </summary>
    [BindProperty]
    public CountryViewModel CountryViewModel { get; set; }

    private readonly ICountryAppService _countryAppService;

    public EditModalModel(ICountryAppService countryAppService)
    {
        _countryAppService = countryAppService;
    }

    /// <summary>
    /// Called when the modal is requested via GET.
    /// Fetches the country data and maps it to the ViewModel.
    /// </summary>
    public async Task OnGetAsync()
    {
        var dto = await _countryAppService.GetAsync(Id);
        CountryViewModel = ObjectMapper.Map<CountryDto, CountryViewModel>(dto);
    }

    /// <summary>
    /// Called when the modal form is submitted via POST.
    /// Maps the ViewModel to a DTO and calls the Application Service to update.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var dto = ObjectMapper.Map<CountryViewModel, CreateUpdateCountryDto>(CountryViewModel);
        // Pass the Id from the ViewModel itself, ensuring consistency
        await _countryAppService.UpdateAsync(CountryViewModel.Id, dto);
        return NoContent(); // Indicates success
    }
} 