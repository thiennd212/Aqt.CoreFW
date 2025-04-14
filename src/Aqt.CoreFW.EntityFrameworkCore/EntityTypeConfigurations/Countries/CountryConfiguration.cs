using Aqt.CoreFW.Domain.Countries.Entities;
using Aqt.CoreFW.Domain.Shared;          // For CoreFWConsts
using Aqt.CoreFW.Domain.Shared.Countries; // For CountryConsts
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Countries;

/// <summary>
/// Configures the database mapping for the <see cref="Country"/> entity.
/// </summary>
public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        // Configure table name and schema (if applicable)
        // Check if DbSchema is null or whitespace before using it.
        builder.ToTable(CoreFWConsts.DbTablePrefix + "Countries", CoreFWConsts.DbSchema);

        // Apply ABP conventions (like soft delete, auditing properties)
        builder.ConfigureByConvention();

        // Primary Key
        builder.HasKey(x => x.Id);

        // Configure Code property
        builder.Property(x => x.Code)
            .IsRequired() // Database constraint
            .HasMaxLength(CountryConsts.MaxCodeLength) // Database constraint
            .HasColumnName(nameof(Country.Code)); // Optional: Explicit column name

        // Configure Name property
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(CountryConsts.MaxNameLength)
            .HasColumnName(nameof(Country.Name));

        // Index for unique Code constraint
        builder.HasIndex(x => x.Code).IsUnique();

        // Optional: Index for Name property to potentially speed up lookups
        builder.HasIndex(x => x.Name);

        // Add other configurations if needed...
    }
} 