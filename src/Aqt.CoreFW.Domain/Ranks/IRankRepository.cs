using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Ranks.Entities; // Namespace Entity
using Aqt.CoreFW.Ranks; // Namespace Enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.Ranks // Namespace Repository Interface
{
    public interface IRankRepository : IRepository<Rank, Guid>
    {
        /// <summary>
        /// Finds a rank by its unique code.
        /// </summary>
        Task<Rank?> FindByCodeAsync(
            [NotNull] string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a rank with the given code already exists.
        /// </summary>
        Task<bool> CodeExistsAsync(
            [NotNull] string code,
            Guid? excludedId = null, // Optional ID to exclude (for updates)
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of ranks based on filtering, sorting, and paging parameters.
        /// </summary>
        Task<List<Rank>> GetListAsync(
            string? filterText = null,         // Filter by Code or Name
            RankStatus? status = null,         // Filter by Status
            string? sorting = null,            // Sorting parameters
            int maxResultCount = int.MaxValue, // Max items
            int skipCount = 0,                 // Items to skip
                                               // bool includeDetails = false,    // Không cần includeDetails cho Rank hiện tại
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of ranks based on filtering parameters.
        /// </summary>
        Task<long> GetCountAsync(
            string? filterText = null,
            RankStatus? status = null,
            CancellationToken cancellationToken = default);

        // Không cần GetListByProvinceIdAsync hay GetListByDistrictIdAsync cho Rank
    }
}