# Kế hoạch triển khai chức năng quản lý danh mục Tỉnh/Thành phố

## 0. Phân tích nghiệp vụ

### 0.1. Mục tiêu
- Xây dựng chức năng cho phép người quản trị hệ thống (System Administrator) hoặc người dùng được cấp quyền quản lý danh mục các Tỉnh/Thành phố thuộc về các Quốc gia đã có trong hệ thống.
- Đảm bảo dữ liệu Tỉnh/Thành phố (mã, tên, thuộc quốc gia nào) là nhất quán và chính xác.

### 0.2. Đối tượng sử dụng
- Quản trị viên hệ thống (System Administrator) hoặc người dùng được cấp quyền quản lý danh mục Tỉnh/Thành phố.

### 0.3. Yêu cầu chức năng chính (CRUD)
- **Xem danh sách (Read):** Hiển thị danh sách các Tỉnh/Thành phố đã có trong hệ thống dưới dạng bảng. Hỗ trợ lọc theo Quốc gia, phân trang và sắp xếp theo mã hoặc tên Tỉnh/Thành phố.
- **Thêm mới (Create):** Cho phép người dùng thêm một Tỉnh/Thành phố mới vào danh mục. Yêu cầu chọn Quốc gia, nhập Mã Tỉnh/Thành phố (Code) và Tên Tỉnh/Thành phố (Name).
- **Sửa (Update):** Cho phép người dùng chỉnh sửa thông tin (Quốc gia, Mã, Tên) của một Tỉnh/Thành phố đã tồn tại.
- **Xóa (Delete):** Cho phép người dùng xóa một Tỉnh/Thành phố khỏi danh mục. Sử dụng cơ chế xóa mềm (Soft Delete). Cần có bước xác nhận trước khi xóa.

### 0.4. Yêu cầu dữ liệu
- **Mã Tỉnh/Thành phố (Code):**
    - Kiểu dữ liệu: Chuỗi (String).
    - Bắt buộc nhập.
    - Độ dài tối đa: 10 ký tự (theo `ProvinceConsts`).
    - Phải là duy nhất (unique) *trong phạm vi một Quốc gia*.
- **Tên Tỉnh/Thành phố (Name):**
    - Kiểu dữ liệu: Chuỗi (String).
    - Bắt buộc nhập.
    - Độ dài tối đa: 256 ký tự (theo `ProvinceConsts`).
- **Mã Quốc gia (CountryId):**
    - Kiểu dữ liệu: Guid.
    - Bắt buộc chọn từ danh sách Quốc gia đã có.
    - Thiết lập khóa ngoại (Foreign Key) tới bảng Quốc gia.
- **Thông tin Audit:** Lưu trữ thông tin về người tạo, thời gian tạo, người sửa cuối cùng, thời gian sửa cuối cùng, trạng thái xóa mềm (IsDeleted). (Kế thừa từ `FullAuditedAggregateRoot`).

### 0.5. Yêu cầu giao diện người dùng (UI)
- **Màn hình danh sách:**
    - Dropdown hoặc bộ lọc để chọn Quốc gia.
    - Bảng hiển thị các cột: Mã Tỉnh/Thành, Tên Tỉnh/Thành, (có thể thêm Tên Quốc gia).
    - Nút "Thêm mới Tỉnh/Thành phố".
    - Các nút hành động (Sửa, Xóa) trên mỗi dòng của bảng.
    - Phân trang.
- **Modal Thêm mới/Sửa:**
    - Dropdown chọn Quốc gia.
    - Form nhập liệu cho Mã và Tên Tỉnh/Thành phố.
    - Các nút Lưu và Hủy.

### 0.6. Yêu cầu về phân quyền
- Cần định nghĩa các quyền riêng biệt cho việc xem danh sách, thêm, sửa, xóa Tỉnh/Thành phố.
- Chỉ những người dùng được gán quyền tương ứng mới có thể thực hiện các thao tác đó. Giao diện cần ẩn/hiện các nút chức năng dựa trên quyền của người dùng.

### 0.7. Quy tắc nghiệp vụ
- Mã Tỉnh/Thành phố không được trùng lặp *trong cùng một Quốc gia*. Hệ thống phải kiểm tra và thông báo lỗi nếu người dùng cố gắng tạo hoặc sửa thành một mã đã tồn tại cho quốc gia đó.
- Không cho phép xóa Quốc gia nếu Quốc gia đó vẫn còn Tỉnh/Thành phố liên kết (Cần xử lý ở chức năng quản lý Quốc gia hoặc đặt ràng buộc DB).

## Tóm tắt Tiến độ Thực hiện Dự kiến

- [ ] **Bước 1: Tầng Domain (`Aqt.CoreOracle.Domain`)**
- [ ] **Bước 2: Tầng Domain.Shared (`Aqt.CoreOracle.Domain.Shared`)**
- [ ] **Bước 3: Tầng Application.Contracts (`Aqt.CoreOracle.Application.Contracts`)**
- [ ] **Bước 4: Tầng Application (`Aqt.CoreOracle.Application`)**
- [ ] **Bước 5: Tầng EntityFrameworkCore (`Aqt.CoreOracle.EntityFrameworkCore`)**
- [ ] **Bước 6: Tầng Web (`Aqt.CoreOracle.Web`)**
- [ ] **Bước 7: Các bước triển khai và kiểm thử cuối cùng** (Build, chạy migrations, tạo JS proxies, kiểm thử)

