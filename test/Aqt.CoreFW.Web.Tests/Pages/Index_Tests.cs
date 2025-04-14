using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Aqt.CoreFW.Pages;

[Collection(CoreFWTestConsts.CollectionDefinitionName)]
public class Index_Tests : CoreFWWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
