using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Identity; // Namespace chứa OrganizationUnit entity chuẩn
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.OrganizationUnits; // Namespace Repository Interface tùy chỉnh

public interface IOrganizationUnitRepository : IRepository<OrganizationUnit, Guid>
{
    /// <summary>
    /// Finds an organization unit by its unique MANUAL code (extended property).
    /// </summary>
    Task<OrganizationUnit?> FindByManualCodeAsync(
        [NotNull] string manualCode,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an organization unit with the given MANUAL code already exists.
    /// </summary>
    Task<bool> ManualCodeExistsAsync(
        [NotNull] string manualCode,
        Guid? excludedId = null, // Optional ID to exclude (for updates)
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of organization units with their direct children count, including extended properties.
    /// Useful for building the initial levels or specific parts of the tree.
    /// </summary>
    Task<List<OrganizationUnit>> GetListWithDetailsAsync(
        string? filterText = null,      // Filter by ManualCode or DisplayName
        Guid? parentId = null,          // Filter by parent ID (null for root)
        // Thêm filter theo Status (thuộc tính mở rộng) nếu cần
        string? sorting = null,         // Sorting parameters (có thể sort theo Order mở rộng)
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of organization units based on filtering parameters.
    /// </summary>
    Task<long> GetCountAsync(
        string? filterText = null,
        Guid? parentId = null,
        // Thêm filter theo Status (thuộc tính mở rộng) nếu cần
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all children of a specific parent, optionally recursively.
    /// Includes extended properties.
    /// </summary>
    Task<List<OrganizationUnit>> GetChildrenAsync(
        Guid? parentId,
        bool recursive = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all organization units, optimized for tree view display.
    /// Includes necessary extended properties like Order, Status.
    /// </summary>
    Task<List<OrganizationUnit>> GetAllForTreeAsync(CancellationToken cancellationToken = default);

    // Có thể thêm các phương thức khác nếu cần, ví dụ: GetListByIdsAsync
} 