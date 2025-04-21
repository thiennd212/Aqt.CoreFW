using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Communes.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Using for Province/District Lookups
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent

namespace Aqt.CoreFW.Application.Contracts.Communes;

public interface ICommuneAppService :
    ICrudAppService< // Inherit from ICrudAppService for basic CRUD
        CommuneDto,               // DTO for displaying communes
        Guid,                     // Primary key type
        GetCommunesInput,         // DTO for filtering/paging list
        CreateUpdateCommuneDto>   // DTO for creating/updating
{
    Task<ListResultDto<CommuneLookupDto>> GetLookupAsync(Guid? provinceId = null, Guid? districtId = null);

    Task<ListResultDto<ProvinceLookupDto>> GetProvinceLookupAsync();

    Task<ListResultDto<DistrictLookupDto>> GetDistrictLookupAsync(Guid? provinceId = null);

    Task<IRemoteStreamContent> GetListAsExcelAsync(GetCommunesInput input);
}