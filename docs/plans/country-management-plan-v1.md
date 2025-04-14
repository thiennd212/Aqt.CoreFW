# Kế hoạch triển khai chức năng quản lý danh mục Quốc gia

## 0. Phân tích nghiệp vụ

### 0.1. Mục tiêu
- Xây dựng chức năng cho phép người quản trị hệ thống (System Administrator) hoặc người dùng được cấp quyền quản lý danh mục các Quốc gia.
- Đảm bảo dữ liệu Quốc gia (mã, tên) là nhất quán và chính xác, làm cơ sở cho việc quản lý các danh mục phụ thuộc khác (như Tỉnh/Thành phố).

### 0.2. Đối tượng sử dụng
- Quản trị viên hệ thống hoặc người dùng được cấp quyền quản lý danh mục Quốc gia.

### 0.3. Yêu cầu chức năng chính (CRUD)
- **Xem danh sách (Read):** Hiển thị danh sách các Quốc gia đã có trong hệ thống dưới dạng bảng. Hỗ trợ tìm kiếm theo Mã hoặc Tên Quốc gia, phân trang và sắp xếp.
- **Thêm mới (Create):** Cho phép người dùng thêm một Quốc gia mới vào danh mục. Yêu cầu nhập Mã Quốc gia (Code) và Tên Quốc gia (Name).
- **Sửa (Update):** Cho phép người dùng chỉnh sửa thông tin (Mã, Tên) của một Quốc gia đã tồn tại.
- **Xóa (Delete):** Cho phép người dùng xóa một Quốc gia khỏi danh mục. Sử dụng cơ chế xóa mềm (Soft Delete). Cần có bước xác nhận trước khi xóa. *Lưu ý: Cần xem xét việc không cho phép xóa nếu Quốc gia đó còn Tỉnh/Thành phố liên kết (sẽ được kiểm tra ở tầng Application hoặc có ràng buộc DB).*

### 0.4. Yêu cầu dữ liệu
- **Mã Quốc gia (Code):**
    - Kiểu dữ liệu: Chuỗi (String).
    - Bắt buộc nhập.
    - Độ dài tối đa: 5 ký tự (theo `CountryConsts`).
    - Phải là duy nhất (unique) *trên toàn hệ thống*.
- **Tên Quốc gia (Name):**
    - Kiểu dữ liệu: Chuỗi (String).
    - Bắt buộc nhập.
    - Độ dài tối đa: 100 ký tự (theo `CountryConsts`).
- **Thông tin Audit:** Lưu trữ thông tin về người tạo, thời gian tạo, người sửa cuối cùng, thời gian sửa cuối cùng, trạng thái xóa mềm (IsDeleted). (Kế thừa từ `FullAuditedAggregateRoot`).

### 0.5. Yêu cầu giao diện người dùng (UI)
- **Màn hình danh sách:**
    - Ô tìm kiếm theo Mã hoặc Tên.
    - Bảng hiển thị các cột: Mã Quốc gia, Tên Quốc gia.
    - Nút "Thêm mới Quốc gia".
    - Các nút hành động (Sửa, Xóa) trên mỗi dòng của bảng.
    - Phân trang.
- **Modal Thêm mới/Sửa:**
    - Form nhập liệu cho Mã và Tên Quốc gia.
    - Các nút Lưu và Hủy.

### 0.6. Yêu cầu về phân quyền
- Cần định nghĩa các quyền riêng biệt cho việc xem danh sách, thêm, sửa, xóa Quốc gia.
- Chỉ những người dùng được gán quyền tương ứng mới có thể thực hiện các thao tác đó. Giao diện cần ẩn/hiện các nút chức năng dựa trên quyền của người dùng.

### 0.7. Quy tắc nghiệp vụ
- Mã Quốc gia không được trùng lặp. Hệ thống phải kiểm tra và thông báo lỗi nếu người dùng cố gắng tạo hoặc sửa thành một mã đã tồn tại.
- (Optional - Cần xem xét) Không cho phép xóa Quốc gia nếu vẫn còn Tỉnh/Thành phố tham chiếu đến.

## Tóm tắt Tiến độ Thực hiện Dự kiến

- [ ] **Bước 1: Tầng Domain (`Aqt.CoreFW.Domain`)**
- [ ] **Bước 2: Tầng Domain.Shared (`Aqt.CoreFW.Domain.Shared`)**
- [ ] **Bước 3: Tầng Application.Contracts (`Aqt.CoreFW.Application.Contracts`)**
- [ ] **Bước 4: Tầng Application (`Aqt.CoreFW.Application`)**
- [ ] **Bước 5: Tầng EntityFrameworkCore (`Aqt.CoreFW.EntityFrameworkCore`)**
- [ ] **Bước 6: Tầng Web (`Aqt.CoreFW.Web`)**
- [ ] **Bước 7: Các bước triển khai và kiểm thử cuối cùng** (Build, chạy migrations, tạo JS proxies, kiểm thử)

---

## 1. Tầng Domain (`Aqt.CoreFW.Domain`)

