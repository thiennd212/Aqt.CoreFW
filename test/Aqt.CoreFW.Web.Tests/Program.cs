using Microsoft.AspNetCore.Builder;
using Aqt.CoreFW;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Aqt.CoreFW.Web.csproj"); 
await builder.RunAbpModuleAsync<CoreFWWebTestModule>(applicationName: "Aqt.CoreFW.Web");

public partial class Program
{
}
