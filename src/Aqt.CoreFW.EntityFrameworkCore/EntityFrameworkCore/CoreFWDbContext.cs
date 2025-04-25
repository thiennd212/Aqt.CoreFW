using Aqt.CoreFW.Domain.AccountTypes.Entities;
using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Domain.Countries.Entities;
using Aqt.CoreFW.Domain.DataCores.Entities;
using Aqt.CoreFW.Domain.DataGroups.Entities;
using Aqt.CoreFW.Domain.Districts.Entities;
using Aqt.CoreFW.Domain.JobTitles.Entities;
using Aqt.CoreFW.Domain.Provinces.Entities;
using Aqt.CoreFW.Domain.Ranks.Entities;
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities;
using Aqt.CoreFW.Domain.DataImportants.Entities;
using EasyAbp.FileManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Aqt.CoreFW.Domain.Procedures.Entities;

namespace Aqt.CoreFW.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class CoreFWDbContext :
    AbpDbContext<CoreFWDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Country> Countries { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Commune> Communes { get; set; }
    public DbSet<Rank> Ranks { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    // Chú thích: Khi có module Employee, bạn sẽ thêm DbSet cho nó ở đây nếu cần truy vấn trực tiếp
    // public DbSet<Employee> Employees { get; set; }
    public DbSet<WorkflowStatus> WorkflowStatuses { get; set; }
    public DbSet<DataGroup> DataGroups { get; set; } // Thêm DbSet cho DataGroup
    public DbSet<DataCore> DataCores { get; set; } // Thêm DbSet cho DataCore
    public DbSet<DataImportant> DataImportants { get; set; } // << Thêm DbSet
    public DbSet<Procedure> Procedures { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public CoreFWDbContext(DbContextOptions<CoreFWDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        //EasyAbp.FileManagement
        builder.ConfigureFileManagement();
        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(CoreFWConsts.DbTablePrefix + "YourEntities", CoreFWConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        // Apply all IEntityTypeConfiguration implementations from this assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
