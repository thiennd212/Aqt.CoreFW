using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Aqt.CoreFW.Shared.Services
{
    public interface IAbpExcelExportHelper
    {
        /// <summary>
        /// Exports a list to Excel and returns RemoteStreamContent for ABP download.
        /// </summary>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <param name="items">The list of DTOs</param>
        /// <param name="filePrefix">Prefix for the generated file name</param>
        /// <param name="sheetName">Optional sheet name</param>
        /// <returns>IRemoteStreamContent</returns>
        Task<IRemoteStreamContent> ExportToExcelAsync<TDto>(IEnumerable<TDto> items, string filePrefix = "Export", string sheetName = "Sheet1");
    }
}
