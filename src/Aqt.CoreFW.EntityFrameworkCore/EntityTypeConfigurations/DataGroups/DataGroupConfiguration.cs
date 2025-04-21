using Aqt.CoreFW.Domain.DataGroups.Entities; // DataGroup Entity
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts
using Aqt.CoreFW.DataGroups; // Namespace chứa DataGroupConsts và DataGroupStatus enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.DataGroups; // Namespace Configuration

/// <summary>
/// Configures the database mapping for the <see cref="DataGroup"/> entity.
/// </summary>
public class DataGroupConfiguration : IEntityTypeConfiguration<DataGroup>
{
    public void Configure(EntityTypeBuilder<DataGroup> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts
        builder.ToTable(CoreFWConsts.DbTablePrefix + "DataGroups", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng các quy ước chuẩn của ABP

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(DataGroupConsts.MaxCodeLength)
            .HasColumnName(nameof(DataGroup.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(DataGroupConsts.MaxNameLength)
            .HasColumnName(nameof(DataGroup.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(DataGroup.Status))
            .HasConversion<byte>(); // Map enum sang byte

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName(nameof(DataGroup.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(DataGroupConsts.MaxDescriptionLength)
            .HasColumnName(nameof(DataGroup.Description));

        // ParentId là nullable Guid
        builder.Property(x => x.ParentId)
            .HasColumnName(nameof(DataGroup.ParentId));

        builder.Property(x => x.LastSyncDate)
            .HasColumnName(nameof(DataGroup.LastSyncDate));

        builder.Property(x => x.SyncRecordId)
            .HasColumnName(nameof(DataGroup.SyncRecordId));

        builder.Property(x => x.SyncRecordCode)
            .HasMaxLength(DataGroupConsts.MaxSyncRecordCodeLength)
            .HasColumnName(nameof(DataGroup.SyncRecordCode));

        // --- Foreign Keys (Self-Referencing) ---
        builder.HasOne<DataGroup>() // Không cần chỉ định navigation property 'Parent' nếu không có trong Entity
               .WithMany() // Một Parent có thể có nhiều Children (không cần collection 'Children')
               .HasForeignKey(x => x.ParentId)
               .OnDelete(DeleteBehavior.Restrict); // Quan trọng: Ngăn chặn việc xóa cha nếu còn con (Manager cũng đã kiểm tra)

        // --- Indexes ---
        builder.HasIndex(x => x.Code)
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataGroups_Code");

        builder.HasIndex(x => x.Name)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataGroups_Name");

        builder.HasIndex(x => x.Status)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataGroups_Status");

        // Index trên ParentId để tăng tốc truy vấn con hoặc nhóm gốc
        builder.HasIndex(x => x.ParentId)
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataGroups_ParentId");

        // Index kết hợp cho sắp xếp/lọc phổ biến trong cùng cấp
        builder.HasIndex(x => new { x.ParentId, x.Status, x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataGroups_ParentId_Status_Order_Name");
    }
} 