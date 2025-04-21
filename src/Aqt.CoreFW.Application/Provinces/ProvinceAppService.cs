using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Provinces;
using Aqt.CoreFW.Application.Contracts.Provinces.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Aqt.CoreFW.Domain.Provinces; // Namespace Province Repository Interface
// Using cho Entities
using Aqt.CoreFW.Domain.Countries.Entities;
using Aqt.CoreFW.Domain.Provinces.Entities;
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Permissions;
using Aqt.CoreFW.Domain.Shared.Provinces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Aqt.CoreFW.Domain.Countries.Repositories;
using Volo.Abp.Timing;

namespace Aqt.CoreFW.Application.Provinces;

[Authorize(CoreFWPermissions.Provinces.Default)]
public class ProvinceAppService :
    CrudAppService<
        Province,
        ProvinceDto,
        Guid,
        GetProvincesInput,
        CreateUpdateProvinceDto>,
    IProvinceAppService
{
    private readonly IProvinceRepository _provinceRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IAsyncQueryableExecuter _asyncExecuter;
    private readonly IClock _clock; // Inject IClock

    public ProvinceAppService(
        IRepository<Province, Guid> repository,
        IProvinceRepository provinceRepository,
        ICountryRepository countryRepository,
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> localizer,
        IAsyncQueryableExecuter asyncExecuter,
        IClock clock)
        : base(repository)
    {
        _provinceRepository = provinceRepository;
        _countryRepository = countryRepository;
        _guidGenerator = guidGenerator;
        _localizer = localizer;
        _asyncExecuter = asyncExecuter;
        _clock = clock; // Initialize IClock

        GetPolicyName = CoreFWPermissions.Provinces.Default;
        GetListPolicyName = CoreFWPermissions.Provinces.Default;
        CreatePolicyName = CoreFWPermissions.Provinces.Create;
        UpdatePolicyName = CoreFWPermissions.Provinces.Update;
        DeletePolicyName = CoreFWPermissions.Provinces.Delete;
    }

    [Authorize(CoreFWPermissions.Provinces.Create)]
    public override async Task<ProvinceDto> CreateAsync(CreateUpdateProvinceDto input)
    {
        if (await _provinceRepository.CodeExistsAsync(input.Code))
        {
            throw new UserFriendlyException(_localizer[CoreFWDomainErrorCodes.ProvinceCodeAlreadyExists, input.Code]);
        }

        var entity = new Province(
            _guidGenerator.Create(),
            input.Code,
            input.Name,
            input.CountryId,
            input.Order,
            input.Description,
            input.Status
        );

        await Repository.InsertAsync(entity, autoSave: true);

        var dto = ObjectMapper.Map<Province, ProvinceDto>(entity);
        var country = await _countryRepository.FindAsync(entity.CountryId);
        dto.CountryName = country?.Name ?? string.Empty;
        return dto;
    }

    [Authorize(CoreFWPermissions.Provinces.Update)]
    public override async Task<ProvinceDto> UpdateAsync(Guid id, CreateUpdateProvinceDto input)
    {
        var entity = await GetEntityByIdAsync(id);

        entity.SetName(input.Name);
        entity.SetOrder(input.Order);
        entity.SetDescription(input.Description);

        if (input.Status == ProvinceStatus.Active) entity.Activate(); else entity.Deactivate();

        await Repository.UpdateAsync(entity, autoSave: true);

        var dto = ObjectMapper.Map<Province, ProvinceDto>(entity);
        var country = await _countryRepository.FindAsync(entity.CountryId);
        dto.CountryName = country?.Name ?? string.Empty;
        return dto;
    }

    [Authorize(CoreFWPermissions.Provinces.Delete)]
    public override async Task DeleteAsync(Guid id)
    {
        await base.DeleteAsync(id);
    }

    [AllowAnonymous]
    public async Task<ListResultDto<ProvinceLookupDto>> GetLookupAsync(Guid? countryId = null)
    {
        var queryable = await Repository.GetQueryableAsync();
        var query = queryable
            .Where(p => p.Status == ProvinceStatus.Active)
            .WhereIf(countryId.HasValue, p => p.CountryId == countryId.Value)
            .OrderBy(p => p.Order).ThenBy(p => p.Name);

        var provinces = await _asyncExecuter.ToListAsync(query);
        var lookupDtos = ObjectMapper.Map<List<Province>, List<ProvinceLookupDto>>(provinces);
        return new ListResultDto<ProvinceLookupDto>(lookupDtos);
    }

    [AllowAnonymous]
    public async Task<ListResultDto<CountryLookupDto>> GetCountryLookupAsync()
    {
        // Giả sử Country có thuộc tính IsActive
        var countries = await _countryRepository.GetListAsync(includeDetails: false);
        var sortedCountries = countries.OrderBy(c => c.Name).ToList();

        var lookupDtos = ObjectMapper.Map<List<Country>, List<CountryLookupDto>>(sortedCountries);
        return new ListResultDto<CountryLookupDto>(lookupDtos);
    }

    public override async Task<PagedResultDto<ProvinceDto>> GetListAsync(GetProvincesInput input)
    {
        var totalCount = await _provinceRepository.GetCountAsync(
            filterText: input.Filter,
            status: input.Status,
            countryId: input.CountryId
        );

        var provinces = await _provinceRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            countryId: input.CountryId,
            sorting: input.Sorting ?? nameof(Province.Name),
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        var countryIds = provinces.Select(p => p.CountryId).Distinct().ToList();
        var countries = await _countryRepository.GetListAsync(c => countryIds.Contains(c.Id));
        var countryNameMap = countries.ToDictionary(c => c.Id, c => c.Name);

        var provinceDtos = ObjectMapper.Map<List<Province>, List<ProvinceDto>>(provinces);
        foreach (var dto in provinceDtos)
        {
            dto.CountryName = countryNameMap.TryGetValue(dto.CountryId, out var name) ? name : string.Empty;
        }

        return new PagedResultDto<ProvinceDto>(totalCount, provinceDtos);
    }

    [Authorize(CoreFWPermissions.Provinces.Export)]
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetProvincesInput input)
    {
        var provinces = await _provinceRepository.GetListAsync(
            filterText: input.Filter,
            status: input.Status,
            countryId: input.CountryId,
            sorting: input.Sorting ?? nameof(Province.Name),
            maxResultCount: int.MaxValue,
            skipCount: 0
        );

        var countryIds = provinces.Select(p => p.CountryId).Distinct().ToList();
        var countries = await _countryRepository.GetListAsync(c => countryIds.Contains(c.Id));
        var countryNameMap = countries.ToDictionary(c => c.Id, c => c.Name);

        var excelDtos = new List<ProvinceExcelDto>();
        foreach(var province in provinces)
        {
            var excelDto = ObjectMapper.Map<Province, ProvinceExcelDto>(province);
            excelDto.CountryName = countryNameMap.TryGetValue(province.CountryId, out var name) ? name : string.Empty;
            // StatusText được xử lý bởi ProvinceToExcelMappingAction khi ObjectMapper.Map được gọi
            excelDtos.Add(excelDto);
        }

        var stream = new MemoryStream();
        await stream.SaveAsAsync(excelDtos);
        stream.Seek(0, SeekOrigin.Begin);

        var fileName = $"Districts{_clock.Now:yyyyMMdd_HHmmss}.xlsx";
        // 6. Return the file stream
        return new RemoteStreamContent(
            stream,
            fileName: fileName, // Suggested filename
            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );
    }
}