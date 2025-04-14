# Kế hoạch chi tiết: Tầng Domain (`Aqt.CoreFW.Domain`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.Domain` để hỗ trợ chức năng quản lý Quốc gia.

## 1. Entity

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Domain/Countries/Entities`
- **Tệp:** Tạo file `Country.cs`
- **Nội dung:**
  ```csharp
  using System;
  using Aqt.CoreFW.Domain.Shared.Countries; // Sẽ tạo ở bước sau
  using JetBrains.Annotations;
  using Volo.Abp;
  using Volo.Abp.Domain.Entities.Auditing;

  namespace Aqt.CoreFW.Domain.Countries.Entities;

  public class Country : FullAuditedAggregateRoot<Guid>
  {
      public virtual string Code { get; private set; }
      public virtual string Name { get; set; } // Cho phép sửa tên

      protected Country() { /* For ORM */ }

      public Country(Guid id, [NotNull] string code, [NotNull] string name) : base(id)
      {
          SetCode(code);
          SetName(name);
      }

      // Phương thức nội bộ để thay đổi mã, được gọi bởi AppService sau khi kiểm tra unique
      internal Country SetCode([NotNull] string code)
      {
          Check.NotNullOrWhiteSpace(code, nameof(code));
          Check.Length(code, nameof(code), CountryConsts.MaxCodeLength);
          Code = code;
          return this;
      }

      // Phương thức công khai để thay đổi tên
      public Country SetName([NotNull] string name)
      {
          Check.NotNullOrWhiteSpace(name, nameof(name));
          Check.Length(name, nameof(name), CountryConsts.MaxNameLength);
          Name = name;
          return this;
      }
  }
  ```

## 2. Repository Interface

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Domain/Countries` (nếu chưa có)
- **Tệp:** Tạo file `ICountryRepository.cs`
- **Nội dung:**
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Domain.Countries.Entities;
  using Volo.Abp.Domain.Repositories;

  namespace Aqt.CoreFW.Domain.Countries;

  public interface ICountryRepository : IRepository<Country, Guid>
  {
      // Tìm quốc gia theo mã
      Task<Country?> FindByCodeAsync(
          string code,
          CancellationToken cancellationToken = default);

      // Kiểm tra mã quốc gia đã tồn tại chưa, loại trừ ID hiện tại (khi cập nhật)
      Task<bool> CodeExistsAsync(
          string code,
          Guid? excludedId = null,
          CancellationToken cancellationToken = default);

      // Lấy danh sách quốc gia có lọc, phân trang, sắp xếp
      Task<List<Country>> GetListAsync(
          string? filterText = null,
          string? sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = 0,
          CancellationToken cancellationToken = default);

      // Đếm số lượng quốc gia theo bộ lọc
      Task<long> GetCountAsync(
          string? filterText = null,
          CancellationToken cancellationToken = default);

      // Kiểm tra xem quốc gia có tỉnh/thành phố liên kết không
      Task<bool> HasProvincesAsync(
          Guid countryId,
          CancellationToken cancellationToken = default);
  }
  ``` 