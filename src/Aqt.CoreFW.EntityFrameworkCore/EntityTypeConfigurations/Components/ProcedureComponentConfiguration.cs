    using Aqt.CoreFW.Components; // Namespace chứa ComponentConsts và Enums
    using Aqt.CoreFW.Domain.Components.Entities; // Component Entity
    // using Aqt.CoreFW.Domain.Procedures.Entities; // Không cần trực tiếp ở đây nếu không config FK từ đây
    using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Volo.Abp.EntityFrameworkCore.Modeling;

    namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Components; // Namespace Configuration

    /// <summary>
    /// Configures the database mapping for the <see cref="ProcedureComponent"/> entity.
    /// </summary>
    public class ProcedureComponentConfiguration : IEntityTypeConfiguration<ProcedureComponent>
    {
        public void Configure(EntityTypeBuilder<ProcedureComponent> builder)
        {
            // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts
            builder.ToTable(CoreFWConsts.DbTablePrefix + "ProcedureComponents", CoreFWConsts.DbSchema);

            builder.ConfigureByConvention(); // Áp dụng các quy ước chuẩn của ABP

            builder.HasKey(x => x.Id);

            // --- Property Configurations ---
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(ComponentConsts.MaxCodeLength)
                .HasColumnName(nameof(ProcedureComponent.Code));

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(ComponentConsts.MaxNameLength)
                .HasColumnName(nameof(ProcedureComponent.Name));

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnName(nameof(ProcedureComponent.Status))
                .HasConversion<byte>(); // Map enum sang byte

            builder.Property(x => x.Order)
                .IsRequired()
                .HasColumnName(nameof(ProcedureComponent.Order));

            builder.Property(x => x.Description)
                .HasMaxLength(ComponentConsts.MaxDescriptionLength)
                .HasColumnName(nameof(ProcedureComponent.Description));

            builder.Property(x => x.Type)
                .IsRequired()
                .HasColumnName(nameof(ProcedureComponent.Type))
                .HasConversion<byte>(); // Map enum sang byte

            // Cấu hình cho FormDefinition - có thể cần kiểu dữ liệu lớn
            builder.Property(x => x.FormDefinition)
                .HasColumnName(nameof(ProcedureComponent.FormDefinition))
                .IsRequired(false); // Nullable
                // .HasColumnType("nvarchar(max)"); // Bỏ comment nếu dùng SQL Server và cần nvarchar(max)
                // .HasColumnType("text"); // Bỏ comment nếu dùng PostgreSQL/MySQL và cần text

            builder.Property(x => x.TempPath)
                .HasMaxLength(ComponentConsts.MaxTempPathLength)
                .HasColumnName(nameof(ProcedureComponent.TempPath))
                .IsRequired(false); // Nullable

            // --- Relationships ---

            // Many-to-Many relationship with Procedure through ProcedureComponentLink
            builder.HasMany(c => c.ProcedureLinks) // Component has many Links
                   .WithOne() // Each Link relates to one Component (No navigation property back needed here)
                   .HasForeignKey(l => l.ProcedureComponentId) // Foreign key in the Link table
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // Xóa link nếu Component bị xóa


            // --- Indexes ---

            // Index cho Code (Unique)
            builder.HasIndex(x => x.Code)
                   .IsUnique() // Đảm bảo Code là duy nhất
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}ProcedureComponents_Code");

            builder.HasIndex(x => x.Name)
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}ProcedureComponents_Name");

            builder.HasIndex(x => x.Status)
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}ProcedureComponents_Status");

            builder.HasIndex(x => x.Type)
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}ProcedureComponents_Type");

            // Index kết hợp cho sắp xếp/lọc phổ biến
            builder.HasIndex(x => new { x.Status, x.Type, x.Order, x.Name })
                  .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}ProcedureComponents_Status_Type_Order_Name");
        }
    }