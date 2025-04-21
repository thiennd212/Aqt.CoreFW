using Aqt.CoreFW.Domain.Ranks.Entities;
using Aqt.CoreFW.Domain.Shared; // Namespace chứa CoreFWConsts (Nếu có)
using Aqt.CoreFW.Ranks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Ranks
{
    /// <summary>
    /// Configures the database mapping for the <see cref="Rank"/> entity.
    /// </summary>
    public class RankConfiguration : IEntityTypeConfiguration<Rank>
    {
        public void Configure(EntityTypeBuilder<Rank> builder)
        {
            // Giả sử bạn có CoreFWConsts.DbTablePrefix và CoreFWConsts.DbSchema
            // Nếu không, thay thế bằng chuỗi cứng hoặc bỏ qua Schema
            builder.ToTable(CoreFWConsts.DbTablePrefix + "Ranks", CoreFWConsts.DbSchema);

            builder.ConfigureByConvention();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(RankConsts.MaxCodeLength)
                .HasColumnName(nameof(Rank.Code));

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(RankConsts.MaxNameLength)
                .HasColumnName(nameof(Rank.Name));

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnName(nameof(Rank.Status))
                .HasConversion<byte>(); // Map enum RankStatus sang kiểu byte

            builder.Property(x => x.Order)
                .IsRequired()
                .HasColumnName(nameof(Rank.Order));

            builder.Property(x => x.Description)
                .HasMaxLength(RankConsts.MaxDescriptionLength)
                .HasColumnName(nameof(Rank.Description));

            builder.Property(x => x.LastSyncDate)
                .HasColumnName(nameof(Rank.LastSyncDate));

            builder.Property(x => x.SyncRecordId)
                .HasColumnName(nameof(Rank.SyncRecordId));

            builder.Property(x => x.SyncRecordCode)
                .HasMaxLength(RankConsts.MaxSyncRecordCodeLength)
                .HasColumnName(nameof(Rank.SyncRecordCode));

            builder.HasIndex(x => x.Code)
                   .IsUnique()
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Ranks_Code");

            builder.HasIndex(x => x.Name)
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Ranks_Name");

            builder.HasIndex(x => x.Status)
                   .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Ranks_Status");

            builder.HasIndex(x => new { x.Status, x.Order, x.Name })
                  .HasDatabaseName($"IX_{CoreFWConsts.DbTablePrefix}Ranks_Status_Order_Name");
        }
    }
}