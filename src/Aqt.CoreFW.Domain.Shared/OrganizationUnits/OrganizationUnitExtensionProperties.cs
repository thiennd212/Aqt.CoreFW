namespace Aqt.CoreFW.OrganizationUnits;

/// <summary>
/// Defines constants for the names of extended properties added to the OrganizationUnit entity.
/// These constants are used consistently across layers (Domain, Application, EFCore mapping).
/// </summary>
public static class OrganizationUnitExtensionProperties
{
    public const string ModuleName = "Identity"; // Module name for OrganizationUnit

    public const string ManualCode = "ManualCode";
    public const string Status = "Status";
    public const string Order = "Order";
    public const string Description = "Description";
    public const string LastSyncedTime = "LastSyncedTime";
    public const string SyncRecordId = "SyncRecordId";
    public const string SyncRecordCode = "SyncRecordCode";
} 