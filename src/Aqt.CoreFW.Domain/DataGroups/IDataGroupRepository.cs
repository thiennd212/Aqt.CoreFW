using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.DataGroups.Entities; // Namespace Entity
using Aqt.CoreFW.DataGroups; // Namespace Enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.DataGroups; // Namespace Repository Interface

public interface IDataGroupRepository : IRepository<DataGroup, Guid>
{
    /// <summary>
    /// Finds a data group by its unique code.
    /// </summary>
    Task<DataGroup?> FindByCodeAsync(
        [NotNull] string code,
        bool includeDetails = false, // Thêm tùy chọn includeDetails
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a data group with the given code already exists.
    /// </summary>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludedId = null, // Optional ID to exclude (for updates)
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of data groups based on filtering, sorting, and paging parameters.
    /// </summary>
    Task<List<DataGroup>> GetListAsync(
        string? filterText = null,         // Filter by Code or Name
        DataGroupStatus? status = null,    // Filter by Status
        Guid? parentId = null,             // Filter by ParentId (use null for root level)
        bool? parentIdIsNull = null,       // Explicitly filter for root level if needed (optional)
        string? sorting = null,            // Sorting parameters
        int maxResultCount = int.MaxValue, // Max items
        int skipCount = 0,                 // Items to skip
        bool includeDetails = false,       // Include Parent navigation property if needed
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of data groups based on filtering parameters.
    /// </summary>
    Task<long> GetCountAsync(
        string? filterText = null,
        DataGroupStatus? status = null,
        Guid? parentId = null,
        bool? parentIdIsNull = null, // Match GetListAsync
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of direct child data groups for a given parent ID.
    /// </summary>
    Task<List<DataGroup>> GetChildrenAsync(
         Guid parentId,
         bool includeDetails = false,
         CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all descendant data groups (children, grandchildren, etc.) for a given parent ID.
    /// Implement this efficiently, possibly using recursive CTEs in SQL if applicable.
    /// </summary>
    Task<List<DataGroup>> GetAllDescendantsAsync(
        Guid parentId,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Efficiently gets the IDs of all descendant data groups for a given parent ID.
    /// Crucial for cycle detection in DataGroupManager.
    /// </summary>
    Task<List<Guid>> GetAllDescendantIdsAsync(
        Guid parentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a data group has any child data groups.
    /// Used for delete validation.
    /// </summary>
    Task<bool> HasChildrenAsync(
         Guid id,
         CancellationToken cancellationToken = default);

    // /// <summary>
    ///// Gets a data group by ID, optionally including details (like Parent).
    ///// </summary>
    //Task<DataGroup> GetAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default); // Overload GetAsync

    ///// <summary>
    ///// Finds a data group by ID, optionally including details.
    ///// </summary>
    //Task<DataGroup> FindAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default); // Overload FindAsync
} 