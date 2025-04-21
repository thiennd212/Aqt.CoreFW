using Aqt.CoreFW.Domain.Communes.Entities;
using Aqt.CoreFW.Domain.Districts.Entities; // Needed for FK constraint
using Aqt.CoreFW.Domain.Provinces.Entities; // Needed for FK constraint
using Aqt.CoreFW.Domain.Shared.Communes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Communes;

public class CommuneConfiguration : IEntityTypeConfiguration<Commune>
{
    public void Configure(EntityTypeBuilder<Commune> builder)
    {
        // Recommended table name format
        builder.ToTable(CoreFWConsts.DbTablePrefix + "Communes", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Apply ABP base configurations

        builder.HasKey(x => x.Id);

        // Property Configurations
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(CommuneConsts.MaxCodeLength)
            .HasColumnName(nameof(Commune.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(CommuneConsts.MaxNameLength)
            .HasColumnName(nameof(Commune.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(Commune.Status))
            .HasConversion<byte>(); // Store enum as byte

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0) // Set default value if desired at DB level
            .HasColumnName(nameof(Commune.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(CommuneConsts.MaxDescriptionLength)
            .HasColumnName(nameof(Commune.Description));

        builder.Property(x => x.ProvinceId)
            .IsRequired()
            .HasColumnName(nameof(Commune.ProvinceId));

        builder.Property(x => x.DistrictId)
            .HasColumnName(nameof(Commune.DistrictId)); // Nullable FK

        builder.Property(x => x.LastSyncedTime)
            .HasColumnName(nameof(Commune.LastSyncedTime));

        builder.Property(x => x.SyncId)
            .HasMaxLength(CommuneConsts.MaxSyncIdLength)
            .HasColumnName(nameof(Commune.SyncId));

        builder.Property(x => x.SyncCode)
            .HasMaxLength(CommuneConsts.MaxSyncCodeLength)
            .HasColumnName(nameof(Commune.SyncCode));

        // Foreign Keys
        builder.HasOne<Province>() // Define relationship with Province
               .WithMany()        // Assuming Province does not have a Commune collection navigation property
               .HasForeignKey(x => x.ProvinceId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Prevent deleting Province if Communes exist

        builder.HasOne<District>() // Define relationship with District
               .WithMany()        // Assuming District does not have a Commune collection navigation property
               .HasForeignKey(x => x.DistrictId)
               .IsRequired(false) // FK is optional (nullable)
               .OnDelete(DeleteBehavior.Restrict); // Prevent deleting District if Communes exist

        // Indexes
        builder.HasIndex(x => x.Code)
               .IsUnique() // Enforce unique code at database level
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Communes_Code");

        builder.HasIndex(x => x.Name) // Non-unique index on Name for searching
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Communes_Name");

        builder.HasIndex(x => x.ProvinceId) // Index on required FK
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Communes_ProvinceId");

        builder.HasIndex(x => x.DistrictId) // Index on optional FK
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Communes_DistrictId");

        builder.HasIndex(x => x.Status) // Index for filtering by status
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Communes_Status");

        // Composite index for common filtering/sorting scenarios
        builder.HasIndex(x => new { x.ProvinceId, x.DistrictId, x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Communes_ProvinceId_DistrictId_Order_Name");
    }
}