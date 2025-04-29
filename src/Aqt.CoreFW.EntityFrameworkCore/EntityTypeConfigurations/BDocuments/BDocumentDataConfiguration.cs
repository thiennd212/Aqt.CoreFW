using Aqt.CoreFW.Domain.BDocuments.Entities; // BDocumentData Entity
using Aqt.CoreFW.Domain.Shared; // CoreFWConsts
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.BDocuments;

/// <summary>
/// Configures the database mapping for the <see cref="BDocumentData"/> entity.
/// </summary>
public class BDocumentDataConfiguration : IEntityTypeConfiguration<BDocumentData>
{
    public void Configure(EntityTypeBuilder<BDocumentData> builder)
    {
        // Table Name and Schema
        builder.ToTable(CoreFWConsts.DbTablePrefix + "BDocumentData", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Apply standard conventions (AuditedEntity)

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.BDocumentId)
            .IsRequired()
            .HasColumnName(nameof(BDocumentData.BDocumentId));

        builder.Property(x => x.ProcedureComponentId)
            .IsRequired()
            .HasColumnName(nameof(BDocumentData.ProcedureComponentId));

        // Cấu hình cho DuLieuNhap - cần kiểu dữ liệu lớn cho JSON
        builder.Property(x => x.DuLieuNhap)
            .HasColumnName(nameof(BDocumentData.DuLieuNhap))
            .IsRequired(false) // Nullable
            .HasColumnType("NCLOB"); // Kiểu dữ liệu lớn cho SQL Server, hoặc "text" cho PostgreSQL/MySQL

        // FileId là nullable Guid?
        builder.Property(x => x.FileId)
           .HasColumnName(nameof(BDocumentData.FileId))
           .IsRequired(false); // Nullable

        // --- Relationships ---
        // Relationship back to BDocument đã được định nghĩa trong BDocumentConfiguration
        // Không cần định nghĩa lại ở đây, EF Core sẽ tự động hiểu từ cấu hình HasMany của BDocument

        // --- Indexes ---
        builder.HasIndex(x => x.BDocumentId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_BDocumentId");

        builder.HasIndex(x => x.ProcedureComponentId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_ProcedureComponentId");

        builder.HasIndex(x => x.FileId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_FileId");

        // Composite index đảm bảo mỗi component chỉ có 1 entry cho mỗi document
        builder.HasIndex(x => new { x.BDocumentId, x.ProcedureComponentId })
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_Doc_Comp");
    }
}