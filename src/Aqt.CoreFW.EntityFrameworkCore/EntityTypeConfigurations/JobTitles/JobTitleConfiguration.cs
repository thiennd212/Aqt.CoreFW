using Aqt.CoreFW.Domain.JobTitles.Entities;     // Entity JobTitle
using Aqt.CoreFW.Domain.Shared.JobTitles;     // JobTitleConsts
using Microsoft.EntityFrameworkCore;          // IEntityTypeConfiguration, DbContextOptionsBuilder
using Microsoft.EntityFrameworkCore.Metadata.Builders; // EntityTypeBuilder
using Volo.Abp.EntityFrameworkCore.Modeling;  // ConfigureByConvention

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.JobTitles;

/// <summary>
/// Configures the database mapping for the JobTitle entity.
/// </summary>
public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
{
    public void Configure(EntityTypeBuilder<JobTitle> builder)
    {
        // Lấy schema và tiền tố bảng từ hằng số
        builder.ToTable(CoreFWConsts.DbTablePrefix + "JobTitles", CoreFWConsts.DbSchema);

        // Áp dụng các cấu hình chuẩn của ABP cho các thuộc tính cơ sở
        // (Id, ExtraProperties, ConcurrencyStamp, và các thuộc tính audit nếu kế thừa Audited...)
        builder.ConfigureByConvention();

        // --- Cấu hình các thuộc tính cụ thể của JobTitle ---

        // Khóa chính đã được cấu hình bởi ConfigureByConvention (thường là Id)
        // builder.HasKey(x => x.Id); // Thường không cần nếu Id là tên khóa chính và được ConfigureByConvention xử lý

        // Cột Code
        builder.Property(x => x.Code)
            .IsRequired() // Tương đương NOT NULL trong SQL
            .HasMaxLength(JobTitleConsts.MaxCodeLength) // Độ dài tối đa
            .HasColumnName(nameof(JobTitle.Code)); // Đặt tên cột rõ ràng (tùy chọn, thường EF tự làm đúng)

        // Cột Name
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(JobTitleConsts.MaxNameLength)
            .HasColumnName(nameof(JobTitle.Name));

        // Cột Description
        builder.Property(x => x.Description)
            .HasMaxLength(JobTitleConsts.MaxDescriptionLength) // Cho phép NULL (vì string? trong entity)
            .HasColumnName(nameof(JobTitle.Description));

        // Cột IsActive
        builder.Property(x => x.IsActive)
            .IsRequired() // boolean không thể null
            .HasDefaultValue(true) // Đặt giá trị mặc định ở cấp DB là true
            .HasColumnName(nameof(JobTitle.IsActive));

        builder.Property(x => x.LastSyncDate)
                .HasColumnName(nameof(JobTitle.LastSyncDate));

        builder.Property(x => x.SyncRecordId)
            .HasColumnName(nameof(JobTitle.SyncRecordId));

        builder.Property(x => x.SyncRecordCode)
            .HasMaxLength(JobTitleConsts.MaxSyncRecordCodeLength)
            .HasColumnName(nameof(JobTitle.SyncRecordCode));

        // --- Cấu hình Chỉ mục (Indexes) ---

        // **Rất quan trọng:** Index duy nhất cho cột Code để đảm bảo không trùng lặp ở DB
        builder.HasIndex(x => x.Code)
               .IsUnique();

        // Index cho cột Name để tăng tốc độ tìm kiếm/sắp xếp theo tên (tùy chọn)
        builder.HasIndex(x => x.Name);

        // Index cho cột IsActive để tăng tốc độ lọc theo trạng thái (tùy chọn)
        builder.HasIndex(x => x.IsActive);

        // Cấu hình các thuộc tính audit (CreationTime, CreatorId,...)
        // thường được xử lý bởi ConfigureByConvention nếu bạn kế thừa đúng lớp base entity của ABP.
    }
}