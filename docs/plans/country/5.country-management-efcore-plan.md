# Kế hoạch chi tiết: Tầng EntityFrameworkCore (`Aqt.CoreFW.EntityFrameworkCore`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.EntityFrameworkCore`.

## 1. DbContext

- **Vị trí:** Cập nhật file `src/Aqt.CoreFW.EntityFrameworkCore/EntityFrameworkCore/CoreFWDbContext.cs`
- **Nội dung cần thêm:**
  ```csharp
  using Aqt.CoreFW.Domain.Countries.Entities;
  using Microsoft.EntityFrameworkCore;
  // ... other usings ...

  public class CoreFWDbContext : AbpDbContext<CoreFWDbContext>, /* ... các interface khác ... */
  {
      // ... các DbSet khác ...
      public DbSet<Country> Countries { get; set; }

      public CoreFWDbContext(DbContextOptions<CoreFWDbContext> options)
          : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder builder)
      {
          base.OnModelCreating(builder);

          // Cấu hình các module khác của ABP...
          // builder.ConfigurePermissionManagement();
          // builder.ConfigureSettingManagement();
          // ...

          // Áp dụng tất cả cấu hình entity trong assembly này
          // Đảm bảo dòng này được gọi SAU base.OnModelCreating và các Configure... của ABP
          builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
      }
  }
  ```
- **Lưu ý quan trọng:** Đảm bảo rằng phương thức `OnModelCreating` có dòng `builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());`. Nếu không có, bạn cần thêm nó vào cuối phương thức để cấu hình `CountryConfiguration` được tự động áp dụng.

## 2. Repository Implementation

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.EntityFrameworkCore/Countries`
- **Tệp:** Tạo file `CountryRepository.cs`
- **Nội dung:**
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Dynamic.Core; // Cần cho sorting dạng chuỗi và PageBy
  using System.Threading;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Domain.Countries.Repositories;
  using Aqt.CoreFW.Domain.Countries.Entities;
  using Aqt.CoreFW.Domain.Provinces.Entities; // Cần để kiểm tra HasProvincesAsync
  using Aqt.CoreFW.EntityFrameworkCore; // Namespace của DbContext
  using Microsoft.EntityFrameworkCore;
  using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
  using Volo.Abp.EntityFrameworkCore;

  namespace Aqt.CoreFW.EntityFrameworkCore.Countries;

  public class CountryRepository :
      EfCoreRepository<CoreFWDbContext, Country, Guid>, // Kế thừa EfCoreRepository
      ICountryRepository // Implement interface
  {
      public CountryRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
          : base(dbContextProvider)
      {
      }

      public async Task<Country?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          return await dbSet.FirstOrDefaultAsync(c => c.Code == code, GetCancellationToken(cancellationToken));
      }

      public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          return await dbSet.AnyAsync(c => c.Code == code && (!excludedId.HasValue || c.Id != excludedId.Value), GetCancellationToken(cancellationToken));
      }

      public async Task<List<Country>> GetListAsync(
          string? filterText = null,
          string? sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          var query = dbSet
              .WhereIf(!filterText.IsNullOrWhiteSpace(),
                  c => c.Code.Contains(filterText) || c.Name.Contains(filterText));

          // Áp dụng sắp xếp, nếu không có thì sắp xếp theo Name mặc định
          query = query.OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Country.Name) + " asc" : sorting);

          return await query.PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
      }

      public async Task<long> GetCountAsync(
          string? filterText = null,
          CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          var query = dbSet
              .WhereIf(!filterText.IsNullOrWhiteSpace(),
                  c => c.Code.Contains(filterText) || c.Name.Contains(filterText));

          return await query.LongCountAsync(GetCancellationToken(cancellationToken));
      }

      public async Task<bool> HasProvincesAsync(Guid countryId, CancellationToken cancellationToken = default)
      {
          var dbContext = await GetDbContextAsync();
          // Truy cập DbSet<Province> qua DbContext để kiểm tra
          // Đảm bảo rằng DbSet<Province> đã được khai báo trong CoreFWDbContext
          if(dbContext.Provinces == null){
               // Xử lý trường hợp DbSet chưa được khởi tạo nếu cần, hoặc throw lỗi
               // Ví dụ: throw new InvalidOperationException("Provinces DbSet is not available in the DbContext.");
               // Hoặc trả về false/true tùy theo logic nghiệp vụ mong muốn khi không tìm thấy DbSet
               return false; // Giả sử không có tỉnh nếu DbSet không tồn tại
          }
          return await dbContext.Provinces.AnyAsync(p => p.CountryId == countryId, GetCancellationToken(cancellationToken));
      }

      // Ghi đè GetQueryableAsync nếu cần thêm Include mặc định (không cần thiết cho Country đơn giản)
      // public override async Task<IQueryable<Country>> GetQueryableAsync()
      // {
      //     return (await base.GetQueryableAsync()); //.Include(...) nếu cần
      // }
  }
  ```
- **Lưu ý về `HasProvincesAsync`:** Cần đảm bảo `DbSet<Province> Provinces { get; set; }` đã được khai báo trong `CoreFWDbContext.cs`. Nếu chưa, cần thêm vào và tạo migration tương ứng cho Province trước.

## 3. Entity Configuration

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.EntityFrameworkCore/EntityTypeConfigurations/Countries`
- **Tệp:** Tạo file `CountryConfiguration.cs`
- **Nội dung:**
  ```csharp
  using Aqt.CoreFW.Domain.Countries.Entities;
  using Aqt.CoreFW.Domain.Shared;          // For CoreFWConsts
  using Aqt.CoreFW.Domain.Shared.Countries; // For CountryConsts
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;
  using Volo.Abp.EntityFrameworkCore.Modeling;
  using System.Reflection; // Required for Assembly

  namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Countries;

  public class CountryConfiguration : IEntityTypeConfiguration<Country>
  {
      public void Configure(EntityTypeBuilder<Country> builder)
      {
          // Cấu hình tên bảng và schema (nếu có)
          // Kiểm tra CoreFWConsts.DbSchema có giá trị hay không trước khi sử dụng
          var schema = CoreFWConsts.DbSchema;
          builder.ToTable(CoreFWConsts.DbTablePrefix + "Countries", schema);

          // Áp dụng các cấu hình convention của ABP (như soft delete, audit properties)
          builder.ConfigureByConvention();

          // Khóa chính
          builder.HasKey(x => x.Id);

          // Cấu hình cột Code
          builder.Property(x => x.Code)
              .IsRequired() // Bắt buộc
              .HasMaxLength(CountryConsts.MaxCodeLength) // Độ dài tối đa
              .HasColumnName(nameof(Country.Code)); // Tên cột trong DB

          // Cấu hình cột Name
          builder.Property(x => x.Name)
              .IsRequired()
              .HasMaxLength(CountryConsts.MaxNameLength)
              .HasColumnName(nameof(Country.Name));

          // Index để đảm bảo Code là duy nhất
          builder.HasIndex(x => x.Code).IsUnique();

          // Index cho Name để tăng tốc tìm kiếm (tùy chọn)
          builder.HasIndex(x => x.Name);

          // Các cấu hình khác nếu cần...
      }
  }
  ```

</rewritten_file> 