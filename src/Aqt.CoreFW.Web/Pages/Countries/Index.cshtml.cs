using Aqt.CoreFW.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aqt.CoreFW.Web.Pages.Countries;

/// <summary>
/// PageModel for the Country list page.
/// </summary>
public class IndexModel : CoreFWPageModel
{
    /// <summary>
    /// Called when the page is requested via GET.
    /// No specific logic needed here as data is loaded via AJAX.
    /// </summary>
    public void OnGet() { }
} 