## 1. Tầng Domain (`Aqt.CoreOracle.Domain`)

### Entity
- Tạo thư mục `Provinces/Entities`
- Tạo file `Province.cs`:
  ```csharp
  using System;
  using Aqt.CoreOracle.Domain.Countries.Entities; // Assuming Country entity exists
  using Aqt.CoreOracle.Domain.Shared.Provinces;
  using JetBrains.Annotations;
  using Volo.Abp;
  using Volo.Abp.Domain.Entities.Auditing;

  namespace Aqt.CoreOracle.Domain.Provinces.Entities;

  public class Province : FullAuditedAggregateRoot<Guid>
  {
      public virtual Guid CountryId { get; private set; }
      public virtual Country Country { get; private set; } // Navigation Property
      public string Code { get; private set; }
      public string Name { get; set; }

      protected Province() { /* For ORM */ }

      public Province(Guid id, Guid countryId, [NotNull] string code, [NotNull] string name) : base(id)
      {
          SetCountry(countryId);
          SetCode(code);
          SetName(name);
      }

      internal void SetCountry(Guid countryId)
      {
          CountryId = countryId;
          // Country navigation property will be loaded by EF Core if included
      }

      internal void SetCode([NotNull] string code)
      {
          Check.NotNullOrWhiteSpace(code, nameof(code));
          Check.Length(code, nameof(code), ProvinceConsts.MaxCodeLength);
          Code = code;
      }

      internal void SetName([NotNull] string name)
      {
          Check.NotNullOrWhiteSpace(name, nameof(name));
          Check.Length(name, nameof(name), ProvinceConsts.MaxNameLength);
          Name = name;
      }

      // Optional: Method to change country if allowed
      internal void ChangeCountry(Guid newCountryId)
      {
          SetCountry(newCountryId);
      }
  }
  ```

