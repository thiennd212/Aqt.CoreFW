using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using CustomOuRepository = Aqt.CoreFW.Domain.OrganizationUnits.IOrganizationUnitRepository; // Alias for custom repo
using StandardOuRepository = Volo.Abp.Identity.IOrganizationUnitRepository; // Alias for standard repo
using Aqt.CoreFW.Domain.OrganizationUnits; // Domain Services, Custom Repo, Consts, Enums
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories; // For IRepository<TEntity, TKey>
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending; // For ExtraProperties access
using Volo.Abp.ObjectMapping;
using Volo.Abp.Guids; // For IGuidGenerator
using System.Linq.Dynamic.Core;
using Aqt.CoreFW.OrganizationUnits; // For OrderBy string extension

namespace Aqt.CoreFW.Application.OrganizationUnits;

[Authorize(CoreFWPermissions.OrganizationUnits.Default)]
public class OrganizationUnitAppService : ApplicationService, IOrganizationUnitAppService
{
    // Inject IRepository<OrganizationUnit, Guid> thay vì IIdentityOrganizationUnitRepository
    private readonly CustomOuRepository _customOrganizationUnitRepository;
    private readonly IRepository<OrganizationUnit, Guid> _repository; // Standard generic repository
    private readonly OrganizationUnitManager _organizationUnitManager;
    private readonly ExtendedOrganizationUnitManager _extendedOrganizationUnitManager;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    private readonly IGuidGenerator _guidGenerator; // Needed for Create workaround

    public OrganizationUnitAppService(
        CustomOuRepository customOrganizationUnitRepository,
        IRepository<OrganizationUnit, Guid> repository, // Inject standard generic repo
        OrganizationUnitManager organizationUnitManager,
        ExtendedOrganizationUnitManager extendedOrganizationUnitManager,
        IStringLocalizer<CoreFWResource> localizer,
        IGuidGenerator guidGenerator) // Inject GuidGenerator
    {
        _customOrganizationUnitRepository = customOrganizationUnitRepository;
        _repository = repository;
        _organizationUnitManager = organizationUnitManager;
        _extendedOrganizationUnitManager = extendedOrganizationUnitManager;
        _localizer = localizer;
        _guidGenerator = guidGenerator;
        // ObjectMapper is inherited
    }

    // --- GetAsync ---
    public async Task<OrganizationUnitDto> GetAsync(Guid id)
    {
        var organizationUnit = await _customOrganizationUnitRepository.GetAsync(id);
        // Map sang DTO (AutoMapper profile + Mapping Action đã xử lý)
        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
    }

    // --- GetTreeAsync ---
    public async Task<ListResultDto<OrganizationUnitTreeNodeDto>> GetTreeAsync()
    {
        var organizationUnits = await _customOrganizationUnitRepository.GetAllForTreeAsync();
        var childrenCountMap = organizationUnits
            .Where(ou => ou.ParentId.HasValue)
            .GroupBy(ou => ou.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());
        // Map sang TreeNode DTOs (AutoMapper profile + Mapping Action đã xử lý phần lớn)
        var treeNodes = ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitTreeNodeDto>>(organizationUnits);
        // Thiết lập thuộc tính Children
        foreach (var node in treeNodes)
        {
            var arrCode = node.Code.Split('.');
            if (arrCode.Length <= 1)
            {
                node.State.Opened = true; // Mở rộng node gốc
            }
            if (Guid.TryParse(node.Id, out var guidId))
            {
                node.Children = childrenCountMap.ContainsKey(guidId) && childrenCountMap[guidId] > 0;
            }
            //Nếu có con thì icon là folder, nếu không có con thì icon là file
            if (node.Children)
            {
                node.Icon = "fas fa-folder";
            }
            else
            {
                node.Icon = "fas fa-file";
            }

        }
        return new ListResultDto<OrganizationUnitTreeNodeDto>(treeNodes);
    }


