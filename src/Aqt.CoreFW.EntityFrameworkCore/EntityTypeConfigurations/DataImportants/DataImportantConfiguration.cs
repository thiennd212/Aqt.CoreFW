using Aqt.CoreFW.Domain.DataImportants.Entities; // DataImportant Entity
using Aqt.CoreFW.Domain.DataGroups.Entities; // DataGroup Entity for FK relation
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts
using Aqt.CoreFW.DataImportants; // Namespace chứa DataImportantConsts và DataImportantStatus enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.DataImportants; // Namespace Configuration

/// <summary>
/// Configures the database mapping for the <see cref="DataImportant"/> entity.
/// </summary>
public class DataImportantConfiguration : IEntityTypeConfiguration<DataImportant>
{
    public void Configure(EntityTypeBuilder<DataImportant> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts
        builder.ToTable(CoreFWConsts.DbTablePrefix + "DataImportants", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng các quy ước chuẩn của ABP

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(DataImportantConsts.MaxCodeLength)
            .HasColumnName(nameof(DataImportant.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(DataImportantConsts.MaxNameLength)
            .HasColumnName(nameof(DataImportant.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(DataImportant.Status))
            .HasConversion<byte>(); // Map enum sang byte

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName(nameof(DataImportant.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(DataImportantConsts.MaxDescriptionLength)
            .HasColumnName(nameof(DataImportant.Description));

        // DataGroupId là Guid bắt buộc
        builder.Property(x => x.DataGroupId)
            .IsRequired()
            .HasColumnName(nameof(DataImportant.DataGroupId));

        // --- Foreign Keys ---
        builder.HasOne<DataGroup>() // Không cần chỉ định navigation property 'DataGroup' nếu không có trong Entity
               .WithMany() // Một DataGroup có thể có nhiều DataImportant
               .HasForeignKey(x => x.DataGroupId)
               .IsRequired() // Đảm bảo FK là bắt buộc
               .OnDelete(DeleteBehavior.Restrict); // Quan trọng: Ngăn chặn việc xóa DataGroup nếu còn DataImportant tham chiếu

        // --- Indexes ---

        // Index cho Code (Unique theo DataGroupId)
        builder.HasIndex(x => new { x.DataGroupId, x.Code })
               .IsUnique() // Đảm bảo Code là duy nhất trong phạm vi DataGroupId
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataImportants_DataGroupId_Code");

        builder.HasIndex(x => x.Name)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataImportants_Name");

        builder.HasIndex(x => x.Status)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataImportants_Status");

        // Index trên DataGroupId (đã có trong index kết hợp bên trên, nhưng thêm riêng cũng không sao)
        builder.HasIndex(x => x.DataGroupId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataImportants_DataGroupId");

        // Index kết hợp cho sắp xếp/lọc phổ biến trong cùng nhóm
        builder.HasIndex(x => new { x.DataGroupId, x.Status, x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}DataImportants_DataGroupId_Status_Order_Name");
    }
} 