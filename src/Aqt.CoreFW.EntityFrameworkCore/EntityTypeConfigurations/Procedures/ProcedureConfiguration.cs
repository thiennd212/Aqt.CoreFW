using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts
using Aqt.CoreFW.Procedures; // Namespace chứa ProcedureConsts và ProcedureStatus enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Procedures; // Namespace Configuration

/// <summary>
/// Configures the database mapping for the <see cref="Procedure"/> entity.
/// </summary>
public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
{
    public void Configure(EntityTypeBuilder<Procedure> builder)
    {
        // Table mapping
        builder.ToTable(CoreFWConsts.DbTablePrefix + "Procedures", CoreFWConsts.DbSchema);

        // Apply standard ABP conventions (like setting common properties)
        builder.ConfigureByConvention();

        // Primary Key
        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired() // Mã là bắt buộc
            .HasMaxLength(ProcedureConsts.MaxCodeLength) // Giới hạn độ dài từ Domain.Shared
            .HasColumnName(nameof(Procedure.Code)); // Tên cột

        builder.Property(x => x.Name)
            .IsRequired() // Tên là bắt buộc
            .HasMaxLength(ProcedureConsts.MaxNameLength)
            .HasColumnName(nameof(Procedure.Name));

        builder.Property(x => x.Status)
            .IsRequired() // Trạng thái là bắt buộc
            .HasColumnName(nameof(Procedure.Status))
            .HasConversion<byte>(); // Lưu trữ Enum dưới dạng byte

        builder.Property(x => x.Order)
            .IsRequired() // Thứ tự là bắt buộc
            .HasDefaultValue(0) // Có thể đặt giá trị mặc định
            .HasColumnName(nameof(Procedure.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(ProcedureConsts.MaxDescriptionLength) // Cho phép null (theo entity)
            .HasColumnName(nameof(Procedure.Description));

        builder.Property(x => x.LastSyncedDate)
            .HasColumnName(nameof(Procedure.LastSyncedDate)); // Cho phép null

        builder.Property(x => x.SyncRecordId)
            .HasColumnName(nameof(Procedure.SyncRecordId)); // Cho phép null

        builder.Property(x => x.SyncRecordCode)
            .HasMaxLength(ProcedureConsts.MaxSyncRecordCodeLength) // Cho phép null
            .HasColumnName(nameof(Procedure.SyncRecordCode));

        // Configure Auditing properties if needed (though ConfigureByConvention might handle some)
        // builder.Property(x => x.CreationTime).IsRequired();
        // builder.Property(x => x.CreatorId);
        // ... etc.

        // --- Foreign Keys ---
        // Không có khóa ngoại nào được định nghĩa cho Procedure trong kế hoạch này.

        // --- Indexes ---

        // Index UNIQUE cho Code để đảm bảo tính duy nhất toàn hệ thống
        builder.HasIndex(x => x.Code)
               .IsUnique() // Ràng buộc UNIQUE
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Procedures_Code"); // Đặt tên index theo quy ước

        // Index cho Name để tăng tốc tìm kiếm/sắp xếp
        builder.HasIndex(x => x.Name)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Procedures_Name");

        // Index cho Status để tăng tốc lọc theo trạng thái
        builder.HasIndex(x => x.Status)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Procedures_Status");

        // Index cho SyncRecordCode (nếu thường xuyên tìm kiếm theo mã này)
        builder.HasIndex(x => x.SyncRecordCode)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Procedures_SyncRecordCode");

        // Index kết hợp cho các truy vấn lọc/sắp xếp phổ biến
        // Ví dụ: Lọc theo Status, sắp xếp theo Order rồi Name
        builder.HasIndex(x => new { x.Status, x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Procedures_Status_Order_Name");
    }
}