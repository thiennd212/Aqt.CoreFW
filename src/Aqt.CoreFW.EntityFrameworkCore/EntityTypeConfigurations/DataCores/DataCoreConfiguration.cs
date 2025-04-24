using Aqt.CoreFW.Domain.DataCores.Entities;
using Aqt.CoreFW.Domain.DataGroups.Entities;
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts (Kiểm tra lại nếu khác)
using Aqt.CoreFW.DataCores; // Namespace chứa DataCoreConsts và DataCoreStatus enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.DataCores;

/// <summary>
/// Configures the database mapping for the <see cref="DataCore"/> entity.
/// </summary>
public class DataCoreConfiguration : IEntityTypeConfiguration<DataCore>
{
    public void Configure(EntityTypeBuilder<DataCore> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts
        builder.ToTable(CoreFWConsts.DbTablePrefix + "DataCores", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng các quy ước chuẩn của ABP

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(DataCoreConsts.MaxCodeLength)
            .HasColumnName(nameof(DataCore.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(DataCoreConsts.MaxNameLength)
            .HasColumnName(nameof(DataCore.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(DataCore.Status))
            .HasConversion<byte>(); // Map enum sang byte

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName(nameof(DataCore.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(DataCoreConsts.MaxDescriptionLength)
            .HasColumnName(nameof(DataCore.Description));

        // DataGroupId là Guid bắt buộc
        builder.Property(x => x.DataGroupId)
            .IsRequired()
            .HasColumnName(nameof(DataCore.DataGroupId));

        // --- Foreign Keys ---
        builder.HasOne<DataGroup>() // Không cần chỉ định navigation property 'DataGroup' nếu không có trong Entity
               .WithMany() // Một DataGroup có thể có nhiều DataCore
               .HasForeignKey(x => x.DataGroupId)
               .IsRequired() // Đảm bảo FK là bắt buộc
               .OnDelete(DeleteBehavior.Restrict); // Quan trọng: Ngăn chặn việc xóa DataGroup nếu còn DataCore tham chiếu

        // --- Indexes ---

        // Index cho Code (unique toàn cục)
        builder.HasIndex(x => x.Code)
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataCores_Code");

        builder.HasIndex(x => x.Name)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataCores_Name");

        builder.HasIndex(x => x.Status)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataCores_Status");

        // Index trên DataGroupId để tăng tốc lọc theo nhóm
        builder.HasIndex(x => x.DataGroupId)
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataCores_DataGroupId");

        // Index kết hợp cho sắp xếp/lọc phổ biến trong cùng nhóm
        builder.HasIndex(x => new { x.DataGroupId, x.Status, x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataCores_DataGroupId_Status_Order_Name");
    }
} 