### Entity
- Tạo thư mục `Countries/Entities`
- Tạo file `Country.cs`:
  ```csharp
  using System;
  using Aqt.CoreFW.Domain.Shared.Countries; // Sẽ tạo ở bước sau
  using JetBrains.Annotations;
  using Volo.Abp;
  using Volo.Abp.Domain.Entities.Auditing;

  namespace Aqt.CoreFW.Countries.Entities;

  public class Country : FullAuditedAggregateRoot<Guid>
  {
      public string Code { get; private set; }
      public string Name { get; private set; } // Dùng private set để kiểm soát thay đổi

      protected Country() { /* For ORM */ }

      public Country(Guid id, [NotNull] string code, [NotNull] string name) : base(id)
      {
          SetCode(code);
          SetName(name);
      }

      // Sử dụng internal để chỉ cho phép thay đổi từ bên trong domain hoặc AppService thông qua phương thức UpdateAsync
      internal void SetCode([NotNull] string code)
      {
          Check.NotNullOrWhiteSpace(code, nameof(code));
          Check.Length(code, nameof(code), CountryConsts.MaxCodeLength);
          Code = code.ToUpperInvariant(); // Thống nhất viết hoa
      }

      internal void SetName([NotNull] string name)
      {
          Check.NotNullOrWhiteSpace(name, nameof(name));
          Check.Length(name, nameof(name), CountryConsts.MaxNameLength);
          Name = name;
      }

      // Phương thức ChangeName để thay đổi tên (nếu cần logic phức tạp hơn)
      public void ChangeName([NotNull] string newName)
      {
           SetName(newName);
           // Có thể thêm logic hoặc domain event ở đây nếu cần
      }
  }
  ```

