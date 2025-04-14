using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Aqt.CoreFW.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class CoreFWDbContextFactory : IDesignTimeDbContextFactory<CoreFWDbContext>
{
    public CoreFWDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        CoreFWEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<CoreFWDbContext>()
            .UseOracle(configuration.GetConnectionString("Default"));
        
        return new CoreFWDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Aqt.CoreFW.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
