using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Aqt.CoreFW.Application.Contracts.OrganizationUnits;

public interface IOrganizationUnitAppService : IApplicationService
{
    Task<OrganizationUnitDto> GetAsync(Guid id);

    Task<ListResultDto<OrganizationUnitTreeNodeDto>> GetTreeAsync();

    Task<OrganizationUnitDto> CreateAsync(CreateOrganizationUnitDto input);

    Task<OrganizationUnitDto> UpdateAsync(Guid id, UpdateOrganizationUnitDto input);

    Task DeleteAsync(Guid id);

    Task MoveAsync(MoveOrganizationUnitInput input);

    Task<ListResultDto<OrganizationUnitLookupDto>> GetLookupAsync(GetOrganizationUnitsInput input);
} 