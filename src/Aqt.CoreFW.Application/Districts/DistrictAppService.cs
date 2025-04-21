using Aqt.CoreFW.Application.Contracts.Districts;
using Aqt.CoreFW.Application.Contracts.Districts.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Domain.Districts;
using Aqt.CoreFW.Domain.Districts.Entities;
using Aqt.CoreFW.Domain.Provinces.Entities;
using Aqt.CoreFW.Domain.Shared.Districts;
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Permissions;
using Aqt.CoreFW.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Aqt.CoreFW.Application.Districts;

[Authorize(CoreFWPermissions.Districts.Default)]
public class DistrictAppService :
    CrudAppService<
        District,
        DistrictDto,
        Guid,
        GetDistrictsInput,
        CreateUpdateDistrictDto>,
    IDistrictAppService
{
    private readonly IDistrictRepository _districtRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAbpExcelExportHelper _excelExportHelper;

    public DistrictAppService(
        IDistrictRepository districtRepository,
        IRepository<Province, Guid> provinceRepository,
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> localizer,
        IAbpExcelExportHelper excelExportHelper)
        : base(districtRepository)
    {
        _districtRepository = districtRepository;
        _provinceRepository = provinceRepository;
        _guidGenerator = guidGenerator;
        _localizer = localizer;
        _excelExportHelper = excelExportHelper;

        GetPolicyName = CoreFWPermissions.Districts.Default;
        GetListPolicyName = CoreFWPermissions.Districts.Default;
        CreatePolicyName = CoreFWPermissions.Districts.Create;
        UpdatePolicyName = CoreFWPermissions.Districts.Update;
        DeletePolicyName = CoreFWPermissions.Districts.Delete;
    }

    [Authorize(CoreFWPermissions.Districts.Create)]
    public override async Task<DistrictDto> CreateAsync(CreateUpdateDistrictDto input)
    {
        var province = await _provinceRepository.FindAsync(input.ProvinceId);
        if (province == null)
        {
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.ProvinceNotFound]);
        }

        var existingCode = await _districtRepository.FindByCodeAsync(input.Code);
        if (existingCode != null)
        {
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.DistrictCodeAlreadyExists, input.Code]);
        }

        // Optional: Check for duplicate name within the province if required by business logic
        // var existingName = await _districtRepository.FindByNameAndProvinceAsync(input.Name, input.ProvinceId);
        // if (existingName != null)
        // {
        //     throw new UserFriendlyException($"District name '{input.Name}' already exists in province '{province.Name}'."); // TODO: Localize & Add Error Code
        // }

        var entity = new District(
            _guidGenerator.Create(),
            input.Code,
            input.Name,
            input.ProvinceId,
            input.Order,
            input.Description,
            input.Status,
            null, null, null
        );

        await _districtRepository.InsertAsync(entity, autoSave: true);

        var dto = ObjectMapper.Map<District, DistrictDto>(entity);
        dto.ProvinceName = province.Name;
        return dto;
    }

    [Authorize(CoreFWPermissions.Districts.Update)]
    public override async Task<DistrictDto> UpdateAsync(Guid id, CreateUpdateDistrictDto input)
    {
        var entity = await _districtRepository.GetAsync(id);

        var province = await _provinceRepository.FindAsync(input.ProvinceId);
        if (province == null)
        {
            // Fallback to existing province if ProvinceId wasn't found (e.g., if it could be changed)
            // Since ProvinceId is immutable here, this mainly gets the name for the DTO.
            province = await _provinceRepository.GetAsync(entity.ProvinceId);
        }

        if (entity.Code != input.Code)
        {
            var existingCode = await _districtRepository.FindByCodeAsync(input.Code);
            if (existingCode != null && existingCode.Id != entity.Id)
            {
                throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.DistrictCodeAlreadyExists, input.Code]);
            }
            // If Code were mutable:
            // entity.SetCodeInternal(input.Code);
        }

        if (entity.Name != input.Name)
        {
            // Optional: Check for duplicate name within the province (excluding self) if required
            // var existingName = await _districtRepository.FindByNameAndProvinceAsync(input.Name, entity.ProvinceId);
            // if (existingName != null && existingName.Id != entity.Id)
            // {
            //      throw new UserFriendlyException($"District name '{input.Name}' already exists in province '{province.Name}'."); // TODO: Localize & Add Error Code
            // }
            // Use public methods to update the entity state
            entity.SetName(input.Name);
        }

        // Use public methods to update the entity state
        entity.SetOrder(input.Order);
        entity.SetDescription(input.Description);
        // Use Activate/Deactivate methods based on input status
        if (input.Status == DistrictStatus.Active)
        {
            entity.Activate();
        }
        else
        {
            entity.Deactivate();
        }
        // entity.SetStatus(input.Status); // Replace direct call with Activate/Deactivate

        await _districtRepository.UpdateAsync(entity, autoSave: true);

        var dto = ObjectMapper.Map<District, DistrictDto>(entity);
        dto.ProvinceName = province.Name;
        return dto;
    }

    [Authorize(CoreFWPermissions.Districts.Delete)]
    public override Task DeleteAsync(Guid id)
    {
        // Optional: Add checks for dependencies before deletion if necessary
        // await CheckIfDistrictIsInUseAsync(id);
        return base.DeleteAsync(id);
    }

    [AllowAnonymous]
    public async Task<ListResultDto<DistrictLookupDto>> GetLookupAsync(Guid? provinceId = null)
    {
        var districts = await _districtRepository.GetListAsync(
            status: DistrictStatus.Active,
            provinceId: provinceId,
            sorting: nameof(District.Order) + "," + nameof(District.Name)
        );

        var lookupDtos = ObjectMapper.Map<List<District>, List<DistrictLookupDto>>(districts);
        return new ListResultDto<DistrictLookupDto>(lookupDtos);
    }

    [AllowAnonymous]
    public async Task<ListResultDto<ProvinceLookupDto>> GetProvinceLookupAsync()
    {
        // Consider filtering active provinces if Province has a status
        var provinces = await _provinceRepository.GetListAsync();
        var sortedProvinces = provinces.OrderBy(p => p.Name).ToList();

        var lookupDtos = ObjectMapper.Map<List<Province>, List<ProvinceLookupDto>>(sortedProvinces);
        return new ListResultDto<ProvinceLookupDto>(lookupDtos);
    }

    public override async Task<PagedResultDto<DistrictDto>> GetListAsync(GetDistrictsInput input)
    {
        var totalCount = await _districtRepository.GetCountAsync(
            filterText: input.Filter,
            status: input.Status,
            provinceId: input.ProvinceId
        );

        var districts = await _districtRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            provinceId: input.ProvinceId,
            sorting: input.Sorting ?? nameof(District.Order) + "," + nameof(District.Name),
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        var provinceIds = districts.Select(d => d.ProvinceId).Distinct().ToList();
        var provinces = (await _provinceRepository.GetListAsync(p => provinceIds.Contains(p.Id)))
                        .ToDictionary(p => p.Id, p => p.Name);

        var districtDtos = ObjectMapper.Map<List<District>, List<DistrictDto>>(districts);
        foreach (var dto in districtDtos)
        {
            dto.ProvinceName = provinces.TryGetValue(dto.ProvinceId, out var name) ? name : string.Empty;
        }

        return new PagedResultDto<DistrictDto>(totalCount, districtDtos);
    }

    [Authorize(CoreFWPermissions.Districts.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetDistrictsInput input)
    {
        await CheckPolicyAsync(CoreFWPermissions.Districts.Export);

        var districts = await _districtRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            provinceId: input.ProvinceId,
            sorting: input.Sorting ?? nameof(District.Order) + "," + nameof(District.Name),
            maxResultCount: int.MaxValue,
            skipCount: 0
        );

        if (!districts.Any())
        {
            throw new UserFriendlyException(_localizer["No data found to export."]);
        }

        var provinceIds = districts.Select(p => p.ProvinceId).Distinct().ToList();
        var provinces = (await _provinceRepository.GetListAsync(p => provinceIds.Contains(p.Id)))
                        .ToDictionary(p => p.Id, p => p.Name);

        var excelDtos = new List<DistrictExcelDto>();
        foreach (var district in districts)
        {
            var dto = ObjectMapper.Map<District, DistrictExcelDto>(district);
            dto.ProvinceName = provinces.TryGetValue(district.ProvinceId, out var name) ? name : string.Empty;
            excelDtos.Add(dto);
        }

        return await _excelExportHelper.ExportToExcelAsync(excelDtos,"Districts","Data");
    }
}