    // --- CreateAsync (Workaround) ---
    [Authorize(CoreFWPermissions.OrganizationUnits.Create)]
    public async Task<OrganizationUnitDto> CreateAsync(CreateOrganizationUnitDto input)
    {
        if (!string.IsNullOrWhiteSpace(input.ManualCode))
        {
            await _extendedOrganizationUnitManager.CheckManualCodeDuplicationAsync(input.ManualCode);
        }
        // Workaround: Tạo entity trực tiếp do vấn đề với OrganizationUnitManager.CreateAsync
        var organizationUnit = new OrganizationUnit(
            _guidGenerator.Create(),
            input.DisplayName,
            input.ParentId,
            CurrentTenant.Id
        );
        await _extendedOrganizationUnitManager.SetExtendedPropertiesAsync(
            organizationUnit,
            input.ManualCode,
            input.Status,
            input.Order,
            input.Description
        );
        // Tạo OU bằng OrganizationUnitManager để tự sinh mã hệ thống
        await _organizationUnitManager.CreateAsync(organizationUnit);
        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
    }

    // --- UpdateAsync ---
    [Authorize(CoreFWPermissions.OrganizationUnits.Update)]
    public async Task<OrganizationUnitDto> UpdateAsync(Guid id, UpdateOrganizationUnitDto input)
    {
        // 1. Lấy entity hiện tại bằng repo chuẩn generic
        var organizationUnit = await _repository.GetAsync(id);

        // 2. Cập nhật DisplayName trực tiếp
        if (organizationUnit.DisplayName != input.DisplayName)
        {
            organizationUnit.DisplayName = input.DisplayName;
        }

        // 3. Lấy các thuộc tính sync hiện có từ ExtraProperties
        var lastSyncedTime = organizationUnit.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.LastSyncedTime)
                                ? organizationUnit.ExtraProperties[OrganizationUnitExtensionProperties.LastSyncedTime] as DateTime?
                                : null;
        var syncRecordId = organizationUnit.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.SyncRecordId)
                                ? organizationUnit.ExtraProperties[OrganizationUnitExtensionProperties.SyncRecordId] as string
                                : null;
        var syncRecordCode = organizationUnit.ExtraProperties.ContainsKey(OrganizationUnitExtensionProperties.SyncRecordCode)
                                ? organizationUnit.ExtraProperties[OrganizationUnitExtensionProperties.SyncRecordCode] as string
                                : null;

        // 4. Cập nhật thuộc tính mở rộng qua Extended Manager (truyền cả giá trị sync cũ)
        await _extendedOrganizationUnitManager.SetExtendedPropertiesAsync(
            organizationUnit,
            input.ManualCode,
            input.Status,
            input.Order,
            input.Description,
            lastSyncedTime,
            syncRecordId,
            syncRecordCode
        );

        // Cập nhật OU bằng OrganizationUnitManager để tự sinh lại mã hệ thống nếu cần
        await _organizationUnitManager.UpdateAsync(organizationUnit);

        // 6. Map sang DTO trả về
        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
    }

    // --- DeleteAsync ---
    [Authorize(CoreFWPermissions.OrganizationUnits.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _organizationUnitManager.DeleteAsync(id);
    }

    // --- MoveAsync ---
    [Authorize(CoreFWPermissions.OrganizationUnits.Move)]
    public async Task MoveAsync(MoveOrganizationUnitInput input)
    {
        await _organizationUnitManager.MoveAsync(input.Id, input.NewParentId);
    }

    // --- GetLookupAsync ---
    [AllowAnonymous]
    public async Task<ListResultDto<OrganizationUnitLookupDto>> GetLookupAsync(GetOrganizationUnitsInput input)
    {
        var queryable = await _repository.GetQueryableAsync();
        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                     ou => ou.DisplayName.Contains(input.Filter!)); // Chỉ filter theo DisplayName

        var totalCount = await _repository.LongCountAsync(
            ou => ou.DisplayName.Contains(input.Filter!)
        );

        // Sắp xếp theo DisplayName mặc định
        queryable = queryable.OrderBy(input.Sorting ?? nameof(OrganizationUnit.DisplayName)).PageBy(input);

        var organizationUnits = await AsyncExecuter.ToListAsync(queryable);
        // Map sang Lookup DTO (AutoMapper xử lý DisplayName)
        var lookupDtos = ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitLookupDto>>(organizationUnits);
        return new PagedResultDto<OrganizationUnitLookupDto>(totalCount, lookupDtos);
    }
}