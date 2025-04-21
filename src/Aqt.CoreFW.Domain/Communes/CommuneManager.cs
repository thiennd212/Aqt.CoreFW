using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Domain.Districts; // Assuming IDistrictRepository exists
using Aqt.CoreFW.Domain.Provinces; // Assuming IProvinceRepository exists
using Aqt.CoreFW.Domain.Shared.Communes;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Aqt.CoreFW.Domain.Communes;

public class CommuneManager : DomainService
{
    private readonly ICommuneRepository _communeRepository;
    private readonly IProvinceRepository _provinceRepository;
    private readonly IDistrictRepository _districtRepository;

    public CommuneManager(
        ICommuneRepository communeRepository,
        IProvinceRepository provinceRepository,
        IDistrictRepository districtRepository)
    {
        _communeRepository = communeRepository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
    }

    public async Task<Commune> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        Guid provinceId,
        Guid? districtId,
        int order = 0,
        [CanBeNull] string? description = null,
        CommuneStatus status = CommuneStatus.Active,
        [CanBeNull] DateTime? lastSyncedTime = null,
        [CanBeNull] string? syncId = null,
        [CanBeNull] string? syncCode = null)
    {
        await CheckCodeDuplicationAsync(code);
        await ValidateProvinceAndDistrictAsync(provinceId, districtId);

        var commune = new Commune(
            GuidGenerator.Create(),
            code,
            name,
            provinceId,
            districtId,
            order,
            description,
            status,
            lastSyncedTime,
            syncId,
            syncCode
        );

        return commune;
    }

    public async Task<Commune> UpdateAsync(
        [NotNull] Commune commune,
        [NotNull] string name,
        Guid? districtId,
        int order,
        [CanBeNull] string? description,
        CommuneStatus status,
        [CanBeNull] DateTime? lastSyncedTime,
        [CanBeNull] string? syncId,
        [CanBeNull] string? syncCode)
    {
        if (commune.DistrictId != districtId)
        {
            // Validate relationship before changing
            await ValidateProvinceAndDistrictAsync(commune.ProvinceId, districtId);
            commune.SetDistrictInternal(districtId);
        }

        commune.SetName(name);
        commune.SetOrder(order);
        commune.SetDescription(description);
        commune.SetSyncInfo(lastSyncedTime, syncId, syncCode);

        if (status == CommuneStatus.Active) commune.Activate(); else commune.Deactivate();

        return commune;
    }

    private async Task CheckCodeDuplicationAsync([NotNull] string code, Guid? excludedId = null)
    {
        if (await _communeRepository.CodeExistsAsync(code, excludedId))
        {
            throw new BusinessException(CoreFWDomainErrorCodes.CommuneCodeAlreadyExists)
                .WithData("code", code);
        }
    }

    private async Task ValidateProvinceAndDistrictAsync(Guid provinceId, Guid? districtId)
    {
        // Check Province existence
        if (!await _provinceRepository.AnyAsync(p => p.Id == provinceId))
        {
            // Use specific error code for Commune context
            throw new BusinessException(CoreFWDomainErrorCodes.ProvinceNotFoundForCommune);
        }

        // If DistrictId is provided, check its validity and relationship
        if (districtId.HasValue)
        {
            var district = await _districtRepository.FindAsync(districtId.Value);
            if (district == null)
            {
                throw new BusinessException(CoreFWDomainErrorCodes.DistrictNotFoundForCommune);
            }
            // Crucially, check if the district belongs to the specified province
            if (district.ProvinceId != provinceId)
            {
                throw new BusinessException(CoreFWDomainErrorCodes.InvalidDistrictForSelectedProvince);
            }
        }
    }
}