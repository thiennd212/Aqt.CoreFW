using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Ranks;
using Aqt.CoreFW.Application.Contracts.Ranks.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Namespace mới cho RankLookupDto
using Aqt.CoreFW.Domain.Ranks;
using Aqt.CoreFW.Domain.Ranks.Entities;
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Permissions;
using Aqt.CoreFW.Ranks;
using Aqt.CoreFW.Shared.Services; // Namespace cho IAbpExcelExportHelper
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Aqt.CoreFW.Application.Ranks
{
    [Authorize(CoreFWPermissions.Ranks.Default)]
    public class RankAppService :
        CrudAppService<
            Rank,
            RankDto,
            Guid,
            GetRanksInput,
            CreateUpdateRankDto>,
        IRankAppService
    {
        private readonly IRankRepository _rankRepository;
        private readonly RankManager _rankManager;
        private readonly IStringLocalizer<CoreFWResource> _localizer;
        private readonly IAbpExcelExportHelper _excelExportHelper;

        public RankAppService(
            IRepository<Rank, Guid> repository,
            IRankRepository rankRepository,
            RankManager rankManager,
            IStringLocalizer<CoreFWResource> localizer,
            IAbpExcelExportHelper excelExportHelper)
            : base(repository)
        {
            _rankRepository = rankRepository;
            _rankManager = rankManager;
            _localizer = localizer;
            _excelExportHelper = excelExportHelper;

            GetPolicyName = CoreFWPermissions.Ranks.Default;
            GetListPolicyName = CoreFWPermissions.Ranks.Default;
            CreatePolicyName = CoreFWPermissions.Ranks.Create;
            UpdatePolicyName = CoreFWPermissions.Ranks.Update;
            DeletePolicyName = CoreFWPermissions.Ranks.Delete;
        }

        [Authorize(CoreFWPermissions.Ranks.Create)]
        public override async Task<RankDto> CreateAsync(CreateUpdateRankDto input)
        {
            var entity = await _rankManager.CreateAsync(
                input.Code,
                input.Name,
                input.Order,
                input.Status, // Sửa lại thứ tự tham số theo RankManager.CreateAsync
                input.Description
            );

            await Repository.InsertAsync(entity, autoSave: true);
            return ObjectMapper.Map<Rank, RankDto>(entity);
        }

        [Authorize(CoreFWPermissions.Ranks.Update)]
        public override async Task<RankDto> UpdateAsync(Guid id, CreateUpdateRankDto input)
        {
            var entity = await GetEntityByIdAsync(id);

            if (!string.Equals(entity.Code, input.Code, StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException(_localizer["RankCodeCannotBeChanged"]); // Sử dụng key localization đã thông báo
            }

            // Kế hoạch gốc gọi RankManager.UpdateAsync, nhưng theo code trong kế hoạch,
            // nó chỉ gọi các phương thức public của entity. Ta có thể làm trực tiếp ở đây
            // hoặc giữ nguyên gọi Manager nếu muốn tập trung logic. Giữ theo kế hoạch:
            entity = await _rankManager.UpdateAsync(
                entity,
                input.Name,
                input.Order,
                input.Description,
                input.Status,
                entity.LastSyncDate,
                entity.SyncRecordId,
                entity.SyncRecordCode
            );

            await Repository.UpdateAsync(entity, autoSave: true);
            return ObjectMapper.Map<Rank, RankDto>(entity);
        }

        [Authorize(CoreFWPermissions.Ranks.Delete)]
        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);
        }

        [AllowAnonymous]
        public async Task<ListResultDto<RankLookupDto>> GetLookupAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            var query = queryable
                .Where(r => r.Status == RankStatus.Active)
                .OrderBy(r => r.Order).ThenBy(r => r.Name);

            var ranks = await AsyncExecuter.ToListAsync(query);
            var lookupDtos = ObjectMapper.Map<List<Rank>, List<RankLookupDto>>(ranks);
            return new ListResultDto<RankLookupDto>(lookupDtos);
        }

        public override async Task<PagedResultDto<RankDto>> GetListAsync(GetRanksInput input)
        {
            var totalCount = await _rankRepository.GetCountAsync(
                filterText: input.Filter,
                status: input.Status
            );

            var ranks = await _rankRepository.GetListAsync(
                filterText: input.Filter,
                status: input.Status,
                sorting: input.Sorting ?? nameof(Rank.Name),
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount
            );

            var rankDtos = ObjectMapper.Map<List<Rank>, List<RankDto>>(ranks);
            return new PagedResultDto<RankDto>(totalCount, rankDtos);
        }

        [Authorize(CoreFWPermissions.Ranks.Export)]
        public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetRanksInput input)
        {
            var ranks = await _rankRepository.GetListAsync(
                filterText: input.Filter,
                status: input.Status,
                sorting: input.Sorting ?? nameof(Rank.Name),
                maxResultCount: int.MaxValue, // Get all records
                skipCount: 0
            );

            if (!ranks.Any())
            {
                throw new UserFriendlyException(_localizer["NoDataFoundToExport"]); // Sử dụng key localization đã thông báo
            }

            var excelDtos = ObjectMapper.Map<List<Rank>, List<RankExcelDto>>(ranks);

            return await _excelExportHelper.ExportToExcelAsync(
                items: excelDtos,
                filePrefix: "Ranks",
                sheetName: "RanksData"
             );
        }
    }
}