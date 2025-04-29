using Aqt.CoreFW.BDocuments; // Namespace chứa BDocumentConsts
using Aqt.CoreFW.Domain.BDocuments.Entities; // BDocument Entity
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure Entity for relationship
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities; // WorkflowStatus Entity for relationship
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.BDocuments;

/// <summary>
/// Configures the database mapping for the <see cref="BDocument"/> entity.
/// </summary>
public class BDocumentConfiguration : IEntityTypeConfiguration<BDocument>
{
    public void Configure(EntityTypeBuilder<BDocument> builder)
    {
        // Sử dụng DbTablePrefix và DbSchema từ CoreFWConsts
        builder.ToTable(CoreFWConsts.DbTablePrefix + "BDocuments", CoreFWConsts.DbSchema);

        builder.ConfigureByConvention(); // Áp dụng quy ước FullAuditedAggregateRoot

        builder.HasKey(x => x.Id);

        // --- Property Configurations ---
        builder.Property(x => x.ProcedureId)
            .IsRequired()
            .HasColumnName(nameof(BDocument.ProcedureId));

        builder.Property(x => x.MaHoSo)
            .IsRequired()
            .HasMaxLength(BDocumentConsts.MaxMaHoSoLength)
            .HasColumnName(nameof(BDocument.MaHoSo));

        builder.Property(x => x.TenChuHoSo)
            .IsRequired()
            .HasMaxLength(BDocumentConsts.MaxTenChuHoSoLength)
            .HasColumnName(nameof(BDocument.TenChuHoSo));

        builder.Property(x => x.SoDinhDanhChuHoSo)
            .HasMaxLength(BDocumentConsts.MaxSoDinhDanhChuHoSoLength)
            .HasColumnName(nameof(BDocument.SoDinhDanhChuHoSo));

        builder.Property(x => x.DiaChiChuHoSo)
            .HasMaxLength(BDocumentConsts.MaxDiaChiChuHoSoLength)
            .HasColumnName(nameof(BDocument.DiaChiChuHoSo));

        builder.Property(x => x.EmailChuHoSo)
            .HasMaxLength(BDocumentConsts.MaxEmailChuHoSoLength)
            .HasColumnName(nameof(BDocument.EmailChuHoSo));

        builder.Property(x => x.SoDienThoaiChuHoSo)
            .HasMaxLength(BDocumentConsts.MaxSoDienThoaiChuHoSoLength)
            .HasColumnName(nameof(BDocument.SoDienThoaiChuHoSo));

        // Cấu hình trường mới
        builder.Property(x => x.PhamViHoatDong)
            .HasColumnName(nameof(BDocument.PhamViHoatDong))
            .HasColumnType("NCLOB") // Hoặc "text" tùy DB
            .IsRequired(false); // Nullable

        builder.Property(x => x.DangKyNhanQuaBuuDien)
            .IsRequired() // bool không nullable
            .HasColumnName(nameof(BDocument.DangKyNhanQuaBuuDien));

        // TrangThaiHoSoId là nullable Guid?
        builder.Property(x => x.TrangThaiHoSoId)
            .HasColumnName(nameof(BDocument.TrangThaiHoSoId))
            .IsRequired(false); // Nullable

        builder.Property(x => x.NgayNop).HasColumnName(nameof(BDocument.NgayNop));
        builder.Property(x => x.NgayTiepNhan).HasColumnName(nameof(BDocument.NgayTiepNhan));
        builder.Property(x => x.NgayHenTra).HasColumnName(nameof(BDocument.NgayHenTra));
        builder.Property(x => x.NgayTraKetQua).HasColumnName(nameof(BDocument.NgayTraKetQua));

        builder.Property(x => x.LyDoTuChoiHoacBoSung)
            .HasMaxLength(BDocumentConsts.MaxLyDoTuChoiHoacBoSungLength)
            .HasColumnName(nameof(BDocument.LyDoTuChoiHoacBoSung));

        // --- Relationships ---

        // Relationship với Procedure
        builder.HasOne(d => d.Procedure)
               .WithMany() // Procedure không có navigation property ngược lại
               .HasForeignKey(d => d.ProcedureId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Procedure nếu có BDocument liên quan

        // Relationship với WorkflowStatus
        builder.HasOne(d => d.TrangThaiHoSo)
               .WithMany() // WorkflowStatus không có navigation property ngược lại
               .HasForeignKey(d => d.TrangThaiHoSoId)
               .IsRequired(false) // FK is nullable
               .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Status nếu có BDocument liên quan

        // Relationship với BDocumentData
        builder.HasMany(d => d.DocumentData)
               .WithOne() // Không cần navigation property ngược từ BDocumentData
               .HasForeignKey(dd => dd.BDocumentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // Xóa data khi xóa document


        // --- Indexes ---

        // Index cho MaHoSo (Unique)
        builder.HasIndex(x => x.MaHoSo)
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_MaHoSo");

        // Indexes cho khóa ngoại và trường hay lọc
        builder.HasIndex(x => x.ProcedureId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_ProcedureId");

        builder.HasIndex(x => x.TrangThaiHoSoId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_TrangThaiHoSoId");

        builder.HasIndex(x => x.TenChuHoSo)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_TenChuHoSo");

        builder.HasIndex(x => x.CreationTime)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_CreationTime");

        // Index tổng hợp cho các bộ lọc thường dùng
        builder.HasIndex(x => new { x.ProcedureId, x.TrangThaiHoSoId, x.CreationTime })
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_Proc_Status_Created");

        // Thêm index cho trường mới nếu cần lọc nhiều
        builder.HasIndex(x => x.DangKyNhanQuaBuuDien)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_DangKyBuuDien");
    }
}