### Repository Interface
- Tạo thư mục `Countries` (nếu chưa có ở root Domain)
- Tạo file `ICountryRepository.cs`:
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Countries.Entities;
  using Volo.Abp.Domain.Repositories;

  namespace Aqt.CoreFW.Countries.Repositories;

  public interface ICountryRepository : IRepository<Country, Guid>
  {
      // Tìm quốc gia theo mã (Code là unique)
      Task<Country?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

      // Kiểm tra xem mã quốc gia đã tồn tại chưa, loại trừ một ID (dùng khi cập nhật)
      Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default);

      // Lấy danh sách quốc gia có filter, sort, phân trang
      Task<List<Country>> GetListAsync(
          string? filterText = null,
          string? sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default);

      // Lấy tổng số quốc gia theo bộ lọc
      Task<long> GetCountAsync(
          string? filterText = null,
          CancellationToken cancellationToken = default);
  }
  ```

---

## 2. Tầng Domain.Shared (`Aqt.CoreFW.Domain.Shared`)

### Constants
- Tạo thư mục `Countries`
- Tạo file `CountryConsts.cs`:
  ```csharp
  namespace Aqt.CoreFW.Domain.Shared.Countries;
  public static class CountryConsts
  {
      public const int MaxCodeLength = 5;  // Ví dụ: VN, US, SG
      public const int MaxNameLength = 100;
  }
  ```

### Localization
- Cập nhật các file `*.json` trong `Localization/CoreFW` (ví dụ: `en.json`, `vi.json`):
  ```json
  {
    // ... các key khác
    "Menu:Countries": "Countries",
    "Countries": "Countries",
    "NewCountry": "New Country",
    "EditCountry": "Edit Country",
    "CountryCode": "Code",
    "CountryName": "Name",
    "AreYouSureToDeleteCountry": "Are you sure you want to delete this country: {0}?",
    "Permission:Countries": "Country Management",
    "Permission:Countries.Create": "Create Country",
    "Permission:Countries.Edit": "Edit Country",
    "Permission:Countries.Delete": "Delete Country",
    "CountryCodeAlreadyExists": "The country code '{0}' already exists.",
    "CountryInUse": "This country cannot be deleted because it is associated with existing provinces/cities." // Key lỗi nếu check ràng buộc xóa
    // ...Thêm các bản dịch tiếng Việt tương ứng vào vi.json
  }
  ```
  **File `vi.json` (ví dụ):**
  ```json
  {
    // ... các key khác
    "Menu:Countries": "Quốc gia",
    "Countries": "Quốc gia",
    "NewCountry": "Thêm mới Quốc gia",
    "EditCountry": "Sửa Quốc gia",
    "CountryCode": "Mã",
    "CountryName": "Tên",
    "AreYouSureToDeleteCountry": "Bạn có chắc muốn xóa quốc gia này: {0}?",
    "Permission:Countries": "Quản lý Quốc gia",
    "Permission:Countries.Create": "Tạo Quốc gia",
    "Permission:Countries.Edit": "Sửa Quốc gia",
    "Permission:Countries.Delete": "Xóa Quốc gia",
    "CountryCodeAlreadyExists": "Mã quốc gia '{0}' đã tồn tại.",
    "CountryInUse": "Không thể xóa quốc gia này vì đang được sử dụng bởi các Tỉnh/Thành phố."
  }
  ```

### Error Codes
- Thêm vào `CoreFWDomainErrorCodes.cs`:
  ```csharp
  public const string CountryCodeAlreadyExists = "CoreFW:00011"; // Ví dụ mã lỗi
  public const string CountryInUse = "CoreFW:00012";
  ```

---

## 3. Tầng Application.Contracts (`Aqt.CoreFW.Application.Contracts`)

### DTOs
- Tạo thư mục `Countries/Dtos`
- Tạo file `CountryDto.cs`:
  ```csharp
  using System;
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreFW.Countries.Dtos;

  public class CountryDto : AuditedEntityDto<Guid> // Kế thừa AuditedEntityDto để có thông tin audit
  {
      public string Code { get; set; }
      public string Name { get; set; }
  }
  ```
- Tạo file `CreateUpdateCountryDto.cs`:
  ```csharp
  using System.ComponentModel.DataAnnotations;
  using Aqt.CoreFW.Domain.Shared.Countries;

  namespace Aqt.CoreFW.Countries.Dtos;

  public class CreateUpdateCountryDto
  {
      [Required]
      [StringLength(CountryConsts.MaxCodeLength)]
      public string Code { get; set; }

      [Required]
      [StringLength(CountryConsts.MaxNameLength)]
      public string Name { get; set; }
  }
  ```
- Tạo file `GetCountriesInput.cs`:
  ```csharp
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreFW.Countries.Dtos;

  // Input cho phương thức GetListAsync
  public class GetCountriesInput : PagedAndSortedResultRequestDto
  {
      public string? Filter { get; set; } // Bộ lọc chung cho Code hoặc Name
  }
  ```
- Tạo file `CountryLookupDto.cs`:
  ```csharp
  using System;
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreFW.Countries.Dtos;

  // DTO dùng cho việc lookup (ví dụ: đổ vào dropdown)
  public class CountryLookupDto : EntityDto<Guid>
  {
      public string Name { get; set; }
      // Có thể thêm Code nếu cần hiển thị cả mã
      // public string Code { get; set; }
  }
  ```

### AppService Interface
- Tạo thư mục `Countries`
- Tạo file `ICountryAppService.cs`:
  ```csharp
  using System;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Countries.Dtos;
  using Volo.Abp.Application.Dtos;
  using Volo.Abp.Application.Services;

  namespace Aqt.CoreFW.Countries;

  public interface ICountryAppService :
      ICrudAppService< // Sử dụng ICrudAppService vì các thao tác là CRUD chuẩn
          CountryDto,               // DTO để hiển thị
          Guid,                     // Kiểu khóa chính
          GetCountriesInput,        // DTO Input cho GetList
          CreateUpdateCountryDto>   // DTO Input cho Create/Update
  {
       // Phương thức lấy danh sách lookup (đã có trong Province plan, có thể dùng chung nếu đặt ở vị trí phù hợp hoặc tạo mới ở đây)
       Task<ListResultDto<CountryLookupDto>> GetLookupAsync();
  }
  ```

### Permissions
- Cập nhật `CoreFWPermissions.cs`:
  ```csharp
  // Bên trong class CoreFWPermissions
  public static class Countries // Tạo class con cho module Countries
  {
      public const string Default = GroupName + ".Countries"; // Quyền xem danh sách
      public const string Create = Default + ".Create";      // Quyền tạo mới
      public const string Edit = Default + ".Edit";        // Quyền sửa
      public const string Delete = Default + ".Delete";      // Quyền xóa
  }
  ```
- Cập nhật `CoreFWPermissionDefinitionProvider.cs`:
  ```csharp
  // Bên trong phương thức Define
  var countriesPermission = myGroup.AddPermission(CoreFWPermissions.Countries.Default, L("Permission:Countries"));
  countriesPermission.AddChild(CoreFWPermissions.Countries.Create, L("Permission:Countries.Create"));
  countriesPermission.AddChild(CoreFWPermissions.Countries.Edit, L("Permission:Countries.Edit"));
  countriesPermission.AddChild(CoreFWPermissions.Countries.Delete, L("Permission:Countries.Delete"));
  ```

---

## 4. Tầng Application (`Aqt.CoreFW.Application`)

### AutoMapper Profile
- Tạo thư mục `Countries`
- Tạo file `CountryApplicationAutoMapperProfile.cs`:
  ```csharp
  using Aqt.CoreFW.Countries.Dtos;
  using Aqt.CoreFW.Countries.Entities;
  using AutoMapper;
  using Volo.Abp.AutoMapper; // Cần using này nếu dùng IgnoreAuditedObjectProperties

  namespace Aqt.CoreFW.Countries;

  public class CountryApplicationAutoMapperProfile : Profile
  {
      public CountryApplicationAutoMapperProfile()
      {
          CreateMap<Country, CountryDto>();
          CreateMap<Country, CountryLookupDto>();

          CreateMap<CreateUpdateCountryDto, Country>()
              .IgnoreAuditedObjectProperties() // Bỏ qua các thuộc tính audit khi map từ DTO sang Entity
              .Ignore(x => x.Id) // Không map Id khi tạo mới
              .Ignore(x => x.ExtraProperties)
              .Ignore(x => x.ConcurrencyStamp);


          // Mapping ngược lại từ Dto -> CreateUpdateDto (dùng cho Edit)
          CreateMap<CountryDto, CreateUpdateCountryDto>();
      }
  }
  ```
- Đăng ký Profile này trong `CoreFWApplicationModule.cs`:
  ```csharp
  // Trong phương thức ConfigureServices
  context.Services.AddAutoMapperObjectMapper<CoreFWApplicationModule>();
  Configure<AbpAutoMapperOptions>(options =>
  {
      // Đảm bảo rằng AddMaps được gọi cho assembly này
      options.AddMaps<CoreFWApplicationModule>(validate: true); // validate: true để kiểm tra cấu hình mapping
      // Hoặc nếu bạn muốn thêm từng profile riêng lẻ:
      // options.AddProfile<CountryApplicationAutoMapperProfile>(validate: true);
  });
  ```

### AppService Implementation
- Tạo thư mục `Countries`
- Tạo file `CountryAppService.cs`:
  ```csharp
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Countries;
  using Aqt.CoreFW.Countries.Dtos;
  using Aqt.CoreFW.Countries;
  using Aqt.CoreFW.Countries.Entities;
  using Aqt.CoreFW.Domain.Shared; // Namespace cho CoreFWDomainErrorCodes
  using Aqt.CoreFW.Permissions;
  using Microsoft.AspNetCore.Authorization;
  using Volo.Abp;
  using Volo.Abp.Application.Dtos;
  using Volo.Abp.Application.Services;
  using Volo.Abp.Domain.Repositories;

  namespace Aqt.CoreFW.Countries;

  [Authorize(CoreFWPermissions.Countries.Default)] // Quyền mặc định để truy cập service
  public class CountryAppService :
      CrudAppService<                 // Kế thừa CrudAppService chuẩn của ABP
          Country,                   // Entity
          CountryDto,                // DTO hiển thị
          Guid,                      // Khóa chính
          GetCountriesInput,         // DTO Input cho GetList
          CreateUpdateCountryDto>,    // DTO Input cho Create/Update
      ICountryAppService             // Implement interface đã định nghĩa
  {
      private readonly ICountryRepository _countryRepository;
      // Inject thêm IProvinceRepository nếu cần kiểm tra ràng buộc xóa
      // private readonly IProvinceRepository _provinceRepository;

      public CountryAppService(ICountryRepository repository /*, IProvinceRepository provinceRepository*/)
          : base(repository)
      {
          _countryRepository = repository;
          // _provinceRepository = provinceRepository;

          // Set các policy name cho các quyền CRUD (ABP sẽ tự động kiểm tra)
          GetPolicyName = CoreFWPermissions.Countries.Default;
          GetListPolicyName = CoreFWPermissions.Countries.Default;
          CreatePolicyName = CoreFWPermissions.Countries.Create;
          UpdatePolicyName = CoreFWPermissions.Countries.Edit;
          DeletePolicyName = CoreFWPermissions.Countries.Delete;
      }

      // Override GetListAsync để xử lý Filter
      protected override async Task<IQueryable<Country>> CreateFilteredQueryAsync(GetCountriesInput input)
      {
          var query = await ReadOnlyRepository.GetQueryableAsync(); // Lấy IQueryable từ repository

          query = query
              .WhereIf(!input.Filter.IsNullOrWhiteSpace(), // Áp dụng Filter nếu có
                  c => c.Code.Contains(input.Filter!) ||
                       c.Name.Contains(input.Filter!));

          return query;
      }

      // Override CreateAsync để kiểm tra trùng Code
      [Authorize(CoreFWPermissions.Countries.Create)]
      public override async Task<CountryDto> CreateAsync(CreateUpdateCountryDto input)
      {
          await CheckCodeDuplicationAsync(input.Code);

          var country = ObjectMapper.Map<CreateUpdateCountryDto, Country>(input);

          // GuidGenerator được inject sẵn trong ApplicationService base class
          country = await _countryRepository.InsertAsync(country, autoSave: true);

          return ObjectMapper.Map<Country, CountryDto>(country);
      }

      // Override UpdateAsync để kiểm tra trùng Code (loại trừ chính nó)
      [Authorize(CoreFWPermissions.Countries.Edit)]
      public override async Task<CountryDto> UpdateAsync(Guid id, CreateUpdateCountryDto input)
      {
          var country = await _countryRepository.GetAsync(id);

          await CheckCodeDuplicationAsync(input.Code, id);

          // Map dữ liệu từ input DTO vào entity đã lấy ra
          ObjectMapper.Map(input, country);

          await _countryRepository.UpdateAsync(country, autoSave: true);

          return ObjectMapper.Map<Country, CountryDto>(country);
      }

       // Override DeleteAsync để kiểm tra ràng buộc (nếu cần)
       [Authorize(CoreFWPermissions.Countries.Delete)]
       public override async Task DeleteAsync(Guid id)
       {
           // Optional: Kiểm tra xem có Province nào thuộc Country này không
           // var provincesExist = await _provinceRepository.AnyAsync(p => p.CountryId == id);
           // if (provincesExist)
           // {
           //     throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryInUse]);
           // }

           await _countryRepository.DeleteAsync(id, autoSave: true); // Hoặc dùng base.DeleteAsync(id);
       }

      // Phương thức helper kiểm tra trùng Code
      private async Task CheckCodeDuplicationAsync(string code, Guid? excludedId = null)
      {
          if (await _countryRepository.CodeExistsAsync(code, excludedId))
          {
              throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryCodeAlreadyExists, code]);
          }
      }

       // Implement phương thức GetLookupAsync
       // Không cần Authorize vì thường dùng cho các chức năng khác (như Province)
       public virtual async Task<ListResultDto<CountryLookupDto>> GetLookupAsync()
       {
           var countries = await _countryRepository.GetListAsync(sorting: nameof(Country.Name)); // Sắp xếp theo tên
           return new ListResultDto<CountryLookupDto>(
               ObjectMapper.Map<List<Country>, List<CountryLookupDto>>(countries)
           );
       }
  }
  ```

---

## 5. Tầng EntityFrameworkCore (`Aqt.CoreFW.EntityFrameworkCore`)

### DbContext
- Cập nhật `CoreFWDbContext.cs`:
  ```csharp
  using Aqt.CoreFW.Countries.Entities; // Thêm using
  //...
  public DbSet<Country> Countries { get; set; } // Thêm DbSet
  ```

### Entity Configuration
- Tạo thư mục `EntityTypeConfigurations/Countries`
- Tạo file `CountryConfiguration.cs`:
  ```csharp
  using Aqt.CoreFW.Countries.Entities;
  using Aqt.CoreFW.Domain.Shared; // Namespace cho CoreFWConsts
  using Aqt.CoreFW.Domain.Shared.Countries; // Namespace cho CountryConsts
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;
  using Volo.Abp.EntityFrameworkCore.Modeling;

  namespace Aqt.CoreFW.EntityFrameworkCore.EntityTypeConfigurations.Countries;

  public class CountryConfiguration : IEntityTypeConfiguration<Country>
  {
      public void Configure(EntityTypeBuilder<Country> builder)
      {
          // Cấu hình tên bảng và schema
          builder.ToTable(CoreFWConsts.DbTablePrefix + "Countries",
              CoreFWConsts.DbSchema);

          // Áp dụng các cấu hình mặc định của ABP (như soft delete, audit properties)
          builder.ConfigureByConvention();

          // Khóa chính
          builder.HasKey(x => x.Id);

          // Cấu hình thuộc tính Code
          builder.Property(x => x.Code)
              .IsRequired()                       // Bắt buộc
              .HasMaxLength(CountryConsts.MaxCodeLength) // Độ dài tối đa
              .HasColumnName(nameof(Country.Code));  // Tên cột trong DB

          // Cấu hình thuộc tính Name
          builder.Property(x => x.Name)
              .IsRequired()
              .HasMaxLength(CountryConsts.MaxNameLength)
              .HasColumnName(nameof(Country.Name));

          // Tạo index cho Code để đảm bảo unique và tăng tốc truy vấn
          builder.HasIndex(x => x.Code).IsUnique();

          // Tạo index cho Name để tăng tốc tìm kiếm/sắp xếp
          builder.HasIndex(x => x.Name);
      }
  }
  ```
- Đảm bảo `CountryConfiguration` được áp dụng trong `CoreFWDbContext.cs`. **Kiểm tra** phương thức `OnModelCreating`:
    - **Nếu đã có dòng:** `builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());` -> **Không cần làm gì thêm.**
    - **Nếu chưa có:** Nên thêm dòng trên vào cuối phương thức `OnModelCreating`.

### Repository Implementation
- Tạo thư mục `Countries`
- Tạo file `CountryRepository.cs`:
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Dynamic.Core; // Cần cho OrderBy với chuỗi sorting
  using System.Threading;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Countries;
  using Aqt.CoreFW.Countries.Entities;
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
          return await dbSet.FirstOrDefaultAsync(x => x.Code == code, GetCancellationToken(cancellationToken));
      }

      public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
      {
          var dbSet = await GetDbSetAsync();
          return await dbSet.AnyAsync(x => x.Code == code &&
                                          (!excludedId.HasValue || x.Id != excludedId.Value), // Loại trừ ID nếu có
                                      GetCancellationToken(cancellationToken));
      }

       // Override GetListAsync để thêm logic filter và sorting
       public override async Task<List<Country>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
       {
           // Triển khai đơn giản, bạn có thể dùng GetListAsync với tham số filter bên dưới
           return await base.GetListAsync(includeDetails, cancellationToken);
       }

       // Implement GetListAsync với các tham số filter, sort, paging
       public async Task<List<Country>> GetListAsync(
           string? filterText = null,
           string? sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default)
       {
           var dbSet = await GetDbSetAsync();
           var query = dbSet
               .WhereIf(!filterText.IsNullOrWhiteSpace(), // Áp dụng filter
                   c => c.Code.Contains(filterText!) ||
                        c.Name.Contains(filterText!));

           // Áp dụng sorting, mặc định sort theo Name nếu không có input sorting
           query = query.OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Country.Name) + " asc" : sorting);

           return await query
               .Skip(skipCount)
               .Take(maxResultCount)
               .ToListAsync(GetCancellationToken(cancellationToken));
       }

       // Implement GetCountAsync với filter
       public async Task<long> GetCountAsync(
           string? filterText = null,
           CancellationToken cancellationToken = default)
       {
           var dbSet = await GetDbSetAsync();
           var query = dbSet
               .WhereIf(!filterText.IsNullOrWhiteSpace(),
                   c => c.Code.Contains(filterText!) ||
                        c.Name.Contains(filterText!));

           return await query.LongCountAsync(GetCancellationToken(cancellationToken));
       }

       // Override GetQueryableAsync nếu cần tùy chỉnh sâu hơn
       // public override async Task<IQueryable<Country>> GetQueryableAsync()
       // {
       //     var query = await base.GetQueryableAsync();
       //     // Thêm Include hoặc các tùy chỉnh khác nếu cần
       //     return query;
       // }
  }
  ```

