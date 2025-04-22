using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.AccountTypes; // Contracts namespace
using Aqt.CoreFW.Application.Contracts.AccountTypes.Dtos; // DTOs namespace
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Lookup DTO namespace
using Aqt.CoreFW.Shared.Services; // IAbpExcelExportHelper namespace (Đảm bảo namespace này đúng)
using Aqt.CoreFW.Domain.AccountTypes; // Domain Service and Repository Interface namespace
using Aqt.CoreFW.Domain.AccountTypes.Entities; // Entity namespace
using Aqt.CoreFW.Localization; // Resource namespace
using Aqt.CoreFW.Permissions; // Permissions namespace
using Aqt.CoreFW.AccountTypes; // Enum namespace from Domain.Shared
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // For IRemoteStreamContent
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping; // For ObjectMapper

namespace Aqt.CoreFW.Application.AccountTypes; // Application Service namespace

[Authorize(CoreFWPermissions.AccountTypes.Default)] // Default policy for read operations
public class AccountTypeAppService :
    CrudAppService<
        AccountType,                 // Entity
        AccountTypeDto,              // DTO Read
        Guid,                        // Primary Key
        GetAccountTypesInput,        // DTO for GetList input
        CreateUpdateAccountTypeDto>, // DTO for Create/Update input
    IAccountTypeAppService           // Implement the contract interface
{
    private readonly IAccountTypeRepository _accountTypeRepository;
    private readonly AccountTypeManager _accountTypeManager; // Inject Domain Service
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper _excelExportHelper; // Đảm bảo interface này tồn tại và được đăng ký DI

    // Constructor injection
    public AccountTypeAppService(
        IRepository<AccountType, Guid> repository, // Base repository from CrudAppService
        IAccountTypeRepository accountTypeRepository, // Specific repository
        AccountTypeManager accountTypeManager,
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper excelExportHelper) // Đảm bảo interface này tồn tại
        : base(repository)
    {
        _accountTypeRepository = accountTypeRepository;
        _accountTypeManager = accountTypeManager;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        // Set permission policies defined in Application.Contracts
        GetPolicyName = CoreFWPermissions.AccountTypes.Default;
        GetListPolicyName = CoreFWPermissions.AccountTypes.Default;
        CreatePolicyName = CoreFWPermissions.AccountTypes.Create;
        UpdatePolicyName = CoreFWPermissions.AccountTypes.Update;
        DeletePolicyName = CoreFWPermissions.AccountTypes.Delete;
    }

    [Authorize(CoreFWPermissions.AccountTypes.Create)]
    public override async Task<AccountTypeDto> CreateAsync(CreateUpdateAccountTypeDto input)
    {
        // Use AccountTypeManager to create the entity, handling code uniqueness validation
        var entity = await _accountTypeManager.CreateAsync(
            input.Code,
            input.Name,
            input.Order,
            input.Description,
            input.Status
        // Sync fields are not managed via this DTO
        );

        await Repository.InsertAsync(entity, autoSave: true);

        // Map entity to DTO for the response
        return ObjectMapper.Map<AccountType, AccountTypeDto>(entity);
    }

    [Authorize(CoreFWPermissions.AccountTypes.Update)]
    public override async Task<AccountTypeDto> UpdateAsync(Guid id, CreateUpdateAccountTypeDto input)
    {
        var entity = await GetEntityByIdAsync(id);

        // Check if Code is being changed (Code is immutable as per Domain plan)
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
            // Throw an exception or handle as needed
            throw new UserFriendlyException(_localizer["AccountTypeCodeCannotBeChanged"]); // Thêm key localization này
        }

        // Use AccountTypeManager to update the entity
        // (Manager's UpdateAsync calls entity's public methods)
        entity = await _accountTypeManager.UpdateAsync(
            entity,
            input.Name,
            input.Order,
            input.Description,
            input.Status,
            entity.LastSyncDate, // Keep existing sync info
            entity.SyncRecordId,
            entity.SyncRecordCode
        );

        await Repository.UpdateAsync(entity, autoSave: true);

        // Map updated entity to DTO
        return ObjectMapper.Map<AccountType, AccountTypeDto>(entity);
    }

    [Authorize(CoreFWPermissions.AccountTypes.Delete)]
    public override async Task DeleteAsync(Guid id)
    {
        // Optional: Add checks here if AccountType deletion has dependencies
        // Example: CheckIfAccountTypeIsInUse(id);
        await base.DeleteAsync(id); // Performs soft delete
    }

    // --- Custom Lookup Method ---

    [AllowAnonymous] // Hoặc [Authorize(CoreFWPermissions.AccountTypes.Default)] nếu chỉ user đăng nhập mới được lookup
    public async Task<ListResultDto<AccountTypeLookupDto>> GetLookupAsync()
    {
        // Get only active account types for lookup
        var queryable = await Repository.GetQueryableAsync();
        var query = queryable
            .Where(at => at.Status == AccountTypeStatus.Active)
            .OrderBy(at => at.Order).ThenBy(at => at.Name); // Order for user-friendly dropdown

        var accountTypes = await AsyncExecuter.ToListAsync(query);

        // Map to Lookup DTO
        var lookupDtos = ObjectMapper.Map<List<AccountType>, List<AccountTypeLookupDto>>(accountTypes);
        return new ListResultDto<AccountTypeLookupDto>(lookupDtos);
    }

    // --- Overridden GetListAsync for Filtering ---

    public override async Task<PagedResultDto<AccountTypeDto>> GetListAsync(GetAccountTypesInput input)
    {
        // 1. Get total count based on filters
        var totalCount = await _accountTypeRepository.GetCountAsync(
            filterText: input.Filter,
            status: input.Status
        );

        // 2. Get paginated list of entities based on filters and sorting
        var accountTypes = await _accountTypeRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            sorting: input.Sorting ?? nameof(AccountType.Order), // Default sort by Order
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        // 3. Map entities to DTOs
        var accountTypeDtos = ObjectMapper.Map<List<AccountType>, List<AccountTypeDto>>(accountTypes);

        return new PagedResultDto<AccountTypeDto>(totalCount, accountTypeDtos);
    }

    // --- Excel Export ---

    [Authorize(CoreFWPermissions.AccountTypes.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetAccountTypesInput input)
    {
        // 1. Get all matching entities (no paging) for export
        var accountTypes = await _accountTypeRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            sorting: input.Sorting ?? nameof(AccountType.Order),
            maxResultCount: int.MaxValue, // Get all records
            skipCount: 0
        );

        if (!accountTypes.Any())
        {
            // Throw exception if no data to export
            throw new UserFriendlyException(_localizer["NoDataFoundToExport"]); // Thêm key localization này
        }

        // 2. Map entities to Excel DTOs
        // The AccountTypeToExcelMappingAction handles StatusText localization.
        var excelDtos = ObjectMapper.Map<List<AccountType>, List<AccountTypeExcelDto>>(accountTypes);

        // 3. Create Excel file using the helper service
        return await _excelExportHelper.ExportToExcelAsync(
            items: excelDtos,
            filePrefix: "AccountTypes", // Suggested filePrefix
            sheetName: "AccountTypesData" // Suggested sheet name
         );
    }

    // --- Localization Keys to Add ---
    // "AccountTypeCodeCannotBeChanged": "Account type code cannot be changed." / "Không thể thay đổi mã loại tài khoản."
    // "NoDataFoundToExport": "No data found to export." / "Không tìm thấy dữ liệu để xuất file."
}