### Repository Interface
- Tạo thư mục `Provinces` (nếu chưa có ở root Domain)
- Tạo file `IProvinceRepository.cs`:
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Aqt.CoreOracle.Domain.Provinces.Entities;
  using Volo.Abp.Domain.Repositories;

  namespace Aqt.CoreOracle.Domain.Provinces;

  public interface IProvinceRepository : IRepository<Province, Guid>
  {
      // Finds a province by its code within a specific country
      Task<Province?> FindByCodeAsync(string code, Guid countryId, CancellationToken cancellationToken = default);

      // Checks if a province code already exists within a specific country, optionally excluding one ID (for updates)
      Task<bool> CodeExistsAsync(string code, Guid countryId, Guid? excludedId = null, CancellationToken cancellationToken = default);

      // Gets a list of provinces filtered by country ID, potentially with sorting and paging
      Task<List<Province>> GetListByCountryAsync(
          Guid countryId,
          string? sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default);

      // Gets the count of provinces filtered by country ID
      Task<long> GetCountByCountryAsync(Guid countryId, CancellationToken cancellationToken = default);

       // Optional: Get list including Country details
       Task<List<Province>> GetListWithCountryAsync(
          string? filterText = null,
          Guid? countryId = null,
          string? sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default);

       // Optional: Get count for the above list method
       Task<long> GetCountAsync(
           string? filterText = null,
           Guid? countryId = null,
           CancellationToken cancellationToken = default);
  }
  ``` 

## 2. Tầng Domain.Shared (`Aqt.CoreOracle.Domain.Shared`)

### Constants
- Tạo thư mục `Provinces`
- Tạo file `ProvinceConsts.cs`:
  ```csharp
  namespace Aqt.CoreOracle.Domain.Shared.Provinces;
  public static class ProvinceConsts
  {
      public const int MaxCodeLength = 10;
      public const int MaxNameLength = 256;
  }
  ```

### Localization
- Cập nhật các file `*.json` trong `Localization/CoreOracle` (ví dụ: `en.json`, `vi.json`):
  ```json
  {
    "Menu:Provinces": "Provinces/Cities",
    "Provinces": "Provinces/Cities",
    "NewProvince": "New Province/City",
    "EditProvince": "Edit Province/City",
    "ProvinceCode": "Code",
    "ProvinceName": "Name",
    "ProvinceCountry": "Country",
    "SelectCountry": "Select Country",
    "AreYouSureToDeleteProvince": "Are you sure you want to delete this province/city: {0}?",
    "Permission:Provinces": "Province/City Management",
    "Permission:Provinces.Create": "Create Province/City",
    "Permission:Provinces.Edit": "Edit Province/City",
    "Permission:Provinces.Delete": "Delete Province/City",
    "ProvinceCodeAlreadyExistsInCountry": "The province/city code '{0}' already exists in the selected country."
  }
  ```

### Error Codes
- Thêm vào `CoreOracleDomainErrorCodes.cs`:
  ```csharp
  public const string ProvinceCodeAlreadyExistsInCountry = "CoreOracle:00021";
  public const string CountryNotFoundForProvince = "CoreOracle:00022";
  ```

## 3. Tầng Application.Contracts (`Aqt.CoreOracle.Application.Contracts`)

### DTOs
- Tạo thư mục `Provinces/Dtos`
- Tạo file `ProvinceDto.cs`:
  ```csharp
  using System;
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;

  public class ProvinceDto : AuditedEntityDto<Guid>
  {
      public Guid CountryId { get; set; }
      public string CountryName { get; set; }
      public string Code { get; set; }
      public string Name { get; set; }
  }
  ```
- Tạo file `CreateUpdateProvinceDto.cs`:
  ```csharp
  using System;
  using System.ComponentModel.DataAnnotations;
  using Aqt.CoreOracle.Domain.Shared.Provinces;

  namespace Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;

  public class CreateUpdateProvinceDto
  {
      [Required]
      public Guid CountryId { get; set; }

      [Required]
      [StringLength(ProvinceConsts.MaxCodeLength)]
      public string Code { get; set; }

      [Required]
      [StringLength(ProvinceConsts.MaxNameLength)]
      public string Name { get; set; }
  }
  ```
- Tạo file `GetProvincesInput.cs`:
  ```csharp
  using System;
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;

  public class GetProvincesInput : PagedAndSortedResultRequestDto
  {
      public string? Filter { get; set; }
      public Guid? CountryId { get; set; }
  }
  ```
- Tạo file `ProvinceLookupDto.cs`:
  ```csharp
  using System;
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;

  public class ProvinceLookupDto : EntityDto<Guid>
  {
      public string Name { get; set; }
  }
  ```

### AppService Interface
- Tạo thư mục `Provinces`
- Tạo file `IProvinceAppService.cs`:
  ```csharp
  using System;
  using System.Threading.Tasks;
  using Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;
  using Volo.Abp.Application.Dtos;
  using Volo.Abp.Application.Services;

  namespace Aqt.CoreOracle.Application.Contracts.Provinces;

  public interface IProvinceAppService :
      ICrudAppService<
          ProvinceDto,
          Guid,
          GetProvincesInput,
          CreateUpdateProvinceDto>
  {
      Task<ListResultDto<ProvinceLookupDto>> GetLookupAsync(Guid countryId);
  }
  ```

### Permissions
- Cập nhật `CoreOraclePermissions.cs`:
  ```csharp
  public static class Provinces
  {
      public const string Default = GroupName + ".Provinces";
      public const string Create = Default + ".Create";
      public const string Edit = Default + ".Edit";
      public const string Delete = Default + ".Delete";
  }
  ```
- Cập nhật `CoreOraclePermissionDefinitionProvider.cs`:
  ```csharp
  var provincesPermission = myGroup.AddPermission(CoreOraclePermissions.Provinces.Default, L("Permission:Provinces"));
  provincesPermission.AddChild(CoreOraclePermissions.Provinces.Create, L("Permission:Provinces.Create"));
  provincesPermission.AddChild(CoreOraclePermissions.Provinces.Edit, L("Permission:Provinces.Edit"));
  provincesPermission.AddChild(CoreOraclePermissions.Provinces.Delete, L("Permission:Provinces.Delete"));
  ``` 

## 4. Tầng Application (`Aqt.CoreOracle.Application`)

### 4.1. Quy tắc tổ chức

#### Cấu trúc thư mục
- Tổ chức theo module: `Provinces/ProvinceAppService.cs`
- Mỗi module có một AutoMapper profile riêng: `Provinces/ProvinceApplicationAutoMapperProfile.cs`
- Không chứa DTO, Permission constant (đã định nghĩa trong Application.Contracts)

#### Namespace
- Format: `Aqt.CoreOracle.Application.Provinces`
- Không dùng lẫn với namespace của Contracts, Domain, HttpApi

#### AppService Implementation
- Kế thừa từ `CrudAppService` vì đây là CRUD đơn giản theo convention ABP
- Implement interface `IProvinceAppService` từ Application.Contracts
- Sử dụng `[Authorize]` attribute với các permission tương ứng
- Inject các dependency cần thiết (IProvinceRepository, ICountryRepository)

#### AutoMapper Profile
- Tạo `ProvinceApplicationAutoMapperProfile` riêng cho module Province
- Using `Volo.Abp.AutoMapper` cho `ProvinceApplicationAutoMapperProfile`
- Không cập nhật `CoreOracleApplicationAutoMapperProfile`
- Định nghĩa mapping giữa Entity và DTO:
  ```csharp
  CreateMap<Province, ProvinceDto>()
      .ForMember(dest => dest.CountryName,
          opt => opt.MapFrom(src => src.Country != null ? src.Country.Name : null));

  CreateMap<CreateUpdateProvinceDto, Province>()
      .IgnoreAuditedObjectProperties()
      .Ignore(x => x.Id)
      .Ignore(x => x.Country);

  CreateMap<ProvinceDto, CreateUpdateProvinceDto>();
  CreateMap<Province, ProvinceLookupDto>();
  ```

#### Using đúng namespace
```csharp
using Aqt.CoreOracle.Application.Contracts.Provinces;
using Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;
using Aqt.CoreOracle.Domain.Countries;
using Aqt.CoreOracle.Domain.Provinces;
using Aqt.CoreOracle.Domain.Provinces.Entities;
using Aqt.CoreOracle.Domain.Shared;
using Aqt.CoreOracle.Permissions;
```

### 4.2. Implementation

[Chi tiết implementation như đã có trong kế hoạch]

## 5. Tầng EntityFrameworkCore (`Aqt.CoreOracle.EntityFrameworkCore`)

### DbContext
- Cập nhật `CoreOracleDbContext.cs`:
  ```csharp
  public DbSet<Province> Provinces { get; set; }
  ```
- **Kiểm tra** trong phương thức `OnModelCreating` của `CoreOracleDbContext.cs`:
    - **Nếu đã có dòng:** `builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());` -> **Thì không cần làm gì thêm** vì `ProvinceConfiguration` sẽ được tự động áp dụng.
    - **Nếu chưa có dòng trên:**
        - **Cách 1 (Khuyến nghị):** Thêm dòng `builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());` vào cuối phương thức `OnModelCreating` để áp dụng tất cả cấu hình trong assembly.
        - **Cách 2:** Thêm dòng `builder.ApplyConfiguration(new ProvinceConfiguration());` để chỉ áp dụng cấu hình cho Province.
  *(Lưu ý: Cách 1 giúp code gọn gàng hơn khi có nhiều entity configuration)*

### Repository Implementation
- Tạo thư mục `Provinces`
- Tạo file `ProvinceRepository.cs`:
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Aqt.CoreOracle.Domain.Provinces;
  using Aqt.CoreOracle.Domain.Provinces.Entities;
  using Microsoft.EntityFrameworkCore;
  using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
  using Volo.Abp.EntityFrameworkCore;

  namespace Aqt.CoreOracle.EntityFrameworkCore.Provinces;

  public class ProvinceRepository :
      EfCoreRepository<CoreOracleDbContext, Province, Guid>,
      IProvinceRepository
  {
      public ProvinceRepository(IDbContextProvider<CoreOracleDbContext> dbContextProvider)
          : base(dbContextProvider)
      {
      }

      public async Task<Province?> FindByCodeAsync(string code, Guid countryId, CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          return await dbSet
              .FirstOrDefaultAsync(x => x.CountryId == countryId && x.Code == code,
                  GetCancellationToken(cancellationToken));
      }

      public async Task<bool> CodeExistsAsync(string code, Guid countryId, Guid? excludedId = null,
          CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          return await dbSet
              .AnyAsync(x => x.CountryId == countryId &&
                            x.Code == code &&
                            (!excludedId.HasValue || x.Id != excludedId.Value),
                  GetCancellationToken(cancellationToken));
      }

      public async Task<List<Province>> GetListByCountryAsync(
          Guid countryId,
          string? sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          var query = dbSet.Where(x => x.CountryId == countryId);

          query = query.OrderBy(sorting.IsNullOrWhiteSpace()
              ? nameof(Province.Name) + " asc"
              : sorting);

          return await query
              .Skip(skipCount)
              .Take(maxResultCount)
              .ToListAsync(GetCancellationToken(cancellationToken));
      }

      public async Task<long> GetCountByCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          return await dbSet
              .LongCountAsync(x => x.CountryId == countryId,
                  GetCancellationToken(cancellationToken));
      }

      public async Task<List<Province>> GetListWithCountryAsync(
          string? filterText = null,
          Guid? countryId = null,
          string? sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          var query = dbSet
              .Include(p => p.Country)
              .WhereIf(countryId.HasValue, p => p.CountryId == countryId.Value)
              .WhereIf(!filterText.IsNullOrWhiteSpace(),
                  p => p.Code.Contains(filterText!) || p.Name.Contains(filterText!));

          query = query.OrderBy(sorting.IsNullOrWhiteSpace()
              ? $"{nameof(Province.Country)}.{nameof(Domain.Countries.Entities.Country.Name)} asc, {nameof(Province.Name)} asc"
              : sorting);

          return await query
              .Skip(skipCount)
              .Take(maxResultCount)
              .ToListAsync(GetCancellationToken(cancellationToken));
      }

      public async Task<long> GetCountAsync(
          string? filterText = null,
          Guid? countryId = null,
          CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          var query = dbSet
              .WhereIf(countryId.HasValue, p => p.CountryId == countryId.Value)
              .WhereIf(!filterText.IsNullOrWhiteSpace(),
                  p => p.Code.Contains(filterText!) || p.Name.Contains(filterText!));

          return await query.LongCountAsync(GetCancellationToken(cancellationToken));
      }
  }
  ```

