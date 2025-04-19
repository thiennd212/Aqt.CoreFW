using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Provinces.Dtos;
using Aqt.CoreFW.Application.Contracts.Shared.Lookups; // Cập nhật using cho CountryLookupDto
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Aqt.CoreFW.Application.Contracts.Provinces;

public interface IProvinceAppService :
    ICrudAppService<
        ProvinceDto,
        Guid,
        GetProvincesInput,
        CreateUpdateProvinceDto>
{
    /// <summary>
    /// Gets a list of active provinces suitable for dropdown lookups, optionally filtered by country.
    /// </summary>
    Task<ListResultDto<ProvinceLookupDto>> GetLookupAsync(Guid? countryId = null);

    /// <summary>
    /// Gets a list of countries suitable for dropdown lookups (used for filtering provinces).
    /// </summary>
    Task<ListResultDto<CountryLookupDto>> GetCountryLookupAsync();

    /// <summary>
    /// Exports the list of provinces to an Excel file based on the provided filters.
    /// </summary>
    Task<IRemoteStreamContent> GetListAsExcelAsync(GetProvincesInput input);
}