--- 

## 6. Tầng Web (`Aqt.CoreFW.Web`)

### AutoMapper Profile (Web)
- Tạo thư mục `Pages/Countries` nếu chưa có.
- Tạo file `Pages/Countries/CountryWebAutoMapperProfile.cs`:
  ```csharp
  using Aqt.CoreFW.Countries.Dtos;
  using AutoMapper;

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class CountryWebAutoMapperProfile : Profile
  {
      public CountryWebAutoMapperProfile()
      {
          // Mapping cần thiết cho Edit Modal: từ DTO sang CreateUpdate DTO
          CreateMap<CountryDto, CreateUpdateCountryDto>();

          // Mapping từ ViewModel sang DTO nếu bạn quyết định dùng ViewModel sau này
          // CreateMap<CreateCountryViewModel, CreateUpdateCountryDto>();
          // CreateMap<EditCountryViewModel, CreateUpdateCountryDto>();
          // CreateMap<CountryDto, EditCountryViewModel>();
      }
  }
  ```
- Đảm bảo Profile này được đăng ký trong `CoreFWWebModule.cs`, tương tự như cách đăng ký Application profile. Thường thì `options.AddMaps<CoreFWWebModule>()` sẽ tự động quét và đăng ký các profile trong assembly Web.

### Menu
- Cập nhật `CoreFWMenus.cs`:
  ```csharp
  // Thêm hằng số cho menu Countries
  public const string Countries = Prefix + ".Countries";
  ```
