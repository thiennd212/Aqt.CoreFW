namespace Aqt.CoreFW;

public static class CoreFWDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    // Country related error codes (adjust prefix/numbering as needed)
    public const string CountryCodeAlreadyExists = "CoreFW:Countries:00001";
    public const string CountryHasProvincesCannotDelete = "CoreFW:Countries:00002";
}
