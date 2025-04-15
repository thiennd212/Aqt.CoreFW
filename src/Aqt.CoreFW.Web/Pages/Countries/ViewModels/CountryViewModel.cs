using System;
using System.ComponentModel.DataAnnotations;
using Aqt.CoreFW.Domain.Shared.Countries;
using Microsoft.AspNetCore.Mvc;

namespace Aqt.CoreFW.Web.Pages.Countries.ViewModels;

/// <summary>
/// ViewModel for Country Create and Edit Modals.
/// Contains properties and validation attributes for the UI form.
/// </summary>
public class CountryViewModel
{
    /// <summary>
    /// The primary key of the Country. Hidden in the form.
    /// </summary>
    [HiddenInput]
    public Guid Id { get; set; }

    /// <summary>
    /// The unique code of the country.
    /// Required and has a maximum length defined in CountryConsts.
    /// Display name is localized via "CountryCode".
    /// </summary>
    [Required]
    [StringLength(CountryConsts.MaxCodeLength)]
    [Display(Name = "CountryCode")]
    public string Code { get; set; }

    /// <summary>
    /// The name of the country.
    /// Required and has a maximum length defined in CountryConsts.
    /// Display name is localized via "CountryName".
    /// </summary>
    [Required]
    [StringLength(CountryConsts.MaxNameLength)]
    [Display(Name = "CountryName")]
    public string Name { get; set; }
} 