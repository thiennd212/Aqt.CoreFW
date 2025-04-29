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
    Task<BDocument?> FindByMaHoSoAsync(
        [NotNull] string maHoSo,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<bool> MaHoSoExistsAsync(
        [NotNull] string maHoSo,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<List<BDocument>> GetListAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? trangThaiHoSoId = null,
        DateTime? ngayNopFrom = null,
        DateTime? ngayNopTo = null,
        // Thêm filter nếu cần
        bool? dangKyNhanQuaBuuDien = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? trangThaiHoSoId = null,
        DateTime? ngayNopFrom = null,
        DateTime? ngayNopTo = null,
         // Thêm filter nếu cần
        bool? dangKyNhanQuaBuuDien = null,
        CancellationToken cancellationToken = default);

    Task<BDocument?> GetWithDataAsync( // Lấy kèm BDocumentData
        Guid id,
        CancellationToken cancellationToken = default);

    // Add other specific query methods if needed
} 