using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.AccountTypes.Entities; // Entity namespace
using Aqt.CoreFW.AccountTypes; // Enum namespace
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Aqt.CoreFW.Domain.AccountTypes; // Domain Service namespace

/// <summary>
/// Domain service for managing Account Types, ensuring consistency and business rules like code uniqueness.
/// </summary>
public class AccountTypeManager : DomainService
{
    private readonly IAccountTypeRepository _accountTypeRepository;

    public AccountTypeManager(IAccountTypeRepository accountTypeRepository)
    {
        _accountTypeRepository = accountTypeRepository;
    }

    /// <summary>
    /// Creates a new valid AccountType entity.
    /// </summary>
    /// <returns>The created AccountType entity.</returns>
    /// <exception cref="BusinessException">Thrown if the code already exists.</exception>
    public async Task<AccountType> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        int order = 0,
        [CanBeNull] string? description = null,
        AccountTypeStatus status = AccountTypeStatus.Active,
        [CanBeNull] DateTime? lastSyncDate = null,
        [CanBeNull] Guid? syncRecordId = null,
        [CanBeNull] string? syncRecordCode = null)
    {
        // 1. Check for duplicate Code
        await CheckCodeDuplicationAsync(code);

        // 2. Create the entity (using internal constructor)
        var accountType = new AccountType(
            GuidGenerator.Create(),
            code,
            name,
            order,
            description,
            status,
            lastSyncDate,
            syncRecordId,
            syncRecordCode
        );

        return accountType; // Repository will insert it
    }

    /// <summary>
    /// Updates an existing AccountType entity.
    /// Code cannot be changed.
    /// </summary>
    /// <param name="accountType">The existing account type entity to update.</param>
    /// <param name="name">The new name.</param>
    /// <param name="order">The new order.</param>
    /// <param name="description">The new description.</param>
    /// <param name="status">The new status.</param>
    /// <param name="lastSyncDate">The new sync date.</param>
    /// <param name="syncRecordId">The new sync record ID.</param>
    /// <param name="syncRecordCode">The new sync record code.</param>
    /// <returns>The updated AccountType entity.</returns>
    public Task<AccountType> UpdateAsync( // Có thể là public vì logic update đơn giản, không cần async
        [NotNull] AccountType accountType,
        [NotNull] string name,
        int order,
        [CanBeNull] string? description,
        AccountTypeStatus status,
        [CanBeNull] DateTime? lastSyncDate,
        [CanBeNull] Guid? syncRecordId,
        [CanBeNull] string? syncRecordCode)
    {
        Check.NotNull(accountType, nameof(accountType));

        // Sử dụng các phương thức public của Entity để cập nhật
        accountType.SetName(name);
        accountType.SetOrder(order);
        accountType.SetDescription(description);
        accountType.SetSyncInfo(lastSyncDate, syncRecordId, syncRecordCode);

        if (status == AccountTypeStatus.Active) accountType.Activate(); else accountType.Deactivate();

        // Không cần gọi Repository.UpdateAsync ở đây, UnitOfWork sẽ xử lý
        return Task.FromResult(accountType); // Trả về entity đã cập nhật
    }

    // --- Helper validation methods ---

    private async Task CheckCodeDuplicationAsync([NotNull] string code, Guid? excludedId = null)
    {
        if (await _accountTypeRepository.CodeExistsAsync(code, excludedId))
        {
            throw new BusinessException(CoreFWDomainErrorCodes.AccountTypeCodeAlreadyExists) // Sử dụng mã lỗi đã định nghĩa
                .WithData("code", code);
        }
    }

    // Note: Không có ChangeCodeAsync vì Code là immutable.
}