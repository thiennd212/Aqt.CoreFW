    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Domain.OrgStructure.Entities; // Reference to the PositionRole entity
    using Volo.Abp.Domain.Repositories;

    namespace Aqt.CoreFW.Domain.OrgStructure.Repositories;

    /// <summary>
    /// Defines the repository interface for the <see cref="PositionRole"/> join entity,
    /// which uses a composite primary key (PositionId, RoleId).
    /// Inherits from the non-generic IRepository as standard generic methods don't apply directly.
    /// </summary>
    public interface IPositionRoleRepository : IRepository // Non-generic
    {
        /// <summary>
        /// Finds a specific PositionRole entry by its composite key.
        /// </summary>
        /// <param name="positionId">The position ID part of the key.</param>
        /// <param name="roleId">The role ID part of the key.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="PositionRole"/> if found; otherwise, null.</returns>
        Task<PositionRole?> FindAsync(
            Guid positionId,
            Guid roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of Role IDs associated with a specific Position ID.
        /// </summary>
        /// <param name="positionId">The ID of the position.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of Role IDs.</returns>
        Task<List<Guid>> GetRoleIdsByPositionIdAsync(
            Guid positionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a distinct list of Role IDs associated with multiple Position IDs.
        /// Useful for synchronizing roles for a user who has multiple positions.
        /// </summary>
        /// <param name="positionIds">A list of Position IDs.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A distinct list of Role IDs.</returns>
        Task<List<Guid>> GetDistinctRoleIdsByPositionIdsAsync(
            List<Guid> positionIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all PositionRole entries associated with a specific Position ID.
        /// </summary>
        /// <param name="positionId">The ID of the position.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="PositionRole"/> entities.</returns>
        Task<List<PositionRole>> GetListByPositionIdAsync(
            Guid positionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specific PositionRole entry by its composite key.
        /// </summary>
        /// <param name="positionId">The position ID part of the key.</param>
        /// <param name="roleId">The role ID part of the key.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task DeleteAsync(Guid positionId, Guid roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes all PositionRole entries associated with a specific Position ID.
        /// Typically used when a Position is deleted.
        /// </summary>
        /// <param name="positionId">The ID of the position whose roles should be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task DeleteByPositionIdAsync(
            Guid positionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a new PositionRole entity.
        /// </summary>
        /// <param name="positionRole">The entity to insert.</param>
        /// <param name="autoSave">Set true to automatically save changes to database. Default: false.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task InsertAsync(
            PositionRole positionRole,
            bool autoSave = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts multiple PositionRole entities.
        /// </summary>
        /// <param name="positionRoles">The collection of entities to insert.</param>
        /// <param name="autoSave">Set true to automatically save changes to database. Default: false.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task InsertManyAsync(
            IEnumerable<PositionRole> positionRoles,
            bool autoSave = false,
            CancellationToken cancellationToken = default);

        // GetCount methods might be needed if complex counting logic arises
        // Task<long> GetCountByPositionIdAsync(Guid positionId, CancellationToken cancellationToken = default);
    }