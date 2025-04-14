using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Aqt.CoreFW.Data;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.EntityFrameworkCore;

public class EntityFrameworkCoreCoreFWDbSchemaMigrator
    : ICoreFWDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreCoreFWDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the CoreFWDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<CoreFWDbContext>()
            .Database
            .MigrateAsync();
    }
}