### Entity Configuration
- Tạo thư mục `EntityTypeConfigurations/Provinces`
- Tạo file `ProvinceConfiguration.cs`:
  ```csharp
  using Aqt.CoreOracle.Domain.Provinces.Entities;
  using Aqt.CoreOracle.Domain.Shared;
  using Aqt.CoreOracle.Domain.Shared.Provinces;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;
  using Volo.Abp.EntityFrameworkCore.Modeling;

  namespace Aqt.CoreOracle.EntityFrameworkCore.EntityTypeConfigurations.Provinces;

  public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
  {
      public void Configure(EntityTypeBuilder<Province> builder)
      {
          builder.ToTable(CoreOracleConsts.DbTablePrefix + "Provinces",
              CoreOracleConsts.DbSchema);
          builder.ConfigureByConvention();

          builder.HasKey(x => x.Id);

          builder.Property(x => x.Code)
              .IsRequired()
              .HasMaxLength(ProvinceConsts.MaxCodeLength)
              .HasColumnName(nameof(Province.Code));

          builder.Property(x => x.Name)
              .IsRequired()
              .HasMaxLength(ProvinceConsts.MaxNameLength)
              .HasColumnName(nameof(Province.Name));

          builder.Property(x => x.CountryId)
              .HasColumnName(nameof(Province.CountryId));

          builder.HasOne(x => x.Country)
              .WithMany()
              .HasForeignKey(x => x.CountryId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

          builder.HasIndex(x => new { x.CountryId, x.Code })
              .IsUnique();

          builder.HasIndex(x => x.Name);
      }
  }
  ```