- Cập nhật `CoreFWMenuContributor.cs` (trong phương thức `ConfigureMainMenuAsync`):
  ```csharp
  using Aqt.CoreFW.Permissions; // Thêm using nếu chưa có
  //...
  // Tìm menu Administration hoặc menu cha bạn muốn thêm vào
  var administration = context.Menu.GetAdministration(); // Hoặc một menu gốc khác

  // Thêm menu Countries nếu user có quyền
  if (await context.IsGrantedAsync(CoreFWPermissions.Countries.Default))
  {
      administration.AddItem(new ApplicationMenuItem(
          CoreFWMenus.Countries,
          l["Menu:Countries"], // Sử dụng localization key
          "/Countries", // Đường dẫn tới trang Index
          icon: "fa fa-globe" // Icon ví dụ
      ).RequirePermissions(CoreFWPermissions.Countries.Default)); // Yêu cầu quyền để thấy menu
  }

  // Sắp xếp vị trí menu nếu cần
  // administration.SetSubItemOrder(CoreFWMenus.Countries, 4);
  ```

### Razor Pages
- Tạo thư mục `Pages/Countries`
- Tạo file `Index.cshtml`:
  ```cshtml
  @page
  @using Aqt.CoreFW.Permissions
  @using Microsoft.AspNetCore.Authorization
  @using Volo.Abp.AspNetCore.Mvc.UI.Layout
  @using Aqt.CoreFW.Web.Pages.Countries
  @using Aqt.CoreFW.Localization
  @using Microsoft.Extensions.Localization
  @model IndexModel
  @inject IStringLocalizer<CoreFWResource> L
  @inject IAuthorizationService AuthorizationService
  @inject IPageLayout PageLayout
  @{
      PageLayout.Content.Title = L["Countries"].Value;
      PageLayout.Content.BreadCrumb.Add(L["Menu:Countries"].Value);
      PageLayout.Content.MenuItemName = CoreFWMenus.Countries; // Đặt tên menu item đang active
  }

  @section scripts {
      <abp-script src="/Pages/Countries/index.js" /> @* JS riêng cho trang này *@
  }

  @section content_toolbar {
      @if (await AuthorizationService.IsGrantedAsync(CoreFWPermissions.Countries.Create))
      {
          <abp-button id="NewCountryButton"
                      text="@L["NewCountry"].Value"
                      icon="plus"
                      button-type="Primary" /> @* Nút thêm mới *@
      }
  }

  <abp-card>
      <abp-card-body>
          @* Form tìm kiếm *@
          <abp-form id="SearchForm">
              <abp-row class="mb-3">
                   <abp-column size-md="_9">
                        <abp-input asp-for="Filter" label="@L["Search"].Value" />
                   </abp-column>
                   <abp-column size-md="_3" class="text-end">
                        @* Nút tìm kiếm không cần id vì JS sẽ bắt sự kiện submit form *@
                        <abp-button button-type="Primary" type="submit" icon="search" text="@L["Search"].Value" class="mt-4"/>
                   </abp-column>
              </abp-row>
          </abp-form>

          @* Bảng hiển thị dữ liệu *@
          <abp-table striped-rows="true" id="CountriesTable" class="nowrap"></abp-table>
      </abp-card-body>
  </abp-card>
  ```
