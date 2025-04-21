using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Domain.Shared.Communes; // For CommuneStatus enum
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.Communes;

public interface ICommuneRepository : IRepository<Commune, Guid>
{
    Task<Commune?> FindByCodeAsync(
        [NotNull] string code,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    Task<List<Commune>> GetListAsync(
        string? filterText = null,
        CommuneStatus? status = null,
        Guid? provinceId = null,
        Guid? districtId = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false, // Consider how this will be implemented in EFCore
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        CommuneStatus? status = null,
        Guid? provinceId = null,
        Guid? districtId = null,
        CancellationToken cancellationToken = default);

    Task<List<Commune>> GetListByProvinceIdAsync(
        Guid provinceId,
        CancellationToken cancellationToken = default);

    Task<List<Commune>> GetListByDistrictIdAsync(
        Guid districtId,
        CancellationToken cancellationToken = default);
}