using Aqt.CoreFW.Domain.AttachedDocuments.Entities; // AttachedDocument Entity
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity for FK relation (!! Giả định namespace và tên Entity, cần xác minh !!)
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts
using Aqt.CoreFW.AttachedDocuments; // Namespace chứa AttachedDocumentConsts và AttachedDocumentStatus enum
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.AttachedDocuments; // Namespace Configuration

/// <summary>
/// Configures the database mapping for the <see cref="AttachedDocument"/> entity.
/// </summary>
public class AttachedDocumentConfiguration : IEntityTypeConfiguration<AttachedDocument>
{
    public void Configure(EntityTypeBuilder<AttachedDocument> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts
        builder.ToTable(CoreFWConsts.DbTablePrefix + "AttachedDocuments", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng các quy ước chuẩn của ABP

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(AttachedDocumentConsts.MaxCodeLength)
            .HasColumnName(nameof(AttachedDocument.Code));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(AttachedDocumentConsts.MaxNameLength)
            .HasColumnName(nameof(AttachedDocument.Name));

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName(nameof(AttachedDocument.Status))
            .HasConversion<byte>(); // Map enum sang byte

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName(nameof(AttachedDocument.Order));

        builder.Property(x => x.Description)
            .HasMaxLength(AttachedDocumentConsts.MaxDescriptionLength)
            .HasColumnName(nameof(AttachedDocument.Description));

        // ProcedureId là Guid bắt buộc
        builder.Property(x => x.ProcedureId)
            .IsRequired()
            .HasColumnName(nameof(AttachedDocument.ProcedureId));

        // --- Foreign Keys ---
        // (!! Giả định Entity tên Procedure trong namespace Aqt.CoreFW.Domain.Procedures.Entities, cần xác minh !!)
        builder.HasOne<Procedure>() // Không cần chỉ định navigation property 'Procedure' nếu không có trong Entity
               .WithMany() // Một Procedure có thể có nhiều AttachedDocument (không cần navigation property ở Procedure)
               .HasForeignKey(x => x.ProcedureId)
               .IsRequired() // Đảm bảo FK là bắt buộc
               .OnDelete(DeleteBehavior.Restrict); // Quan trọng: Ngăn chặn việc xóa Procedure nếu còn AttachedDocument tham chiếu

        // --- Indexes ---

        // Index cho Code (Unique theo ProcedureId)
        builder.HasIndex(x => new { x.ProcedureId, x.Code })
               .IsUnique() // Đảm bảo Code là duy nhất trong phạm vi ProcedureId
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AttachedDocuments_ProcedureId_Code");

        builder.HasIndex(x => x.Name)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AttachedDocuments_Name");

        builder.HasIndex(x => x.Status)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AttachedDocuments_Status");

        // Index trên ProcedureId (đã có trong index kết hợp bên trên, nhưng thêm riêng cũng không sao)
        builder.HasIndex(x => x.ProcedureId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AttachedDocuments_ProcedureId");

        // Index kết hợp cho sắp xếp/lọc phổ biến trong cùng thủ tục
        builder.HasIndex(x => new { x.ProcedureId, x.Status, x.Order, x.Name })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}AttachedDocuments_ProcedureId_Status_Order_Name");
    }
}