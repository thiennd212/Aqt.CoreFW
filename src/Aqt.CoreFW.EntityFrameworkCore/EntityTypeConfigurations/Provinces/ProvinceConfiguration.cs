using Aqt.CoreFW.Domain.Provinces.Entities;
using Aqt.CoreFW.Domain.Countries.Entities; // Namespace Country Entity - Kiểm tra lại nếu cần
using Aqt.CoreFW.Domain.Shared.Provinces; // Namespace CoreFWConsts
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Provinces;

/// <summary>
/// Configures the database mapping for the <see cref="Province"/> entity.
/// </summary>
public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts (nếu có)
        builder.ToTable(CoreFWConsts.DbTablePrefix + "Provinces", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng các quy ước chuẩn của ABP

        builder.HasKey(x => x.Id);

        // --- Cấu hình thuộc tính ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(ProvinceConsts.MaxCodeLength)
            .HasColumnName(nameof(Province.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ProvinceConsts.MaxNameLength)
            .HasColumnName(nameof(Province.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(Province.Status))
            .HasConversion<byte>(); // Chuyển Enum sang byte trong DB

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName(nameof(Province.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(ProvinceConsts.MaxDescriptionLength)
            .HasColumnName(nameof(Province.Description));

        builder.Property(x => x.CountryId)
            .IsRequired()
            .HasColumnName(nameof(Province.CountryId));

        builder.Property(x => x.LastSyncedTime)
            .HasColumnName(nameof(Province.LastSyncedTime));

        builder.Property(x => x.SyncId)
            .HasMaxLength(ProvinceConsts.MaxSyncIdLength)
            .HasColumnName(nameof(Province.SyncId));

        builder.Property(x => x.SyncCode)
            .HasMaxLength(ProvinceConsts.MaxSyncCodeLength)
            .HasColumnName(nameof(Province.SyncCode));

        // --- Khóa ngoại ---
        builder.HasOne<Country>() // Không định nghĩa navigation property ngược lại từ Country
               .WithMany() // Giả định Country không có collection Provinces
               .HasForeignKey(x => x.CountryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Country nếu còn Province

        // --- Chỉ mục (Indexes) ---
        builder.HasIndex(x => x.Code)
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Provinces_Code"); // Index duy nhất cho Code

        // Optional: Index cho Name trong Country nếu cần check unique hoặc tối ưu query
        builder.HasIndex(x => new { x.CountryId, x.Name })
                .IsUnique() // Nếu Name cần duy nhất trong Country
                .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Provinces_CountryId_Name");

        builder.HasIndex(x => x.CountryId) // Index trên khóa ngoại thường tốt cho performance
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Provinces_CountryId");

        builder.HasIndex(x => x.Status) // Index cho việc lọc theo Status
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Provinces_Status");

        // Index tổng hợp cho các trường hợp sắp xếp/lọc phổ biến
        builder.HasIndex(x => new { x.CountryId, x.Order, x.Name })
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Provinces_CountryId_Order_Name");
    }
}