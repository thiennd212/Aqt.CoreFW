using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Ranks.Entities; // Entity namespace
using Aqt.CoreFW.Ranks; // Enum namespace
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Aqt.CoreFW.Domain.Ranks // Domain Service namespace
{
    /// <summary>
    /// Domain service for managing Ranks, ensuring consistency and business rules like code uniqueness.
    /// </summary>
    public class RankManager : DomainService
    {
        private readonly IRankRepository _rankRepository;

        public RankManager(IRankRepository rankRepository)
        {
            _rankRepository = rankRepository;
        }

        /// <summary>
        /// Creates a new valid Rank entity.
        /// </summary>
        /// <returns>The created Rank entity.</returns>
        /// <exception cref="BusinessException">Thrown if the code already exists.</exception>
        public async Task<Rank> CreateAsync(
            [NotNull] string code,
            [NotNull] string name,
            int order = 0,
            RankStatus status = RankStatus.Active,
            [CanBeNull] string? description = null,            
            [CanBeNull] DateTime? lastSyncDate = null,
            [CanBeNull] Guid? syncRecordId = null,
            [CanBeNull] string? syncRecordCode = null)
        {
            // 1. Check for duplicate Code
            await CheckCodeDuplicationAsync(code);

            // 2. Create the entity (using internal constructor)
            var rank = new Rank(
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

            return rank; // Repository will insert it
        }

        /// <summary>
        /// Updates an existing Rank entity.
        /// Code cannot be changed.
        /// </summary>
        /// <param name="rank">The existing rank entity to update.</param>
        /// <param name="name">The new name.</param>
        /// <param name="order">The new order.</param>
        /// <param name="description">The new description.</param>
        /// <param name="status">The new status.</param>
        /// <param name="lastSyncDate">The new sync date.</param>
        /// <param name="syncRecordId">The new sync record ID.</param>
        /// <param name="syncRecordCode">The new sync record code.</param>
        /// <returns>The updated Rank entity.</returns>
        public Task<Rank> UpdateAsync( // Có thể là public vì logic update đơn giản, không cần async
            [NotNull] Rank rank,
            [NotNull] string name,
            int order,
            [CanBeNull] string? description,
            RankStatus status,
            [CanBeNull] DateTime? lastSyncDate,
            [CanBeNull] Guid? syncRecordId,
            [CanBeNull] string? syncRecordCode)
        {
            // Sử dụng các phương thức public của Entity để cập nhật
            rank.SetName(name);
            rank.SetOrder(order);
            rank.SetDescription(description);
            rank.SetSyncInfo(lastSyncDate, syncRecordId, syncRecordCode);

            if (status == RankStatus.Active) rank.Activate(); else rank.Deactivate();

            // Không cần gọi Repository.UpdateAsync ở đây, UnitOfWork sẽ xử lý
            return Task.FromResult(rank); // Trả về entity đã cập nhật
        }

        // --- Helper validation methods ---

        private async Task CheckCodeDuplicationAsync([NotNull] string code, Guid? excludedId = null)
        {
            if (await _rankRepository.CodeExistsAsync(code, excludedId))
            {
                // Giả định rằng CoreFWDomainErrorCodes.RankCodeAlreadyExists đã được định nghĩa
                // Ví dụ: public const string RankCodeAlreadyExists = "CoreFW:00101";
                throw new BusinessException(CoreFWDomainErrorCodes.RankCodeAlreadyExists)
                    .WithData("code", code);
            }
        }

        // Note: Không cần ValidateProvinceAndDistrictAsync cho Rank.
        // Note: Không có ChangeCodeAsync vì Code là immutable.
    }
}