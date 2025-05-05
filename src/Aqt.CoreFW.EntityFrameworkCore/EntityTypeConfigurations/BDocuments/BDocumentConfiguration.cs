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
/// Cấu hình ánh xạ cơ sở dữ liệu cho thực thể <see cref="BDocument"/>.
/// </summary>
public class BDocumentConfiguration : IEntityTypeConfiguration<BDocument>
{
    public void Configure(EntityTypeBuilder<BDocument> builder)
    {
        // Tên bảng và schema
        builder.ToTable(CoreFWConsts.DbTablePrefix + "BDocuments", CoreFWConsts.DbSchema);

        // Áp dụng các quy ước chuẩn cho FullAuditedAggregateRoot
        builder.ConfigureByConvention();

        builder.HasKey(x => x.Id);

        // --- Cấu hình thuộc tính --- 
        builder.Property(x => x.ProcedureId)
            .IsRequired()
            .HasColumnName(nameof(BDocument.ProcedureId)); // Tên cột khớp tên thuộc tính

        builder.Property(x => x.Code) // Đổi sang tiếng Anh
            .IsRequired()
            .HasMaxLength(BDocumentConsts.MaxCodeLength) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.Code)); // Đổi sang tiếng Anh

        builder.Property(x => x.ApplicantName) // Đổi sang tiếng Anh
            .IsRequired()
            .HasMaxLength(BDocumentConsts.MaxApplicantNameLength) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.ApplicantName)); // Đổi sang tiếng Anh

        builder.Property(x => x.ApplicantIdentityNumber) // Đổi sang tiếng Anh
            .HasMaxLength(BDocumentConsts.MaxApplicantIdentityNumberLength) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.ApplicantIdentityNumber)); // Đổi sang tiếng Anh

        builder.Property(x => x.ApplicantAddress) // Đổi sang tiếng Anh
            .HasMaxLength(BDocumentConsts.MaxApplicantAddressLength) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.ApplicantAddress)); // Đổi sang tiếng Anh

        builder.Property(x => x.ApplicantEmail) // Đổi sang tiếng Anh
            .HasMaxLength(BDocumentConsts.MaxApplicantEmailLength) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.ApplicantEmail)); // Đổi sang tiếng Anh

        builder.Property(x => x.ApplicantPhoneNumber) // Đổi sang tiếng Anh
            .HasMaxLength(BDocumentConsts.MaxApplicantPhoneNumberLength) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.ApplicantPhoneNumber)); // Đổi sang tiếng Anh

        // Cấu hình trường mới: Phạm vi hoạt động
        builder.Property(x => x.ScopeOfActivity) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.ScopeOfActivity)) // Đổi sang tiếng Anh
            //.HasColumnType("NCLOB") // Bỏ comment - Kiểu dữ liệu lớn cho Oracle
            .IsRequired(false); // Cho phép null

        // Cấu hình trường mới: Đăng ký nhận qua bưu điện
        builder.Property(x => x.ReceiveByPost) // Đổi sang tiếng Anh
            .IsRequired() // Kiểu bool không cho phép null
            .HasColumnName(nameof(BDocument.ReceiveByPost)); // Đổi sang tiếng Anh

        // Cấu hình trường mới: Trạng thái quy trình
        builder.Property(x => x.WorkflowStatusId) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.WorkflowStatusId)) // Đổi sang tiếng Anh
            .IsRequired(false); // Cho phép null

        // Cấu hình các trường ngày tháng
        builder.Property(x => x.SubmissionDate).HasColumnName(nameof(BDocument.SubmissionDate)); // Đổi sang tiếng Anh
        builder.Property(x => x.ReceptionDate).HasColumnName(nameof(BDocument.ReceptionDate)); // Đổi sang tiếng Anh
        builder.Property(x => x.AppointmentDate).HasColumnName(nameof(BDocument.AppointmentDate)); // Đổi sang tiếng Anh
        builder.Property(x => x.ResultDate).HasColumnName(nameof(BDocument.ResultDate)); // Đổi sang tiếng Anh

        // Cấu hình trường lý do từ chối/bổ sung
        builder.Property(x => x.RejectionOrAdditionReason) // Đổi sang tiếng Anh
            .HasMaxLength(BDocumentConsts.MaxRejectionOrAdditionReasonLength) // Đổi sang tiếng Anh
            .HasColumnName(nameof(BDocument.RejectionOrAdditionReason)); // Đổi sang tiếng Anh

        // Configure ExtraProperties JSON mapping if needed (already handled by ConfigureByConvention usually)
        // builder.Property(x => x.ExtraProperties).HasJsonConversion();

        // --- Cấu hình quan hệ --- 

        // Quan hệ với Procedure (Thủ tục)
        builder.HasOne(d => d.Procedure)
               .WithMany() // Procedure không cần navigation property ngược lại
               .HasForeignKey(d => d.ProcedureId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Procedure nếu có BDocument liên quan

        // Quan hệ với WorkflowStatus (Trạng thái quy trình)
        builder.HasOne(d => d.WorkflowStatus) // Đổi sang tiếng Anh
               .WithMany() // WorkflowStatus không cần navigation property ngược lại
               .HasForeignKey(d => d.WorkflowStatusId) // Đổi sang tiếng Anh
               .IsRequired(false) // Khóa ngoại có thể null
               .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Status nếu có BDocument liên quan

        // Quan hệ với BDocumentData (Dữ liệu chi tiết hồ sơ)
        builder.HasMany(d => d.DocumentData)
               .WithOne() // Không cần navigation property ngược từ BDocumentData
               .HasForeignKey(dd => dd.BDocumentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // Xóa data khi xóa document chính

        // --- Cấu hình chỉ mục (Indexes) --- 

        // Chỉ mục cho Code (đảm bảo duy nhất)
        builder.HasIndex(x => x.Code) // Đổi sang tiếng Anh
               .IsUnique()
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_Code"); // Cập nhật tên index

        // Chỉ mục cho các khóa ngoại và trường hay lọc
        builder.HasIndex(x => x.ProcedureId)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_ProcedureId");

        builder.HasIndex(x => x.WorkflowStatusId) // Đổi sang tiếng Anh
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_WorkflowStatusId"); // Cập nhật tên index

        builder.HasIndex(x => x.ApplicantName) // Đổi sang tiếng Anh
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_ApplicantName"); // Cập nhật tên index

        builder.HasIndex(x => x.CreationTime)
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_CreationTime");

        // Chỉ mục tổng hợp cho các bộ lọc thường dùng
        builder.HasIndex(x => new { x.ProcedureId, x.WorkflowStatusId, x.CreationTime }) // Đổi sang tiếng Anh
              .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_Proc_Status_Created"); // Cập nhật tên index

        // Chỉ mục cho trường mới nếu cần lọc nhiều
        builder.HasIndex(x => x.ReceiveByPost) // Đổi sang tiếng Anh
               .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}BDocuments_ReceiveByPost"); // Cập nhật tên index
    }
}