## 6. Tầng Web (`Aqt.CoreOracle.Web`)

### 6.1. Khi nào sử dụng ViewModel và khi nào dùng DTO trực tiếp?

Việc quyết định sử dụng DTO trực tiếp hay tạo ViewModel riêng trong PageModel phụ thuộc vào độ phức tạp của giao diện và sự khác biệt giữa dữ liệu UI cần và DTO ở tầng Contracts.

**Khi nào nên dùng trực tiếp DTO:**
- Form đơn giản, ánh xạ 1:1 với DTO.
- Chỉ hiển thị dữ liệu DTO, không cần định dạng/tính toán phức tạp.
- Không cần các cấu hình UI đặc thù (như `[SelectItems]`, `[TextArea]`) đặt trên thuộc tính của Model.
- Ít logic hiển thị.

**Khi nào nên tạo ViewModel (trong tầng Web):**
- **Cần cấu hình UI trên Model:** Khi cần dùng các attribute như `[SelectItems]`, `[TextArea]`, `[DatePickerOptions]` để `<abp-form-content />` tự động render đúng control. ViewModel cho phép đặt các attribute này mà không vi phạm kiến trúc tầng (vì ViewModel và PageModel cùng ở tầng Web).
- **Dữ liệu hiển thị khác biệt:** Cần kết hợp dữ liệu từ nhiều nguồn, cần định dạng/tính toán riêng cho UI, cấu trúc dữ liệu View cần khác DTO.
- **Cần cấu trúc dữ liệu hỗ trợ UI:** ViewModel có thể chứa các `List<SelectListItem>` hoặc cấu trúc khác cho dropdown, radio list...
- **Form phức tạp:** Nhiều logic, validation phức tạp, hoặc các trường không ánh xạ 1:1 với DTO.

**Trường hợp chức năng Province này (Create Modal):**
- Để sử dụng `<abp-form-content />` và đồng thời muốn `CountryId` là dropdown (yêu cầu `[SelectItems]`), chúng ta **sẽ tạo ViewModel** (`CreateProvinceViewModel`) trong tầng Web để đặt attribute `[SelectItems]` mà không vi phạm kiến trúc.
- ViewModel này sẽ được bind với form, và PageModel sẽ chịu trách nhiệm map giữa ViewModel và DTO khi gọi AppService.

### 6.2. Menu
- Cập nhật `CoreOracleMenus.cs`:
  ```csharp
  public const string Provinces = Prefix + ".Provinces";
  ```
- Cập nhật `CoreOracleMenuContributor.cs`:
  ```csharp
  if (await context.IsGrantedAsync(CoreOraclePermissions.Provinces.Default))
  {
      var administration = context.Menu.GetAdministration();
      administration.AddItem(new ApplicationMenuItem(
          CoreOracleMenus.Provinces,
          l["Menu:Provinces"],
          "/Provinces"
      ).RequirePermissions(CoreOraclePermissions.Provinces.Default));
  }
  ```

### 6.3. Razor Pages
- Tạo thư mục `Pages/Provinces`
- Tạo file `Index.cshtml`:
  ```cshtml
  @page
  @using Aqt.CoreOracle.Permissions
  @using Microsoft.AspNetCore.Authorization
  @using Volo.Abp.AspNetCore.Mvc.UI.Layout
  @using Aqt.CoreOracle.Web.Pages.Provinces
  @using Aqt.CoreOracle.Localization
  @using Microsoft.Extensions.Localization
  @model IndexModel
  @inject IStringLocalizer<CoreOracleResource> L
  @inject IAuthorizationService AuthorizationService
  @inject IPageLayout PageLayout
  @{
      PageLayout.Content.Title = L["Provinces"].Value;
      PageLayout.Content.BreadCrumb.Add(L["Menu:Provinces"].Value);
  }

  @section scripts {
      <abp-script src="/Pages/Provinces/index.js" />
  }

  @section content_toolbar {
      @if (await AuthorizationService.IsGrantedAsync(CoreOraclePermissions.Provinces.Create))
      {
          <abp-button id="NewProvinceButton"
                      text="@L["NewProvince"].Value"
                      icon="plus"
                      button-type="Primary" />
      }
  }

  <abp-card>
      <abp-card-body>
          <abp-row class="mb-3">
              <abp-column size-md="_4">
                  <label class="form-label">@L["ProvinceCountry"]</label>
                  <abp-select asp-items="Model.CountryLookup" name="CountryIdFilter" label="" />
              </abp-column>
              <abp-column size-md="_4">
                  <label class="form-label">@L["Search"]</label>
                  <input type="text" id="SearchFilter" class="form-control" />
              </abp-column>
              <abp-column size-md="_4" class="text-end">
                   <abp-button id="SearchButton"
                             text="@L["Search"].Value"
                             icon="search"
                             button-type="Submit"
                             class="mt-4"/>
              </abp-column>
          </abp-row>

          <abp-table striped-rows="true" id="ProvincesTable"></abp-table>
      </abp-card-body>
  </abp-card>
  ```
