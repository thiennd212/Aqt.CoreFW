    using Aqt.CoreFW.Domain.Components.Entities; // ProcedureComponentLink Entity
    using Aqt.CoreFW.Domain.Shared; // CoreFWConsts
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Volo.Abp.EntityFrameworkCore.Modeling; // For ConfigureByConvention

    namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Components;

    /// <summary>
    /// Configures the database mapping for the <see cref="ProcedureComponentLink"/> joining entity.
    /// </summary>
    public class ProcedureComponentLinkConfiguration : IEntityTypeConfiguration<ProcedureComponentLink>
    {
        public void Configure(EntityTypeBuilder<ProcedureComponentLink> builder)
        {
            // Table Name and Schema
            builder.ToTable(CoreFWConsts.DbTablePrefix + "ProcedureComponentLinks", CoreFWConsts.DbSchema);

            builder.ConfigureByConvention(); // Apply standard conventions if any

            // Define Composite Primary Key
            builder.HasKey(l => new { l.ProcedureId, l.ProcedureComponentId });

            // Foreign Key to Procedure (Configure relationship from Procedure side if needed)
            // EF Core can often infer this, but explicit configuration is clearer.
            // Quan hệ này thường sẽ được định nghĩa trong ProcedureConfiguration nếu Procedure có collection Links
            // builder.HasOne<Procedure>() // Reference Procedure entity (need using statement)
            //        .WithMany() // Procedure may have many links, but no direct navigation prop defined here
            //        .HasForeignKey(l => l.ProcedureId)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade); // Define delete behavior if Procedure is deleted

            // Foreign Key to ProcedureComponent (Already configured in ProcedureComponentConfiguration via HasMany)
            // Không cần định nghĩa lại ở đây vì đã có trong ProcedureComponentConfiguration.

            // Indexes on Foreign Keys for performance
            builder.HasIndex(l => l.ProcedureId)
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}ProcedureComponentLinks_ProcedureId");

            builder.HasIndex(l => l.ProcedureComponentId)
                  .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}ProcedureComponentLinks_ProcedureComponentId");
        }
    }