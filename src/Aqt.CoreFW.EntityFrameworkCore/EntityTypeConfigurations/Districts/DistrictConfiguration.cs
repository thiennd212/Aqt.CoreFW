using Aqt.CoreFW.Domain.Districts.Entities;
using Aqt.CoreFW.Domain.Provinces.Entities;
using Aqt.CoreFW.Domain.Shared; // Using cho CoreFWConsts
using Aqt.CoreFW.Domain.Shared.Districts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Districts;

/// <summary>
/// Configures the database mapping for the <see cref="District"/> entity.
/// </summary>
public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts
        builder.ToTable(CoreFWConsts.DbTablePrefix + "Districts", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng các convention chuẩn của ABP

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(DistrictConsts.MaxCodeLength)
            .HasColumnName(nameof(District.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(DistrictConsts.MaxNameLength)
            .HasColumnName(nameof(District.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(District.Status))
            .HasConversion<byte>(); // Map enum sang byte

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0) // Thêm giá trị mặc định nếu cần
            .HasColumnName(nameof(District.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(DistrictConsts.MaxDescriptionLength)
            .HasColumnName(nameof(District.Description));

        builder.Property(x => x.ProvinceId)
            .IsRequired()
            .HasColumnName(nameof(District.ProvinceId));

        builder.Property(x => x.LastSyncedTime)
            .HasColumnName(nameof(District.LastSyncedTime));

        builder.Property(x => x.SyncId)
            .HasMaxLength(DistrictConsts.MaxSyncIdLength)
            .HasColumnName(nameof(District.SyncId));

        builder.Property(x => x.SyncCode)
            .HasMaxLength(DistrictConsts.MaxSyncCodeLength)
            .HasColumnName(nameof(District.SyncCode));

        // --- Foreign Key ---
        // Liên kết tới Province, không có navigation property ngược lại từ Province
        builder.HasOne<Province>()
               .WithMany() // Giả định Province không có collection Districts
               .HasForeignKey(x => x.ProvinceId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Province nếu còn District

        // --- Indexes ---
        // Index cho Code (giả định là unique toàn cục)
        builder.HasIndex(x => x.Code)
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Districts_Code");

        // Index cho Name trong Province (cân nhắc unique constraint nếu cần)
        builder.HasIndex(x => new { x.ProvinceId, x.Name })
               // .IsUnique() // Bỏ comment nếu Name phải là duy nhất trong Province
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Districts_ProvinceId_Name");

        // Index cho Foreign Key ProvinceId
        builder.HasIndex(x => x.ProvinceId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Districts_ProvinceId");

        // Index cho Status (tùy chọn, hữu ích khi lọc nhiều theo Status)
        builder.HasIndex(x => x.Status)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Districts_Status");

        // Index tổng hợp cho việc sắp xếp/lọc phổ biến
        builder.HasIndex(x => new { x.ProvinceId, x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Districts_ProvinceId_Order_Name");
    }
}