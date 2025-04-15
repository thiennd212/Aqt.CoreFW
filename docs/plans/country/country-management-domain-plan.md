# Kế hoạch chi tiết: Tầng Domain (`Aqt.CoreFW.Domain`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.Domain` để hỗ trợ chức năng quản lý Quốc gia.

## 1. Entity

**Nguyên tắc thiết kế Entity (DDD):**

*   **Đóng gói (Encapsulation):**
    *   Ưu tiên `private set` cho các thuộc tính để kiểm soát việc thay đổi trạng thái từ bên ngoài.
    *   Việc thay đổi trạng thái phải thông qua các phương thức công khai của thực thể.
*   **Hành vi (Behavior):**
    *   Định nghĩa các phương thức công khai (`public`) để thể hiện hành vi nghiệp vụ (ví dụ: `SetCode`, `SetName`, `Activate`, `Archive`).
    *   Đặt logic validation liên quan đến việc thay đổi trạng thái *bên trong* các phương thức này.
*   **Trạng thái hợp lệ (Valid State):**
    *   Sử dụng **constructor** để yêu cầu các dữ liệu cần thiết khi tạo mới, đảm bảo thực thể luôn ở trạng thái hợp lệ ngay từ đầu.
    *   Áp dụng validation trong constructor và các phương thức thay đổi trạng thái.
*   **Kế thừa:** Sử dụng đúng các lớp base của ABP (`AggregateRoot`, `FullAuditedAggregateRoot`, etc.) dựa trên nhu cầu audit và vai trò (root/sub-entity).

**Cấu trúc `Country.cs` (Đã cập nhật):**

- **Vị trí:** `src/Aqt.CoreFW.Domain/Countries/Entities/Country.cs`
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
      // Thuộc tính Code với private set
      public virtual string Code { get; private set; }
      // Thuộc tính Name với private set
      public virtual string Name { get; private set; }

      // Constructor cho ORM
      protected Country() { /* For ORM */ }

      // Constructor chính, yêu cầu dữ liệu bắt buộc và gọi setter
      public Country(Guid id, [NotNull] string code, [NotNull] string name) : base(id)
      {
          SetCode(code); // Gọi phương thức để áp dụng validation
          SetName(name); // Gọi phương thức để áp dụng validation
      }

      // Phương thức CÔNG KHAI để thay đổi mã (đã sửa từ internal)
      public Country SetCode([NotNull] string code)
      {
          Check.NotNullOrWhiteSpace(code, nameof(code));
          Check.Length(code, nameof(code), CountryConsts.MaxCodeLength);
          Code = code;
          return this;
      }

      // Phương thức CÔNG KHAI để thay đổi tên
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