using Aqt.CoreFW.Domain.WorkflowStatuses.Entities;
using Aqt.CoreFW.Domain.Shared;
using Aqt.CoreFW.Domain.Shared.WorkflowStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.WorkflowStatuses;

/// <summary>
/// Configures the database mapping for the <see cref="WorkflowStatus"/> entity.
/// </summary>
public class WorkflowStatusConfiguration : IEntityTypeConfiguration<WorkflowStatus>
{
    public void Configure(EntityTypeBuilder<WorkflowStatus> builder)
    {
        builder.ToTable(CoreFWConsts.DbTablePrefix + "WorkflowStatuses", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Apply standard ABP conventions

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(WorkflowStatusConsts.MaxCodeLength)
            .HasColumnName(nameof(WorkflowStatus.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(WorkflowStatusConsts.MaxNameLength)
            .HasColumnName(nameof(WorkflowStatus.Name));

        builder.Property(x => x.Description)
            .HasMaxLength(WorkflowStatusConsts.MaxDescriptionLength)
            .HasColumnName(nameof(WorkflowStatus.Description));

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName(nameof(WorkflowStatus.Order));

        builder.Property(x => x.ColorCode)
            .HasMaxLength(WorkflowStatusConsts.MaxColorCodeLength)
            .HasColumnName(nameof(WorkflowStatus.ColorCode));

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnName(nameof(WorkflowStatus.IsActive));

        // --- Indexes ---
        builder.HasIndex(x => x.Code)
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}WorkflowStatuses_Code"); // Unique index on Code

        builder.HasIndex(x => x.Name)
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}WorkflowStatuses_Name"); // Unique index on Name

        builder.HasIndex(x => new { x.Order, x.Name }) // Optional composite index for sorting
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}WorkflowStatuses_Order_Name");

        builder.HasIndex(x => x.IsActive) // Optional index for filtering
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}WorkflowStatuses_IsActive");
    }
}