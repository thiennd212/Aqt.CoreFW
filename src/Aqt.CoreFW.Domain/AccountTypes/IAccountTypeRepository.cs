using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.AccountTypes.Entities; // Namespace Entity
using Aqt.CoreFW.AccountTypes; // Namespace Enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.AccountTypes; // Namespace Repository Interface

public interface IAccountTypeRepository : IRepository<AccountType, Guid>
{
    /// <summary>
    /// Finds an account type by its unique code.
    /// </summary>
    Task<AccountType?> FindByCodeAsync(
        [NotNull] string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an account type with the given code already exists.
    /// </summary>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludedId = null, // Optional ID to exclude (for updates)
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of account types based on filtering, sorting, and paging parameters.
    /// </summary>
    Task<List<AccountType>> GetListAsync(
        string? filterText = null,              // Filter by Code or Name
        AccountTypeStatus? status = null,       // Filter by Status
        string? sorting = null,                 // Sorting parameters
        int maxResultCount = int.MaxValue,      // Max items
        int skipCount = 0,                      // Items to skip
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of account types based on filtering parameters.
    /// </summary>
    Task<long> GetCountAsync(
        string? filterText = null,
        AccountTypeStatus? status = null,
        CancellationToken cancellationToken = default);

    // Có thể thêm các phương thức truy vấn đặc thù khác nếu cần
}