- Tạo file `Index.cshtml.cs`:
  ```csharp
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc; // Cần cho [BindProperty]

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class IndexModel : CoreFWPageModel // Kế thừa PageModel base của dự án
  {
      // Property để bind với ô tìm kiếm trên form
      [BindProperty(SupportsGet = true)] // SupportsGet = true để nhận giá trị filter từ query string nếu có
      public string? Filter { get; set; }

      public void OnGet()
      {
          // Không cần logic đặc biệt khi GET trang Index,
          // dữ liệu sẽ được load bằng DataTables AJAX
      }
  }
  ```
- Tạo file `CreateModal.cshtml`:
  ```cshtml
  @page
  @using Microsoft.AspNetCore.Mvc.Localization
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
  @using Aqt.CoreFW.Localization
  @using Aqt.CoreFW.Web.Pages.Countries
  @model CreateModalModel
  @inject IHtmlLocalizer<CoreFWResource> L
  @{
      Layout = null; // Modal không cần layout
  }
  @* Sử dụng abp-dynamic-form để render form từ DTO *@
  <abp-dynamic-form abp-model="Country" asp-page="/Countries/CreateModal">
      <abp-modal size="Large"> @* Kích thước modal *@
          <abp-modal-header title="@L["NewCountry"].Value"></abp-modal-header>
          <abp-modal-body>
              @* Tự động render các input dựa trên thuộc tính của Country DTO *@
              <abp-form-content />
          </abp-modal-body>
          <abp-modal-footer buttons="@(AbpModalButtons.Cancel | AbpModalButtons.Save)"></abp-modal-footer>
      </abp-modal>
  </abp-dynamic-form>
  ```
