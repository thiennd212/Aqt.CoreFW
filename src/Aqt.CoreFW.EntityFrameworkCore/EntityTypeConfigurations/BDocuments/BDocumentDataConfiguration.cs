using Aqt.CoreFW.Domain.BDocuments.Entities; // BDocumentData Entity
using Aqt.CoreFW.Domain.Shared; // **Added for CoreFWConsts**
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.BDocuments;

/// <summary>
/// Cấu hình ánh xạ cơ sở dữ liệu cho thực thể <see cref="BDocumentData"/>.
/// </summary>
public class BDocumentDataConfiguration : IEntityTypeConfiguration<BDocumentData>
{
    public void Configure(EntityTypeBuilder<BDocumentData> builder)
    {
        // Tên bảng và schema
        builder.ToTable(CoreFWConsts.DbTablePrefix + "BDocumentData", CoreFWConsts.DbSchema);

        // Áp dụng các quy ước chuẩn cho AuditedEntity
        builder.ConfigureByConvention();

        builder.HasKey(x => x.Id);

        // --- Cấu hình thuộc tính --- 
        builder.Property(x => x.BDocumentId)
            .IsRequired()
            .HasColumnName(nameof(BDocumentData.BDocumentId)); // Tên cột khớp tên thuộc tính

        builder.Property(x => x.ProcedureComponentId)
            .IsRequired()
            .HasColumnName(nameof(BDocumentData.ProcedureComponentId)); // Tên cột khớp tên thuộc tính

        // Cấu hình cho InputData (dữ liệu nhập, thường là JSON)
        builder.Property(x => x.InputData) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocumentData.InputData)) // Đổi sang tiếng Anh
            .IsRequired(false) // Cho phép null
            .HasColumnType("NCLOB"); // Kiểu dữ liệu lớn cho Oracle (hoặc phù hợp với DB khác)

        // Cấu hình cho FileId (khóa ngoại đến bảng FileManagement)
        builder.Property(x => x.FileId)
           .HasColumnName(nameof(BDocumentData.FileId)) // Tên cột khớp tên thuộc tính
           .IsRequired(false); // Cho phép null

        // --- Cấu hình quan hệ --- 
        // Quan hệ ngược về BDocument đã được định nghĩa trong BDocumentConfiguration
        // Không cần định nghĩa lại ở đây, EF Core sẽ tự động hiểu từ cấu hình HasMany của BDocument

        // --- Cấu hình chỉ mục (Indexes) --- 
        builder.HasIndex(x => x.BDocumentId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_BDocumentId");

        builder.HasIndex(x => x.ProcedureComponentId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_ProcedureComponentId");

        builder.HasIndex(x => x.FileId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_FileId");

        // Chỉ mục tổng hợp đảm bảo mỗi component chỉ có 1 entry cho mỗi document
        builder.HasIndex(x => new { x.BDocumentId, x.ProcedureComponentId })
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocumentData_Doc_Comp");
    }
}