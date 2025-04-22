using Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos; // DTOs for AccountType
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO
using Aqt.CoreFW.Domain.AccountTypes.Entities; // AccountType Entity
using AutoMapper;

namespace Aqt.CoreFW.Application.AccountTypes; // Namespace for AccountType Application layer

public class AccountTypeApplicationAutoMapperProfile : Profile
{
    public AccountTypeApplicationAutoMapperProfile()
    {
        // --- AccountType Mappings ---
        CreateMap<AccountType, AccountTypeDto>(); // Direct mapping is sufficient

        CreateMap<AccountTypeDto, CreateUpdateAccountTypeDto>(); // For prepopulating edit form

        CreateMap<AccountType, AccountTypeLookupDto>(); // For account type selection dropdowns

        CreateMap<AccountType, AccountTypeExcelDto>()
            .ForMember(dest => dest.StatusText, opt => opt.Ignore()) // Handled by MappingAction
            .AfterMap<AccountTypeToExcelMappingAction>(); // Apply the action after basic mapping

        // No direct mapping from CreateUpdateAccountTypeDto to AccountType entity
        // Create/Update operations use DTO data with AccountTypeManager
    }
}