- Tạo file `CreateModal.cshtml.cs`:
  ```csharp
  using System.Threading.Tasks;
  using Aqt.CoreFW.Countries;
  using Aqt.CoreFW.Countries.Dtos;
  using Microsoft.AspNetCore.Mvc;

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class CreateModalModel : CoreFWPageModel
  {
      // Bind DTO trực tiếp với form vì không cần ViewModel phức tạp
      [BindProperty]
      public CreateUpdateCountryDto Country { get; set; }

      private readonly ICountryAppService _countryAppService;

      public CreateModalModel(ICountryAppService countryAppService)
      {
          _countryAppService = countryAppService;
      }

      public void OnGet()
      {
          // Khởi tạo DTO rỗng khi mở modal
          Country = new CreateUpdateCountryDto();
      }

      public async Task<IActionResult> OnPostAsync()
      {
          // Gọi AppService để tạo mới
          await _countryAppService.CreateAsync(Country);
          return NoContent(); // Trả về 204 No Content để đóng modal và reload table
      }
  }
  ```
- Tạo file `EditModal.cshtml`:
  ```cshtml
  @page
  @using Microsoft.AspNetCore.Mvc.Localization
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
  @using Aqt.CoreFW.Localization
  @using Aqt.CoreFW.Web.Pages.Countries
  @model EditModalModel
  @inject IHtmlLocalizer<CoreFWResource> L
  @{
      Layout = null;
  }
  <abp-dynamic-form abp-model="Country" asp-page="/Countries/EditModal">
       <abp-modal size="Large">
           <abp-modal-header title="@L["EditCountry"].Value"></abp-modal-header>
           <abp-modal-body>
               @* Input ẩn để chứa Id *@
               <input type="hidden" asp-for="Id" />
               <abp-form-content /> @* Render các input còn lại *@
           </abp-modal-body>
           <abp-modal-footer buttons="@(AbpModalButtons.Cancel | AbpModalButtons.Save)"></abp-modal-footer>
       </abp-modal>
  </abp-dynamic-form>
  ```
- Tạo file `EditModal.cshtml.cs`:
  ```csharp
  using System;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Countries;
  using Aqt.CoreFW.Countries.Dtos;
  using Microsoft.AspNetCore.Mvc;

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class EditModalModel : CoreFWPageModel
  {
      // Id lấy từ URL
      [HiddenInput]
      [BindProperty(SupportsGet = true)]
      public Guid Id { get; set; }

      // DTO để bind với form
      [BindProperty]
      public CreateUpdateCountryDto Country { get; set; }

      private readonly ICountryAppService _countryAppService;

      public EditModalModel(ICountryAppService countryAppService)
      {
          _countryAppService = countryAppService;
      }

      public async Task OnGetAsync()
      {
          // Lấy thông tin Country hiện tại
          var countryDto = await _countryAppService.GetAsync(Id);
          // Map sang CreateUpdateCountryDto để hiển thị trên form edit
          Country = ObjectMapper.Map<CountryDto, CreateUpdateCountryDto>(countryDto);
      }

      public async Task<IActionResult> OnPostAsync()
      {
          // Gọi AppService để cập nhật
          await _countryAppService.UpdateAsync(Id, Country);
          return NoContent();
      }
  }
  ```

### JavaScript
- Tạo thư mục `wwwroot/pages/countries`
- Tạo file `wwwroot/pages/countries/index.js`:
  ```javascript
  $(function () {
      // Lấy resource localization
      var l = abp.localization.getResource('CoreFW');

      // Lấy service proxy
      var countryService = aqt.coreFW.application.countries.country; // Namespace proxy

      var dataTable = null; // Khai báo biến DataTable

      // Khởi tạo ModalManager cho Create và Edit
      var createModal = new abp.ModalManager({
          viewUrl: abp.appPath + 'Countries/CreateModal', // Đường dẫn đến CreateModal.cshtml
          modalClass: 'countryCreateModal' // Class CSS tùy chọn cho modal
      });

      var editModal = new abp.ModalManager({
          viewUrl: abp.appPath + 'Countries/EditModal', // Đường dẫn đến EditModal.cshtml
          modalClass: 'countryEditModal'
      });

      // Hàm lấy giá trị filter từ input
      var getFilters = function() {
          return {
              filter: $("#SearchForm input[name='Filter']").val() // Lấy giá trị từ input có name='Filter' trong form SearchForm
          };
      }

      // Hàm khởi tạo hoặc khởi tạo lại DataTable
      function initializeDataTable() {
          if (dataTable) {
              dataTable.destroy(); // Hủy table cũ nếu đã tồn tại
          }
          dataTable = $('#CountriesTable').DataTable(
              abp.libs.datatables.normalizeConfiguration({
                  serverSide: true,     // Sử dụng server-side processing
                  paging: true,         // Bật phân trang
                  order: [[1, "asc"]],  // Sắp xếp mặc định theo cột thứ 2 (Code) tăng dần
                  searching: false,     // Tắt searching mặc định của DataTables (vì đã có filter riêng)
                  scrollX: true,        // Bật scroll ngang nếu cần
                  ajax: abp.libs.datatables.createAjax(countryService.getList, getFilters), // Cấu hình AJAX gọi AppService
                  columnDefs: [         // Định nghĩa các cột
                      {
                          title: l('Actions'), // Cột hành động
                          rowAction: {
                              items: [
                                  {
                                      text: l('Edit'), // Nút sửa
                                      icon: "fa fa-pencil-alt", // Icon sửa
                                      visible: abp.auth.isGranted('CoreFW.Countries.Edit'), // Chỉ hiển thị nếu có quyền sửa
                                      action: function (data) {
                                          // Mở modal sửa với ID của dòng
                                          editModal.open({ id: data.record.id });
                                      }
                                  },
                                  {
                                      text: l('Delete'), // Nút xóa
                                      icon: "fa fa-trash", // Icon xóa
                                      visible: abp.auth.isGranted('CoreFW.Countries.Delete'), // Chỉ hiển thị nếu có quyền xóa
                                      // Hiển thị thông báo xác nhận trước khi xóa
                                      confirmMessage: function (data) {
                                          return l('AreYouSureToDeleteCountry', data.record.name || data.record.code);
                                      },
                                      action: function (data) {
                                          // Gọi service xóa
                                          countryService.delete(data.record.id)
                                              .then(function () {
                                                  abp.notify.success(l('SuccessfullyDeleted')); // Thông báo thành công
                                                  dataTable.ajax.reload(); // Load lại bảng
                                              });
                                      }
                                  }
                              ]
                          }
                      },
                      {
                          title: l('CountryCode'), // Cột Mã Quốc gia
                          data: "code",
                          orderable: true // Cho phép sắp xếp
                      },
                      {
                          title: l('CountryName'), // Cột Tên Quốc gia
                          data: "name",
                          orderable: true
                      }
                      // Thêm các cột khác nếu cần (ví dụ: ngày tạo...)
                  ]
              })
          );
      }

      initializeDataTable(); // Khởi tạo DataTable lần đầu khi trang load

      // Xử lý sự kiện khi modal Create đóng và có kết quả (đã save thành công)
      createModal.onResult(function () {
          dataTable.ajax.reload(); // Load lại data table
      });

      // Xử lý sự kiện khi modal Edit đóng và có kết quả
      editModal.onResult(function () {
          dataTable.ajax.reload();
      });

      // Xử lý sự kiện click nút "New Country"
      $('#NewCountryButton').click(function (e) {
          e.preventDefault();
          createModal.open(); // Mở modal tạo mới
      });

      // Xử lý sự kiện submit form tìm kiếm
      $('#SearchForm').submit(function (e) {
          e.preventDefault();
          dataTable.ajax.reload(); // Load lại data table với filter mới
      });

  });
  ```