- Tạo file `Index.cshtml.cs`:
  ```csharp
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Aqt.CoreOracle.Application.Contracts.Countries;
  using Aqt.CoreOracle.Web.Pages;
  using Microsoft.AspNetCore.Mvc.Rendering;

  namespace Aqt.CoreOracle.Web.Pages.Provinces;

  public class IndexModel : CoreOraclePageModel
  {
      public List<SelectListItem> CountryLookup { get; set; } = new();

      private readonly ICountryAppService _countryAppService;

      public IndexModel(ICountryAppService countryAppService)
      {
          _countryAppService = countryAppService;
      }

      public async Task OnGetAsync()
      {
          var countryLookupDto = await _countryAppService.GetLookupAsync();

          CountryLookup = countryLookupDto.Items
              .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
              .ToList();

          CountryLookup.Insert(0, new SelectListItem(L["All"], ""));
      }
  }
  ```

- **Tạo ViewModel cho Create Modal:**
  - Tạo thư mục `Pages/Provinces/ViewModels`
  - Tạo file `CreateProvinceViewModel.cs`:
    ```csharp
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Aqt.CoreOracle.Domain.Shared.Provinces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form; // Namespace cho SelectItems

    namespace Aqt.CoreOracle.Web.Pages.Provinces.ViewModels;

    public class CreateProvinceViewModel
    {
        [Required]
        [Display(Name = "ProvinceCountry")]
        [SelectItems(nameof(CreateModalModel.CountryLookup))] // Trỏ đến property trong PageModel
        public Guid CountryId { get; set; }

        [Required]
        [Display(Name = "ProvinceCode")]
        [StringLength(ProvinceConsts.MaxCodeLength)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "ProvinceName")]
        [StringLength(ProvinceConsts.MaxNameLength)]
        public string Name { get; set; }
    }
    ```

- Tạo file `CreateModal.cshtml`:
  ```cshtml
  @page "/Provinces/CreateModal"
  @using Microsoft.AspNetCore.Mvc.Localization
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
  @using Aqt.CoreOracle.Localization
  @using Aqt.CoreOracle.Web.Pages.Provinces
  @model CreateModalModel
  @inject IHtmlLocalizer<CoreOracleResource> L
  @{
      Layout = null;
  }
  <abp-dynamic-form abp-model="ProvinceViewModel" asp-page="/Provinces/CreateModal">
      <abp-modal>
          <abp-modal-header title="@L["NewProvince"].Value"></abp-modal-header>
          <abp-modal-body>
              <abp-form-content /> @* Sử dụng form content *@
          </abp-modal-body>
          <abp-modal-footer buttons="@(AbpModalButtons.Cancel | AbpModalButtons.Save)"></abp-modal-footer>
      </abp-modal>
  </abp-dynamic-form>
  ```
- Tạo file `CreateModal.cshtml.cs`:
  ```csharp
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Aqt.CoreOracle.Application.Contracts.Countries;
  using Aqt.CoreOracle.Application.Contracts.Provinces;
  using Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;
  using Aqt.CoreOracle.Web.Pages;
  using Aqt.CoreOracle.Web.Pages.Provinces.ViewModels; // Using ViewModel
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Rendering;

  namespace Aqt.CoreOracle.Web.Pages.Provinces;

  public class CreateModalModel : CoreOraclePageModel
  {
      [BindProperty]
      public CreateProvinceViewModel ProvinceViewModel { get; set; } // Sử dụng ViewModel

      public List<SelectListItem> CountryLookup { get; set; } = new();

      private readonly IProvinceAppService _provinceAppService;
      private readonly ICountryAppService _countryAppService;

      public CreateModalModel(
          IProvinceAppService provinceAppService,
          ICountryAppService countryAppService)
      {
          _provinceAppService = provinceAppService;
          _countryAppService = countryAppService;
      }

      public virtual async Task OnGetAsync()
      {
          ProvinceViewModel = new CreateProvinceViewModel();

          var countryLookupDto = await _countryAppService.GetLookupAsync();
          CountryLookup = countryLookupDto.Items
              .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
              .ToList();
      }

      public virtual async Task<IActionResult> OnPostAsync()
      {
          var provinceDto = ObjectMapper.Map<CreateProvinceViewModel, CreateUpdateProvinceDto>(ProvinceViewModel);
          await _provinceAppService.CreateAsync(provinceDto);
          return NoContent();
      }
  }
  ```
