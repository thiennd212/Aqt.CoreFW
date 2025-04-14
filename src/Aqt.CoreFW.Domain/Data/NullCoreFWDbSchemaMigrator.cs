using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Data;

/* This is used if database provider does't define
 * ICoreFWDbSchemaMigrator implementation.
 */
public class NullCoreFWDbSchemaMigrator : ICoreFWDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