---

## 7. Các bước triển khai và kiểm thử cuối cùng

### Các bước triển khai
1.  Tạo các file và thư mục theo cấu trúc đã định nghĩa trong các tầng Domain, Domain.Shared, Application.Contracts, Application, EntityFrameworkCore, Web.
2.  Implement code cho từng file theo đúng thứ tự và các quy tắc đã nêu.
3.  Chạy lệnh `dotnet build Aqt.CoreFW.sln` để kiểm tra lỗi biên dịch trên toàn bộ solution.
4.  Trong thư mục `src/Aqt.CoreFW.EntityFrameworkCore`, chạy lệnh:
    `dotnet ef migrations add Added_Countries_Table -c CoreFWDbContext -o EntityFrameworkCore/Migrations`
    *(Lệnh này tạo migration mới cho bảng Countries, sử dụng DbContext `CoreFWDbContext` và lưu vào thư mục `EntityFrameworkCore/Migrations`)*
5.  Kiểm tra kỹ nội dung file migration vừa được tạo ra.
6.  Chạy dự án `Aqt.CoreFW.DbMigrator` để áp dụng migration vào cơ sở dữ liệu.
7.  Trong thư mục gốc của solution, chạy lệnh `abp generate-proxy -t js --url https://localhost:44395` (thay URL nếu khác) để tạo hoặc cập nhật JavaScript proxies cho các AppService mới hoặc đã thay đổi.
8.  Chạy ứng dụng Web (`Aqt.CoreFW.Web`).

### Kiểm tra và xác nhận
1.  **Phân quyền:**
    *   Đăng nhập với user **có** quyền `CoreFW.Countries.Default`: Menu "Quốc gia" hiển thị.
    *   Đăng nhập với user **không** có quyền: Menu ẩn, truy cập URL `/Countries` bị từ chối.
    *   Kiểm tra các nút "Thêm mới", "Sửa", "Xóa" ẩn/hiện đúng theo quyền (`CoreFW.Countries.Create`, `.Edit`, `.Delete`).
2.  **Validation:**
    *   **Thêm mới/Sửa:**
        *   Để trống Mã hoặc Tên -> Form báo lỗi required.
        *   Nhập Mã hoặc Tên quá độ dài cho phép -> Form báo lỗi `StringLength`.
        *   Nhập Mã đã tồn tại -> Hệ thống báo lỗi "CountryCodeAlreadyExists" khi nhấn Lưu.
3.  **CRUD:**
    *   **Thêm mới:** Nhập Mã, Tên hợp lệ -> Lưu thành công, dữ liệu hiển thị đúng trên bảng.
    *   **Xem danh sách:**
        *   Kiểm tra dữ liệu hiển thị, phân trang, sắp xếp theo Mã/Tên.
        *   Nhập text vào ô tìm kiếm -> Nhấn "Search" -> Bảng lọc đúng kết quả theo Mã hoặc Tên.
    *   **Sửa:** Mở modal Sửa -> Thay đổi Tên -> Lưu thành công, Tên mới hiển thị đúng. Thử sửa Mã thành mã đã tồn tại (của nước khác) -> Báo lỗi.
    *   **Xóa:** Nhấn nút Xóa -> Hiện popup xác nhận -> Đồng ý -> Xóa thành công, dòng tương ứng biến mất khỏi bảng.
4.  **Giao diện:**
    *   Kiểm tra giao diện bảng, modal, các nút, ô nhập liệu.
    *   Kiểm tra responsive trên các kích thước màn hình khác nhau.
5.  **Localization:**
    *   Chuyển đổi ngôn ngữ (Anh/Việt) -> Kiểm tra tất cả các nhãn, tiêu đề, thông báo hiển thị đúng ngôn ngữ đã chọn.

</rewritten_file>