- Tạo file `EditModal.cshtml`:
  ```cshtml
  @page "/Provinces/EditModal"
  @using Microsoft.AspNetCore.Mvc.Localization
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
  @using Aqt.CoreOracle.Localization
  @using Aqt.CoreOracle.Web.Pages.Provinces
  @model EditModalModel
  @inject IHtmlLocalizer<CoreOracleResource> L
  @{
      Layout = null;
  }
  <abp-dynamic-form abp-model="Province" asp-page="/Provinces/EditModal">
      <abp-modal>
          <abp-modal-header title="@L["EditProvince"].Value"></abp-modal-header>
          <abp-modal-body>
              <input type="hidden" asp-for="Id" />
              <abp-select asp-for="Province.CountryId"
                         label="@L["ProvinceCountry"].Value"
                         asp-items="Model.CountryLookup"
                         required/>
              <abp-input asp-for="Province.Code"
                        label="@L["ProvinceCode"].Value"
                        required/>
              <abp-input asp-for="Province.Name"
                        label="@L["ProvinceName"].Value"
                        required/>
          </abp-modal-body>
          <abp-modal-footer buttons="@(AbpModalButtons.Cancel | AbpModalButtons.Save)"></abp-modal-footer>
      </abp-modal>
  </abp-dynamic-form>
  ```
