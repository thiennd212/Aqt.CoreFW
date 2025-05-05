using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.BDocuments.Entities;
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.BDocuments;

public interface IBDocumentRepository : IRepository<BDocument, Guid>
{
    /// <summary>
    /// Finds a document by its unique code.
    /// Tìm một hồ sơ bằng mã duy nhất của nó.
    /// </summary>
    Task<BDocument?> FindByCodeAsync(
        [NotNull] string code,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a document code already exists.
    /// Kiểm tra xem mã hồ sơ đã tồn tại hay chưa.
    /// </summary>
    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of documents based on filter criteria.
    /// Lấy danh sách hồ sơ dựa trên các tiêu chí lọc.
    /// </summary>
    Task<List<BDocument>> GetListAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? workflowStatusId = null,
        DateTime? submissionDateFrom = null,
        DateTime? submissionDateTo = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of documents based on filter criteria.
    /// Lấy số lượng hồ sơ dựa trên các tiêu chí lọc.
    /// </summary>
    Task<long> GetCountAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? workflowStatusId = null,
        DateTime? submissionDateFrom = null,
        DateTime? submissionDateTo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document by ID, including its associated data.
    /// Lấy một hồ sơ theo ID, bao gồm cả dữ liệu liên quan của nó.
    /// </summary>
    Task<BDocument?> GetWithDataAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Add other specific query methods if needed
} 