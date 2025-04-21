using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aqt.CoreFW.Shared.Services;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Timing;

namespace Aqt.CoreFW.Application.Shared.Excel;

/// <summary>
/// Helper class to export Excel file from any DTO list.
/// </summary>
public class AbpExcelExportHelper : IAbpExcelExportHelper
{
    private readonly IClock _clock;

    public AbpExcelExportHelper(IClock clock)
    {
        _clock = clock;
    }

    /// <summary>
    /// Exports a list to Excel and returns RemoteStreamContent for ABP download.
    /// </summary>
    /// <typeparam name="TDto">The DTO type</typeparam>
    /// <param name="items">The list of DTOs</param>
    /// <param name="filePrefix">Prefix for the generated file name</param>
    /// <param name="sheetName">Optional sheet name</param>
    /// <returns>IRemoteStreamContent</returns>
    public async Task<IRemoteStreamContent> ExportToExcelAsync<TDto>(IEnumerable<TDto> items, string filePrefix = "Export", string sheetName = "Sheet1")
    {
        var stream = new MemoryStream();

        await stream.SaveAsAsync(items, sheetName: sheetName);

        stream.Seek(0, SeekOrigin.Begin);

        var fileName = $"{filePrefix}_{_clock.Now:yyyyMMdd_HHmmss}.xlsx";

        return new RemoteStreamContent(
            stream,
            fileName,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );
    }
}