- Tạo file `EditModal.cshtml.cs`:
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Aqt.CoreOracle.Application.Contracts.Countries;
  using Aqt.CoreOracle.Application.Contracts.Provinces;
  using Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;
  using Aqt.CoreOracle.Web.Pages;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Rendering;

  namespace Aqt.CoreOracle.Web.Pages.Provinces;

  public class EditModalModel : CoreOraclePageModel
  {
      [HiddenInput]
      [BindProperty(SupportsGet = true)]
      public Guid Id { get; set; }

      [BindProperty]
      public CreateUpdateProvinceDto Province { get; set; } = new();

      public List<SelectListItem> CountryLookup { get; set; } = new();

      private readonly IProvinceAppService _provinceAppService;
      private readonly ICountryAppService _countryAppService;

      public EditModalModel(
          IProvinceAppService provinceAppService,
          ICountryAppService countryAppService)
      {
          _provinceAppService = provinceAppService;
          _countryAppService = countryAppService;
      }

      public virtual async Task OnGetAsync()
      {
          var provinceDto = await _provinceAppService.GetAsync(Id);
          Province = ObjectMapper.Map<ProvinceDto, CreateUpdateProvinceDto>(provinceDto);

          var countryLookupDto = await _countryAppService.GetLookupAsync();
          CountryLookup = countryLookupDto.Items
              .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
              .ToList();
      }

      public virtual async Task<IActionResult> OnPostAsync()
      {
          await _provinceAppService.UpdateAsync(Id, Province);
          return NoContent();
      }
  }
  ```

### JavaScript
- Tạo thư mục `wwwroot/pages/provinces`
- Tạo file `wwwroot/pages/provinces/index.js`:
  ```javascript
  $(function () {
      var l = abp.localization.getResource('CoreOracle');
      var provinceService = aqt.coreOracle.application.provinces.province;
      var dataTable = null; // Define globally

      var createModal = new abp.ModalManager({
          viewUrl: abp.appPath + 'Provinces/CreateModal',
          modalClass: 'provinceCreateModal'
      });

      var editModal = new abp.ModalManager({
          viewUrl: abp.appPath + 'Provinces/EditModal',
          modalClass: 'provinceEditModal'
      });

      var getFilters = function() {
          return {
              filter: $('#SearchFilter').val(),
              countryId: $('#CountryIdFilter').val() || null
          };
      }

      function initializeDataTable() {
         if (dataTable) {
              dataTable.destroy(); // Destroy existing table if it exists
         }
         dataTable = $('#ProvincesTable').DataTable(
              abp.libs.datatables.normalizeConfiguration({
                  serverSide: true,
                  paging: true,
                  order: [[2, "asc"]], // Default sort by Code
                  searching: false,
                  scrollX: true,
                  ajax: abp.libs.datatables.createAjax(provinceService.getList, getFilters),
                  columnDefs: [
                      {
                          title: l('Actions'),
                          rowAction: {
                              items: [
                                  {
                                      text: l('Edit'),
                                      icon: "fa fa-pencil-alt",
                                      visible: abp.auth.isGranted('CoreOracle.Provinces.Edit'),
                                      action: function (data) {
                                          editModal.open({ id: data.record.id });
                                      }
                                  },
                                  {
                                      text: l('Delete'),
                                      icon: "fa fa-trash",
                                      visible: abp.auth.isGranted('CoreOracle.Provinces.Delete'),
                                      confirmMessage: function (data) {
                                          return l('AreYouSureToDeleteProvince',
                                              data.record.name || data.record.code);
                                      },
                                      action: function (data) {
                                          provinceService.delete(data.record.id)
                                              .then(function () {
                                                  abp.notify.success(l('SuccessfullyDeleted'));
                                                  dataTable.ajax.reload();
                                              });
                                      }
                                  }
                              ]
                          }
                      },
                      {
                          title: l('ProvinceCountry'),
                          data: "countryName",
                          orderable: true
                      },
                      {
                          title: l('ProvinceCode'),
                          data: "code",
                          orderable: true
                      },
                      {
                          title: l('ProvinceName'),
                          data: "name",
                          orderable: true
                      }
                  ]
              })
          );
      }
      
      initializeDataTable(); // Initialize on page load

      createModal.onResult(function () {
          dataTable.ajax.reload();
      });

      editModal.onResult(function () {
          dataTable.ajax.reload();
      });

      $('#NewProvinceButton').click(function (e) {
          e.preventDefault();
          createModal.open();
      });

      $('#SearchButton').click(function (e) {
          e.preventDefault();
          dataTable.ajax.reload();
      });

      // Reload table when filter inputs change or Enter is pressed
      $('#SearchFilter').on('keypress', function(e) {
          if(e.which === 13) { // Enter key
              dataTable.ajax.reload();
          }
      });

      $('#CountryIdFilter').change(function() {
          dataTable.ajax.reload();
      });
  });
  ```

## 7. Các bước triển khai và kiểm thử cuối cùng

### Các bước triển khai
1. Tạo các file và thư mục theo cấu trúc đã định nghĩa trong các tầng.
2. Implement code cho từng file theo thứ tự: Domain -> Domain.Shared -> Application.Contracts -> Application -> EntityFrameworkCore -> Web.
3. Chạy `dotnet build` để kiểm tra lỗi biên dịch.
4. Chạy lệnh `dotnet ef migrations add Added_Provinces_Table` trong dự án `.EntityFrameworkCore`.
5. Kiểm tra file migration vừa tạo.
6. Chạy dự án `.DbMigrator` để cập nhật schema database.
7. Chạy lệnh `abp generate-proxy -t js` để tạo/cập nhật JS proxies.
8. Chạy ứng dụng Web.

### Kiểm tra và xác nhận
1.  **Phân quyền:**
    *   Đăng nhập với user **có** quyền `CoreOracle.Provinces.Default`: Menu "Provinces/Cities" hiển thị.
    *   Đăng nhập với user **không** có quyền: Menu ẩn, truy cập URL `/Provinces` bị từ chối.
    *   Kiểm tra các nút "Thêm mới", "Sửa", "Xóa" ẩn/hiện đúng theo quyền.
2.  **Validation:**
    *   **Thêm mới/Sửa:**
        *   Để trống các trường bắt buộc -> Báo lỗi.
        *   Nhập quá độ dài cho phép -> Báo lỗi.
        *   Kiểm tra trùng mã trong cùng quốc gia.
3.  **CRUD:**
    *   **Thêm mới:** Chọn Quốc gia, nhập Mã, Tên hợp lệ -> Lưu thành công.
    *   **Xem danh sách:**
        *   Kiểm tra hiển thị, phân trang, sắp xếp.
        *   Kiểm tra bộ lọc Quốc gia và tìm kiếm.
    *   **Sửa:** Thay đổi thông tin -> Lưu thành công.
    *   **Xóa:** Xác nhận xóa -> Xóa thành công.
4.  **Giao diện:**
    *   Kiểm tra giao diện table, modal, các control.
    *   Kiểm tra responsive.
5.  **Localization:**
    *   Kiểm tra hiển thị đúng ngôn ngữ cho tất cả text.

### Lưu ý
- Đảm bảo chức năng quản lý Quốc gia đã hoàn thiện.
- Cần có `GetLookupAsync()` trong `ICountryAppService`.
- Chạy `abp generate-proxy -t js` sau khi thay đổi AppService.
- Kiểm tra kỹ các `using` statement.
- Xem xét thêm `[DisableAuditing]` cho các phương thức đọc.

- **Tạo AutoMapper Profile cho Province Web:**
  - Tạo file `Pages/Provinces/ProvinceWebAutoMapperProfile.cs`:
    ```csharp
    using Aqt.CoreOracle.Application.Contracts.Provinces.Dtos;
    using Aqt.CoreOracle.Web.Pages.Provinces.ViewModels;
    using AutoMapper;

    namespace Aqt.CoreOracle.Web.Pages.Provinces;

    public class ProvinceWebAutoMapperProfile : Profile
    {
        public ProvinceWebAutoMapperProfile()
        {
            // Mapping giữa ViewModel (tầng Web) và DTO (tầng Contracts)
            CreateMap<CreateProvinceViewModel, CreateUpdateProvinceDto>();

            // Thêm các mapping khác cho ViewModel của Province nếu cần
        }
    }
    ```
  - *Lưu ý: Profile này sẽ tự động được đăng ký nếu `CoreOracleWebModule` sử dụng `options.AddMaps<CoreOracleWebModule>()`.* 
  - *Mapping này không được đặt trong `CoreOracleWebAutoMapperProfile.cs` chung.* 