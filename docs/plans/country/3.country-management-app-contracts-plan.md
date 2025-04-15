# Kế hoạch chi tiết: Tầng Application.Contracts (`Aqt.CoreFW.Application.Contracts`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.Application.Contracts`.

## 1. DTOs

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Application.Contracts/Countries/Dtos`
- **Tệp 1:** Tạo file `CountryDto.cs`
  ```csharp
  using System;
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

  public class CountryDto : AuditedEntityDto<Guid>
  {
      public string Code { get; set; }
      public string Name { get; set; }
  }
  ```
- **Tệp 2:** Tạo file `CreateUpdateCountryDto.cs`
  ```csharp
  using System;
  using System.ComponentModel.DataAnnotations;
  using Aqt.CoreFW.Domain.Shared.Countries; // Using constants

  namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

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
- **Tệp 3:** Tạo file `GetCountriesInput.cs`
  ```csharp
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

  public class GetCountriesInput : PagedAndSortedResultRequestDto
  {
      public string? Filter { get; set; } // Lọc theo Code hoặc Name
  }
  ```
- **Tệp 4:** Tạo file `CountryLookupDto.cs`
  ```csharp
  using System;
  using Volo.Abp.Application.Dtos;

  namespace Aqt.CoreFW.Application.Contracts.Countries.Dtos;

  // Dùng cho dropdown chọn quốc gia ở màn hình khác (VD: Tỉnh/Thành)
  public class CountryLookupDto : EntityDto<Guid>
  {
      public string Name { get; set; }
      // Có thể thêm Code nếu cần hiển thị
      // public string Code { get; set; }
  }
  ```

## 2. AppService Interface

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Application.Contracts/Countries`
- **Tệp:** Tạo file `ICountryAppService.cs`
  ```csharp
  using System;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Volo.Abp.Application.Dtos;
  using Volo.Abp.Application.Services;

  namespace Aqt.CoreFW.Application.Contracts.Countries;

  public interface ICountryAppService :
      ICrudAppService< // Kế thừa ICrudAppService vì đây là CRUD cơ bản
          CountryDto,               // DTO để hiển thị
          Guid,                     // Kiểu khóa chính
          GetCountriesInput,        // DTO Input cho GetList
          CreateUpdateCountryDto>   // DTO Input cho Create/Update
  {
      // Thêm phương thức này để lấy danh sách lookup cho dropdown
      Task<ListResultDto<CountryLookupDto>> GetLookupAsync();
  }
  ```

## 3. Permissions

- **Vị trí 1:** Cập nhật file `src/Aqt.CoreFW.Application.Contracts/Permissions/CoreFWPermissions.cs`
- **Nội dung cần thêm:**
  ```csharp
  public static class CoreFWPermissions
  {
      public const string GroupName = "CoreFW";

      // ... các permission hiện có ...

      public static class Countries
      {
          public const string Default = GroupName + ".Countries";
          public const string Create = Default + ".Create";
          public const string Edit = Default + ".Edit";
          public const string Delete = Default + ".Delete";
      }
      // ... các module permission khác ...
  }
  ```
- **Vị trí 2:** Cập nhật file `src/Aqt.CoreFW.Application.Contracts/Permissions/CoreFWPermissionDefinitionProvider.cs`
- **Nội dung cần thêm trong phương thức `Define`:**
  ```csharp
  using Aqt.CoreFW.Localization;
  using Volo.Abp.Authorization.Permissions;
  using Volo.Abp.Localization;

  namespace Aqt.CoreFW.Permissions;

  public class CoreFWPermissionDefinitionProvider : PermissionDefinitionProvider
  {
      public override void Define(IPermissionDefinitionContext context)
      {
          var myGroup = context.AddGroup(CoreFWPermissions.GroupName);

          // Định nghĩa các permission khác nếu có...

          // Định nghĩa permission cho Countries
          var countriesPermission = myGroup.AddPermission(CoreFWPermissions.Countries.Default, L("Permission:Countries"));
          countriesPermission.AddChild(CoreFWPermissions.Countries.Create, L("Permission:Countries.Create"));
          countriesPermission.AddChild(CoreFWPermissions.Countries.Edit, L("Permission:Countries.Edit"));
          countriesPermission.AddChild(CoreFWPermissions.Countries.Delete, L("Permission:Countries.Delete"));
      }

      private static LocalizableString L(string name)
      {
          return LocalizableString.Create<CoreFWResource>(name);
      }
  }
  ```

</rewritten_file> 