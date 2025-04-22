using Aqt.CoreFW.Domain.AccountTypes.Entities; // AccountType Entity
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts (Đảm bảo namespace này đúng)
using Aqt.CoreFW.AccountTypes; // Namespace chứa AccountTypeConsts và AccountTypeStatus enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.AccountTypes; // Namespace Configuration

/// <summary>
/// Configures the database mapping for the <see cref="AccountType"/> entity.
/// </summary>
public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts (Đảm bảo CoreFWConsts tồn tại và có các hằng số này)
        builder.ToTable(CoreFWConsts.DbTablePrefix + "AccountTypes", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng các quy ước chuẩn của ABP

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(AccountTypeConsts.MaxCodeLength)
            .HasColumnName(nameof(AccountType.Code)); // Đảm bảo tên cột khớp tên thuộc tính

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(AccountTypeConsts.MaxNameLength)
            .HasColumnName(nameof(AccountType.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(AccountType.Status))
            .HasConversion<byte>(); // Map enum AccountTypeStatus sang kiểu byte trong CSDL

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName(nameof(AccountType.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(AccountTypeConsts.MaxDescriptionLength)
            .HasColumnName(nameof(AccountType.Description));

        builder.Property(x => x.LastSyncDate)
            .HasColumnName(nameof(AccountType.LastSyncDate));

        builder.Property(x => x.SyncRecordId)
            .HasColumnName(nameof(AccountType.SyncRecordId));

        builder.Property(x => x.SyncRecordCode)
            .HasMaxLength(AccountTypeConsts.MaxSyncRecordCodeLength)
            .HasColumnName(nameof(AccountType.SyncRecordCode));

        // --- Foreign Keys ---
        // AccountType không có foreign key trong kế hoạch này

        // --- Indexes ---
        builder.HasIndex(x => x.Code)
               .IsUnique() // Mã AccountType phải là duy nhất
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AccountTypes_Code"); // Đặt tên index rõ ràng

        // Index trên Name (không cần unique) để tăng tốc tìm kiếm/sắp xếp
        builder.HasIndex(x => x.Name)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AccountTypes_Name");

        // Index trên Status để tăng tốc lọc theo trạng thái
        builder.HasIndex(x => x.Status)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AccountTypes_Status");

        // Index kết hợp cho sắp xếp/lọc phổ biến (ví dụ: theo Order -> Name)
        builder.HasIndex(x => new { x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AccountTypes_Order_Name");
    }
}