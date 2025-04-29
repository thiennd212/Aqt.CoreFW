using Microsoft.AspNetCore.StaticFiles;

namespace Aqt.CoreFW.Helpers
{
    /// <summary>
    /// Helper class for file operations
    /// </summary>
    public static class AbpFileHelper
    {
        private static readonly FileExtensionContentTypeProvider _provider = new();

        /// <summary>
        /// GetMimeType
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMimeType(string fileName)
        {
            if (!_provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
