using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Required for WhereIf or dynamic sorting
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Communes;
using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Shared.Services;
using Aqt.CoreFW.Domain.Communes;
using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Domain.Districts; // IDistrictRepository
using Aqt.CoreFW.Domain.Districts.Entities;
using Aqt.CoreFW.Domain.Provinces; // IProvinceRepository
using Aqt.CoreFW.Domain.Provinces.Entities;
using Aqt.CoreFW.Domain.Shared.Communes;
using Aqt.CoreFW.Domain.Shared.Districts; // Assuming DistrictStatus exists
using Aqt.CoreFW.Domain.Shared.Provinces; // Assuming ProvinceStatus exists
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping; // Needed for manual mapping / helper methods

namespace Aqt.CoreFW.Application.Communes;

[Authorize(CoreFWPermissions.Communes.Default)]
public class CommuneAppService :
    CrudAppService<
        Commune,
        CommuneDto,
        Guid,
        GetCommunesInput,
        CreateUpdateCommuneDto>,
    ICommuneAppService
{
    private readonly ICommuneRepository _communeRepository;
    private readonly IProvinceRepository _provinceRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly CommuneManager _communeManager;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper _excelExportHelper; // Inject helper

    public CommuneAppService(
        IRepository<Commune, Guid> repository, // Base CrudAppService repository
        ICommuneRepository communeRepository, // Specific repository
        IProvinceRepository provinceRepository,
        IDistrictRepository districtRepository,
        CommuneManager communeManager,
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper excelExportHelper) // Inject helper instance
        : base(repository)
    {
        _communeRepository = communeRepository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _communeManager = communeManager;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        // Set permissions policies from Contracts
        GetPolicyName = CoreFWPermissions.Communes.Default;
        GetListPolicyName = CoreFWPermissions.Communes.Default;
        CreatePolicyName = CoreFWPermissions.Communes.Create;
        UpdatePolicyName = CoreFWPermissions.Communes.Update;
        DeletePolicyName = CoreFWPermissions.Communes.Delete;
    }

    [Authorize(CoreFWPermissions.Communes.Create)]
    public override async Task<CommuneDto> CreateAsync(CreateUpdateCommuneDto input)
    {
        var entity = await _communeManager.CreateAsync(
            input.Code,
            input.Name,
            input.ProvinceId,
            input.DistrictId,
            input.Order,
            input.Description,
            input.Status
        );

        await Repository.InsertAsync(entity, autoSave: true);

        return await MapToDtoWithNamesAsync(entity);
    }

    [Authorize(CoreFWPermissions.Communes.Update)]
    public override async Task<CommuneDto> UpdateAsync(Guid id, CreateUpdateCommuneDto input)
    {
        var entity = await GetEntityByIdAsync(id); // Use base method to get entity

        // Prevent Code modification
        if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
        {
            // TODO: Add localization key "UpdatingTheCommuneCodeIsNotAllowed"
            throw new UserFriendlyException(_localizer["UpdatingTheCommuneCodeIsNotAllowed"]);
        }

        entity = await _communeManager.UpdateAsync(
            entity,
            input.Name,
            input.DistrictId,
            input.Order,
            input.Description,
            input.Status,
            entity.LastSyncedTime, // Keep existing sync info
            entity.SyncId,
            entity.SyncCode
        );

        await Repository.UpdateAsync(entity, autoSave: true);

        return await MapToDtoWithNamesAsync(entity);
    }

    [Authorize(CoreFWPermissions.Communes.Delete)]
    public override async Task DeleteAsync(Guid id)
    {
        // Optional: Check dependencies before deleting
        await base.DeleteAsync(id);
    }

    // --- Custom Lookups ---

    [AllowAnonymous] // Adjust permission if needed
    public async Task<ListResultDto<CommuneLookupDto>> GetLookupAsync(Guid? provinceId = null, Guid? districtId = null)
    {
        var queryable = await Repository.GetQueryableAsync();
        var query = queryable
            .Where(c => c.Status == CommuneStatus.Active)
            .WhereIf(provinceId.HasValue, c => c.ProvinceId == provinceId.Value)
            .WhereIf(districtId.HasValue, c => c.DistrictId == districtId.Value)
            .OrderBy(c => c.Order).ThenBy(c => c.Name);

        var communes = await AsyncExecuter.ToListAsync(query);
        var lookupDtos = ObjectMapper.Map<List<Commune>, List<CommuneLookupDto>>(communes);
        return new ListResultDto<CommuneLookupDto>(lookupDtos);
    }

    [AllowAnonymous] // Adjust permission if needed
    public async Task<ListResultDto<ProvinceLookupDto>> GetProvinceLookupAsync()
    {
        // Assuming ProvinceStatus exists in Domain.Shared.Provinces
        var provinces = await _provinceRepository.GetListAsync(p => p.Status == ProvinceStatus.Active);
        var sortedProvinces = provinces.OrderBy(p => p.Order).ThenBy(p => p.Name).ToList();
        var lookupDtos = ObjectMapper.Map<List<Province>, List<ProvinceLookupDto>>(sortedProvinces);
        return new ListResultDto<ProvinceLookupDto>(lookupDtos);
    }

    [AllowAnonymous] // Adjust permission if needed
    public async Task<ListResultDto<DistrictLookupDto>> GetDistrictLookupAsync(Guid? provinceId = null)
    {
        // Assuming DistrictStatus exists in Domain.Shared.Districts
        var queryable = await _districtRepository.GetQueryableAsync();
        var query = queryable
            .Where(d => d.Status == DistrictStatus.Active)
            .WhereIf(provinceId.HasValue, d => d.ProvinceId == provinceId.Value)
            .OrderBy(d => d.Order).ThenBy(d => d.Name);

        var districts = await AsyncExecuter.ToListAsync(query);
        var lookupDtos = ObjectMapper.Map<List<District>, List<DistrictLookupDto>>(districts);
        return new ListResultDto<DistrictLookupDto>(lookupDtos);
    }

    // --- Overridden GetListAsync for Filtering and Name Population ---
    public override async Task<PagedResultDto<CommuneDto>> GetListAsync(GetCommunesInput input)
    {
        var totalCount = await _communeRepository.GetCountAsync(
            filterText: input.Filter,
            status: input.Status,
            provinceId: input.ProvinceId,
            districtId: input.DistrictId
        );

        var communes = await _communeRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            provinceId: input.ProvinceId,
            districtId: input.DistrictId,
            sorting: input.Sorting ?? nameof(Commune.Order), // Default sort by Order then Name
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            includeDetails: false // Details (names) fetched separately
        );

        // Apply secondary sorting by Name if primary sort is Order
        if (string.IsNullOrEmpty(input.Sorting) || input.Sorting.Contains(nameof(Commune.Order), StringComparison.OrdinalIgnoreCase))
        {
            // Ensure the list is materialized before ThenBy if GetListAsync returns IQueryable
            communes = communes.OrderBy(c => c.Order).ThenBy(c => c.Name).ToList();
        }


        var communeDtos = await MapListToDtoWithNamesAsync(communes);

        return new PagedResultDto<CommuneDto>(totalCount, communeDtos);
    }

    // --- Excel Export ---

    [Authorize(CoreFWPermissions.Communes.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetCommunesInput input)
    {
        var communes = await _communeRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            provinceId: input.ProvinceId,
            districtId: input.DistrictId,
            sorting: input.Sorting ?? nameof(Commune.Order), // Default sort by Order then Name
            maxResultCount: int.MaxValue, // Get all for export
            skipCount: 0,
            includeDetails: false
        );

        // Apply secondary sorting by Name if primary sort is Order
        if (string.IsNullOrEmpty(input.Sorting) || input.Sorting.Contains(nameof(Commune.Order), StringComparison.OrdinalIgnoreCase))
        {
            // Ensure the list is materialized before ThenBy if GetListAsync returns IQueryable
            communes = communes.OrderBy(c => c.Order).ThenBy(c => c.Name).ToList();
        }

        if (!communes.Any())
        {
            // TODO: Add localization key "NoDataFoundToExport"
            throw new UserFriendlyException(_localizer["NoDataFoundToExport"]);
        }

        // Efficiently get names for all communes
        var provinceIds = communes.Select(c => c.ProvinceId).Distinct().ToList();
        var districtIds = communes.Where(c => c.DistrictId.HasValue).Select(c => c.DistrictId!.Value).Distinct().ToList();

        var provinces = await _provinceRepository.GetListAsync(p => provinceIds.Contains(p.Id));
        var provinceNameMap = provinces.ToDictionary(p => p.Id, p => p.Name);

        var districts = await _districtRepository.GetListAsync(d => districtIds.Contains(d.Id));
        var districtNameMap = districts.ToDictionary(d => d.Id, d => d.Name);

        // Map to Excel DTOs, populating names *before* the mapping action
        var excelDtos = new List<CommuneExcelDto>();
        foreach (var commune in communes)
        {
            // Manually create and populate the DTO partially first
            var dto = new CommuneExcelDto
            {
                ProvinceName = provinceNameMap.TryGetValue(commune.ProvinceId, out var provName) ? provName : string.Empty,
                DistrictName = commune.DistrictId.HasValue && districtNameMap.TryGetValue(commune.DistrictId.Value, out var distName) ? distName : null
            };

            // Use ObjectMapper to map remaining fields AND trigger the Mapping Action for StatusText
            ObjectMapper.Map(commune, dto);
            excelDtos.Add(dto);
        }
        // Use the injected helper to create the Excel file
        return await _excelExportHelper.ExportToExcelAsync(excelDtos, "Communes", "Data");
    }


    // --- Helper Methods for Populating Names ---

    private async Task<List<CommuneDto>> MapListToDtoWithNamesAsync(List<Commune> communes)
    {
        if (communes == null || !communes.Any()) return new List<CommuneDto>();

        var provinceIds = communes.Select(c => c.ProvinceId).Distinct().ToList();
        var districtIds = communes.Where(c => c.DistrictId.HasValue).Select(c => c.DistrictId!.Value).Distinct().ToList();

        // Fetch names in batches
        var provinces = await _provinceRepository.GetListAsync(p => provinceIds.Contains(p.Id));
        var provinceNameMap = provinces.ToDictionary(p => p.Id, p => p.Name);

        var districts = await _districtRepository.GetListAsync(d => districtIds.Contains(d.Id));
        var districtNameMap = districts.ToDictionary(d => d.Id, d => d.Name);

        var communeDtos = ObjectMapper.Map<List<Commune>, List<CommuneDto>>(communes);
        foreach (var dto in communeDtos)
        {
            dto.ProvinceName = provinceNameMap.TryGetValue(dto.ProvinceId, out var provName) ? provName : string.Empty;
            dto.DistrictName = dto.DistrictId.HasValue && districtNameMap.TryGetValue(dto.DistrictId.Value, out var distName) ? distName : null;
        }
        return communeDtos;
    }

    private async Task<CommuneDto> MapToDtoWithNamesAsync(Commune commune)
    {
        var dto = ObjectMapper.Map<Commune, CommuneDto>(commune);

        var province = await _provinceRepository.FindAsync(commune.ProvinceId);
        dto.ProvinceName = province?.Name ?? string.Empty;

        if (commune.DistrictId.HasValue)
        {
            var district = await _districtRepository.FindAsync(commune.DistrictId.Value);
            dto.DistrictName = district?.Name;
        }

        return dto;
    }
}