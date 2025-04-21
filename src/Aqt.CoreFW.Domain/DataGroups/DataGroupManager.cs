using System;
using System.Linq; // Thêm using Linq
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.DataGroups.Entities; // Entity namespace
using Aqt.CoreFW.DataGroups; // Enum namespace
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids; // Thêm using GuidGenerator

namespace Aqt.CoreFW.Domain.DataGroups; // Domain Service namespace

/// <summary>
/// Domain service for managing DataGroups, ensuring consistency, code uniqueness,
/// and handling hierarchical relationship rules.
/// </summary>
public class DataGroupManager : DomainService
{
    private readonly IDataGroupRepository _dataGroupRepository;
    private readonly IGuidGenerator _guidGenerator; // Inject IGuidGenerator

    public DataGroupManager(IDataGroupRepository dataGroupRepository, IGuidGenerator guidGenerator) // Inject IGuidGenerator
    {
        _dataGroupRepository = dataGroupRepository;
        _guidGenerator = guidGenerator; // Store injected GuidGenerator
    }

    /// <summary>
    /// Creates a new valid DataGroup entity.
    /// </summary>
    /// <returns>The created DataGroup entity.</returns>
    /// <exception cref="BusinessException">Thrown if the code already exists or the parent is invalid.</exception>
    public async Task<DataGroup> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        Guid? parentId = null,
        int order = 0,
        [CanBeNull] string? description = null,
        DataGroupStatus status = DataGroupStatus.Active,
        [CanBeNull] DateTime? lastSyncDate = null,
        [CanBeNull] Guid? syncRecordId = null,
        [CanBeNull] string? syncRecordCode = null)
    {
        // 1. Check for duplicate Code
        await CheckCodeDuplicationAsync(code);

        // 2. Validate ParentId (if provided)
        if (parentId.HasValue)
        {
            await ValidateParentExistsAsync(parentId.Value);
            // Cycle check is inherently handled as a new entity cannot be its own parent or ancestor
        }

        // 3. Create the entity (using internal constructor)
        var dataGroup = new DataGroup(
            _guidGenerator.Create(), // Use injected GuidGenerator
            code,
            name,
            parentId, // Pass validated parentId
            order,
            description,
            status,
            lastSyncDate,
            syncRecordId,
            syncRecordCode
        );

        return dataGroup; // Repository will insert it
    }

    /// <summary>
    /// Updates an existing DataGroup entity.
    /// Code cannot be changed. ParentId changes are handled by ChangeParentAsync.
    /// </summary>
    public async Task<DataGroup> UpdateAsync( // Made async as ValidateParent might be async
        [NotNull] DataGroup dataGroup, // Use existing entity
        [NotNull] string name,
        // Guid? parentId, // Parent change handled separately
        int order,
        [CanBeNull] string? description,
        DataGroupStatus status,
        [CanBeNull] DateTime? lastSyncDate,
        [CanBeNull] Guid? syncRecordId,
        [CanBeNull] string? syncRecordCode)
    {
        // Code is immutable, no check needed here.

        // Validate and set other properties using entity's public methods
        dataGroup.SetName(name);
        dataGroup.SetOrder(order);
        dataGroup.SetDescription(description);
        dataGroup.SetSyncInfo(lastSyncDate, syncRecordId, syncRecordCode);

        if (status == DataGroupStatus.Active) dataGroup.Activate(); else dataGroup.Deactivate();

        // ParentId change requires separate validation logic via ChangeParentAsync
        // if (dataGroup.ParentId != parentId)
        // {
        //     await ChangeParentAsync(dataGroup, parentId); // Call dedicated method if needed
        // }

        // No need to call Repository.UpdateAsync here, UnitOfWork handles it
        return dataGroup; // Return the updated entity
    }

    /// <summary>
    /// Changes the parent of a DataGroup, ensuring no cycles are created.
    /// </summary>
    /// <param name="dataGroup">The data group to modify.</param>
    /// <param name="newParentId">The new parent ID (null for root).</param>
    /// <exception cref="BusinessException">Thrown if the parent change creates a cycle or parent doesn't exist.</exception>
    public async Task ChangeParentAsync([NotNull] DataGroup dataGroup, Guid? newParentId)
    {
        // 1. Check if parent is changing
        if (dataGroup.ParentId == newParentId)
        {
            return; // No change needed
        }

        // 2. Prevent setting parent to self
        if (dataGroup.Id == newParentId)
        {
            throw new BusinessException(CoreFWDomainErrorCodes.CannotSetParentToSelfOrChild)
               .WithData("id", dataGroup.Id)
               .WithData("newParentId", newParentId);
        }

        // 3. Validate new parent exists (if not null) and prevent cycles
        if (newParentId.HasValue)
        {
            await ValidateParentExistsAsync(newParentId.Value);
            await CheckParentCycleAsync(dataGroup, newParentId.Value);
        }

        // 4. Set the parent using the internal method
        dataGroup.SetParentIdInternal(newParentId);
    }

    /// <summary>
    /// Prepares a data group for deletion by checking necessary conditions.
    /// </summary>
    /// <param name="dataGroup">The data group to check.</param>
    /// <exception cref="BusinessException">Thrown if the data group has children.</exception>
    public async Task ValidateBeforeDeleteAsync([NotNull] DataGroup dataGroup)
    {
        if (await _dataGroupRepository.HasChildrenAsync(dataGroup.Id))
        {
            throw new BusinessException(CoreFWDomainErrorCodes.CannotDeleteDataGroupWithChildren)
                .WithData("code", dataGroup.Code);
        }
    }

    // --- Helper validation methods ---

    private async Task CheckCodeDuplicationAsync([NotNull] string code, Guid? excludedId = null)
    {
        if (await _dataGroupRepository.CodeExistsAsync(code, excludedId))
        {
            throw new BusinessException(CoreFWDomainErrorCodes.DataGroupCodeAlreadyExists)
                .WithData("code", code);
        }
    }

    private async Task ValidateParentExistsAsync(Guid parentId)
    {
        // Check if the potential parent group actually exists
        var parentExists = await _dataGroupRepository.FindAsync(parentId) != null;
        if (!parentExists)
        {
            // Consider adding a specific error code for ParentNotFound
            throw new BusinessException("CoreFW:DataGroups:ParentNotFound") // Example Error Code
               .WithData("parentId", parentId);
        }
    }

    private async Task CheckParentCycleAsync([NotNull] DataGroup dataGroup, Guid newParentId)
    {
        // To prevent cycles, the new parent cannot be one of the current group's descendants.
        var descendantIds = await _dataGroupRepository.GetAllDescendantIdsAsync(dataGroup.Id);
        if (descendantIds.Contains(newParentId))
        {
            throw new BusinessException(CoreFWDomainErrorCodes.CannotSetParentToSelfOrChild)
                .WithData("id", dataGroup.Id)
                .WithData("newParentId", newParentId);
        }
    }
}
