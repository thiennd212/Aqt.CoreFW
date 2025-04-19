using Aqt.CoreFW.Application.Contracts.Provinces.Dtos;
using Aqt.CoreFW.Domain.Provinces.Entities; // Namespace Entity, kiểm tra lại nếu cần
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Domain.Shared.Provinces; // Namespace Enum
using AutoMapper;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Application.Provinces;

/// <summary>
/// AutoMapper mapping action to handle specific logic when mapping
/// from Province entity to ProvinceExcelDto.
/// Specifically handles localization of the Status enum.
/// </summary>
public class ProvinceToExcelMappingAction
    : IMappingAction<Province, ProvinceExcelDto>, ITransientDependency
{
    private readonly IStringLocalizer<CoreFWResource> _localizer;

    public ProvinceToExcelMappingAction(IStringLocalizer<CoreFWResource> localizer)
    {
        _localizer = localizer;
    }

    public void Process(Province source, ProvinceExcelDto destination, ResolutionContext context)
    {
        // Localize the Status enum based on its value
        // Giả định key localization là "Enum:ProvinceStatus:0" hoặc "Enum:ProvinceStatus:1"
        // Hoặc bạn có thể dùng source.Status.ToString() nếu key là "Active", "Inactive"
        destination.StatusText = _localizer[$"Enum:{typeof(ProvinceStatus)}:{(int)source.Status}"];

        // CountryName sẽ được xử lý trong AppService trước khi mapping
    }
}