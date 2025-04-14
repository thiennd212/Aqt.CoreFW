using Xunit;

namespace Aqt.CoreFW.EntityFrameworkCore;

[CollectionDefinition(CoreFWTestConsts.CollectionDefinitionName)]
public class CoreFWEntityFrameworkCoreCollection : ICollectionFixture<CoreFWEntityFrameworkCoreFixture>
{

}
