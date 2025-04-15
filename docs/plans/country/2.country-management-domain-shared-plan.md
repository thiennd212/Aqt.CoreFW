# Kế hoạch chi tiết: Tầng Domain.Shared (`Aqt.CoreFW.Domain.Shared`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.Domain.Shared`.

## 1. Constants

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Domain.Shared/Countries`
- **Tệp:** Tạo file `CountryConsts.cs`
- **Nội dung:**
  ```csharp
  namespace Aqt.CoreFW.Domain.Shared.Countries;

  public static class CountryConsts
  {
      // Ví dụ độ dài tối đa, có thể điều chỉnh
      public const int MaxCodeLength = 5;
      public const int MaxNameLength = 100;
  }
  ```

## 2. Localization

- **Vị trí:** Cập nhật các file `*.json` trong `src/Aqt.CoreFW.Domain.Shared/Localization/CoreFW` (ví dụ: `en.json`, `vi.json`)
- **Nội dung cần thêm:**
  ```json
  {
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
    "CannotDeleteCountryWithProvinces": "Cannot delete the country '{0}' because it still has associated provinces/cities."
    // Thêm các key localization khác nếu cần
  }
  ```

## 3. Error Codes

- **Vị trí:** Cập nhật file `src/Aqt.CoreFW.Domain.Shared/CoreFWDomainErrorCodes.cs`
- **Nội dung cần thêm:**
  ```csharp
  namespace Aqt.CoreFW;

  public static class CoreFWDomainErrorCodes
  {
      /* Thêm các mã lỗi ở đây */
      // ... các mã lỗi hiện có ...
      public const string CountryCodeAlreadyExists = "CoreFW:00011"; // Điều chỉnh mã nếu cần
      public const string CountryHasProvincesCannotDelete = "CoreFW:00012";
  }
  ``` 