namespace Aqt.CoreFW.Domain.Shared.OrgStructure;

/// <summary>
/// Contains constants related to the Organization Structure module,
/// shared across different layers.
/// </summary>
public static class OrgStructureConsts
{
    // --- JobTitle Constants ---
    /// <summary>
    /// Maximum length for the JobTitle Code property.
    /// Default value: 50
    /// </summary>
    public const int MaxJobTitleCodeLength = 50;

    /// <summary>
    /// Maximum length for the JobTitle Name property.
    /// Default value: 255
    /// </summary>
    public const int MaxJobTitleNameLength = 255;

    // --- UserProfile Constants ---
    /// <summary>
    /// Maximum length for the UserProfile FullName property.
    /// Default value: 150
    /// </summary>
    public const int MaxUserProfileFullNameLength = 150;
    // Add other UserProfile max length constants here if needed (e.g., MaxPhoneNumberLength)

    // --- OrganizationUnit Extension Property Names ---
    // These names are used to identify the custom properties added to OrganizationUnit
    // via the Object Extension system. They should match the keys used in localization files.

    /// <summary>
    /// Name of the custom property storing the Interoperability Code for an Organization Unit.
    /// Value: "InteroperabilityCode"
    /// </summary>
    public const string OuExtensionPropertyInteroperabilityCode = "InteroperabilityCode";

    /// <summary>
    /// Name of the custom property storing the Address for an Organization Unit.
    /// Value: "Address"
    /// </summary>
    public const string OuExtensionPropertyAddress = "Address";

    // --- Optional: Max lengths for OU Extension Properties (if enforced via validation) ---
    /// <summary>
    /// Maximum length for the InteroperabilityCode extension property.
    /// Default value: 100
    /// </summary>
    public const int MaxInteroperabilityCodeLength = 100;

    /// <summary>
    /// Maximum length for the Address extension property.
    /// Default value: 500
    /// </summary>
    public const int MaxAddressLength = 500;

    // --- Permission Group Name ---
    /// <summary>
    /// Name of the permission group for Organization Structure features.
    /// Value: "OrganizationStructure"
    /// </summary>
    public const string PermissionGroupName = "OrganizationStructure";

    // Add other shared constants for the module here if needed
}