using Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos; // AccountTypeExcelDto
using Aqt.CoreFW.Domain.AccountTypes.Entities; // AccountType Entity
using Aqt.CoreFW.Localization; // CoreFWResource for L[]
using Aqt.CoreFW.AccountTypes; // Enum namespace
using AutoMapper;
using Microsoft.Extensions.Localization; // Required for IStringLocalizer
using Volo.Abp.DependencyInjection; // Required for ITransientDependency

namespace Aqt.CoreFW.Application.AccountTypes; // Namespace for AccountType Application layer

/// <summary>
/// AutoMapper mapping action to handle specific logic when mapping
/// from AccountType entity to AccountTypeExcelDto.
/// Specifically handles localization of the Status enum.
/// </summary>
public class AccountTypeToExcelMappingAction
    : IMappingAction<AccountType, AccountTypeExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;

    public AccountTypeToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
    {
        _localizer = localizer;
    }

    public void Process(AccountType source, AccountTypeExcelDto destination, ResolutionContext context)
    {
        // Localize the Status enum based on its value using the key format defined in Domain.Shared plan
        // Example key: "Enum:AccountTypeStatus.0" or "Enum:AccountTypeStatus.1"
        destination.StatusText = _localizer[$"Enum:AccountTypeStatus.{(int)source.Status}"];
    }
}