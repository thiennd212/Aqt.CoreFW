namespace Aqt.CoreFW; // Vẫn cùng namespace

// Phần partial của cùng lớp CoreFWDomainErrorCodes
public static partial class CoreFWDomainErrorCodes
{
    // --- Organization Structure Module Error Codes ---
    public const string JobTitleCodeAlreadyExists = "CoreFW:ORG001";
    public const string UserProfileAlreadyExistsForUser = "CoreFW:ORG002";
    public const string UserAlreadyHasPrimaryPosition = "CoreFW:ORG003";
    public const string PositionNotFound = "CoreFW:ORG004";
    public const string CannotDeleteJobTitleWithPositions = "CoreFW:ORG005";
    public const string CannotDeleteOrganizationUnitWithPositions = "CoreFW:ORG006";
    public const string PositionAlreadyExists = "CoreFW:ORG007";
    public const string OrganizationUnitNotFound = "CoreFW:ORG009";
    public const string JobTitleNotFound = "CoreFW:JT001";
    public const string RoleNotFound = "CoreFW:ROL001";
    public const string UserNotFound = "CoreFW:USR001";
    // ... thêm các mã lỗi khác của module này nếu cần ...
}