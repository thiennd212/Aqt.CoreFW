# Kế hoạch chi tiết: Tầng Application (`Aqt.CoreFW.Application`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.Application`.

## 1. AutoMapper Profile

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Application/Countries`
- **Tệp:** Tạo file `CountryApplicationAutoMapperProfile.cs`
- **Nội dung (Đã cập nhật):**
  ```csharp
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Aqt.CoreFW.Domain.Countries.Entities;
  using AutoMapper;
  using Volo.Abp.ObjectExtending;
  using Volo.Abp.AutoMapper;
  using Volo.Abp.DependencyInjection;

  namespace Aqt.CoreFW.Application.Countries;

  public class CountryApplicationAutoMapperProfile : Profile
  {
      public CountryApplicationAutoMapperProfile()
      {
          // Mapping từ Entity sang DTO hiển thị (VẪN CẦN THIẾT)
          CreateMap<Country, CountryDto>();

          // --- ĐÃ LOẠI BỎ mapping từ CreateUpdateCountryDto sang Country ---
          // Việc tạo/cập nhật Entity sẽ được thực hiện thủ công trong AppService
          // sử dụng constructor và các phương thức của Entity.
          // CreateMap<CreateUpdateCountryDto, Country>()...;

          // Mapping từ DTO hiển thị sang DTO tạo/sửa (dùng cho Edit modal)
          CreateMap<CountryDto, CreateUpdateCountryDto>();

          // Mapping cho Lookup DTO (VẪN CẦN THIẾT)
          CreateMap<Country, CountryLookupDto>();
      }
  }
  ```

## 2. AppService Implementation

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Application/Countries`
- **Tệp:** Tạo file `CountryAppService.cs`
- **Nội dung (Đã cập nhật):**
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Application.Contracts.Countries;
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Aqt.CoreFW.Domain.Countries.Repositories;
  using Aqt.CoreFW.Domain.Countries.Entities;
  using Aqt.CoreFW; // For Error Codes
  using Aqt.CoreFW.Localization; // For Localization
  using Aqt.CoreFW.Permissions; // For Permissions
  using Microsoft.AspNetCore.Authorization;
  using Volo.Abp;
  using Volo.Abp.Application.Dtos;
  using Volo.Abp.Application.Services;
  using Volo.Abp.Domain.Repositories;
  using System.Linq.Dynamic.Core; // For WhereIf
  using Volo.Abp.ObjectMapping;
  using Volo.Abp.Guids;

  namespace Aqt.CoreFW.Application.Countries;

  [Authorize(CoreFWPermissions.Countries.Default)]
  public class CountryAppService :
      CrudAppService<
          Country,
          CountryDto,
          Guid,
          GetCountriesInput,
          CreateUpdateCountryDto>,
      ICountryAppService
  {
      private readonly ICountryRepository _countryRepository;
      private readonly IGuidGenerator _guidGenerator;

      public CountryAppService(
          IRepository<Country, Guid> repository,
          ICountryRepository countryRepository,
          IGuidGenerator guidGenerator)
          : base(repository)
      {
          _countryRepository = countryRepository;
          _guidGenerator = guidGenerator;
          LocalizationResource = typeof(CoreFWResource); // Set localization resource
          // Set các policy name cho CRUD operations
          GetPolicyName = CoreFWPermissions.Countries.Default;
          GetListPolicyName = CoreFWPermissions.Countries.Default;
          CreatePolicyName = CoreFWPermissions.Countries.Create;
          UpdatePolicyName = CoreFWPermissions.Countries.Edit;
          DeletePolicyName = CoreFWPermissions.Countries.Delete;
      }

      [Authorize(CoreFWPermissions.Countries.Create)]
      public override async Task<CountryDto> CreateAsync(CreateUpdateCountryDto input)
      {
          // Kiểm tra trùng mã
          if (await _countryRepository.CodeExistsAsync(input.Code))
          {
              throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryCodeAlreadyExists, input.Code]);
          }

          // Tạo Entity bằng constructor, không dùng ObjectMapper
          var entity = new Country(
              _guidGenerator.Create(),
              input.Code,
              input.Name
          );

          await Repository.InsertAsync(entity, autoSave: true);

          // Map Entity sang DTO để trả về (VẪN DÙNG ObjectMapper)
          return ObjectMapper.Map<Country, CountryDto>(entity);
      }

      [Authorize(CoreFWPermissions.Countries.Edit)]
      public override async Task<CountryDto> UpdateAsync(Guid id, CreateUpdateCountryDto input)
      {
          var entity = await GetEntityByIdAsync(id);

          // Kiểm tra trùng mã
          if (entity.Code != input.Code && await _countryRepository.CodeExistsAsync(input.Code, id))
          {
              throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryCodeAlreadyExists, input.Code]);
          }

          // Cập nhật Entity bằng các phương thức của nó, không dùng MapToEntity/ObjectMapper
          entity.SetCode(input.Code);
          entity.SetName(input.Name);

          await Repository.UpdateAsync(entity, autoSave: true);

          // Map Entity sang DTO để trả về (VẪN DÙNG ObjectMapper)
          return ObjectMapper.Map<Country, CountryDto>(entity);
      }

      // Ghi đè phương thức Delete để kiểm tra ràng buộc Province/City
      [Authorize(CoreFWPermissions.Countries.Delete)]
      public override async Task DeleteAsync(Guid id)
      {
          // TODO: Bỏ comment khi có Province
          /*
          // Kiểm tra xem quốc gia có tỉnh/thành nào không
          if (await _countryRepository.HasProvincesAsync(id))
          {
              var entity = await GetEntityByIdAsync(id); // Lấy tên để hiển thị lỗi
              throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryHasProvincesCannotDelete, entity.Name ?? entity.Code]);
          }
          */
          // Nếu không có ràng buộc, gọi phương thức xóa của base (thực hiện soft delete)
          await base.DeleteAsync(id);
      }

      // Implement phương thức GetLookupAsync
      [AllowAnonymous] // Cho phép truy cập không cần đăng nhập (tùy yêu cầu)
      public async Task<ListResultDto<CountryLookupDto>> GetLookupAsync()
      {
          // Lấy danh sách country từ repository
          var countries = await _countryRepository.GetListAsync(sorting: nameof(Country.Name));

          // Map sang Lookup DTO
          var lookupDtos = ObjectMapper.Map<List<Country>, List<CountryLookupDto>>(countries);

          return new ListResultDto<CountryLookupDto>(lookupDtos);
      }

      // Ghi đè GetListAsync để sử dụng phương thức có filter từ ICountryRepository (tùy chọn, nhưng nên làm để rõ ràng)
      // CrudAppService cũng có cơ chế lọc riêng, nhưng ghi đè giúp kiểm soát tốt hơn
      public override async Task<PagedResultDto<CountryDto>> GetListAsync(GetCountriesInput input)
      {
          // Lấy tổng số lượng bản ghi thỏa mãn điều kiện lọc (không phân trang)
          var totalCount = await _countryRepository.GetCountAsync(input.Filter);

          // Lấy danh sách bản ghi đã lọc và phân trang, sắp xếp
          var countries = await _countryRepository.GetListAsync(
              filterText: input.Filter,
              sorting: input.Sorting,
              maxResultCount: input.MaxResultCount,
              skipCount: input.SkipCount
          );

          // Map kết quả sang DTO
          var countryDtos = ObjectMapper.Map<List<Country>, List<CountryDto>>(countries);

          return new PagedResultDto<CountryDto>(totalCount, countryDtos);
      }

      // Phương thức này được CrudAppService gọi để áp dụng filter mặc định.
      // Ghi đè nó để sử dụng logic lọc từ repository tùy chỉnh hoặc thêm bộ lọc phức tạp hơn.
      // Nếu GetListAsync đã được ghi đè hoàn toàn (như ở trên), phương thức này có thể không cần thiết.
      /*
      protected override async Task<IQueryable<Country>> CreateFilteredQueryAsync(GetCountriesInput input)
      {
          var queryable = await Repository.GetQueryableAsync();
          return queryable
              .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                  c => c.Code.Contains(input.Filter) || c.Name.Contains(input.Filter));
          // Lưu ý: ABP CrudAppService sẽ tự động áp dụng Paging và Sorting sau bước này
      }
      */
  }
  ```

## 3. Ghi chú thay đổi về tạo/cập nhật Entity (Cập nhật)

- **Nguyên tắc cốt lõi: Không AutoMap DTO -> Entity:**
  - **Tạo mới:** Luôn sử dụng **constructor** của Entity trong `AppService`. Truyền `Guid` mới (từ `IGuidGenerator`) và dữ liệu từ `CreateUpdateDto`.
  - **Cập nhật:** Luôn gọi các **phương thức setter công khai** (ví dụ: `SetCode`, `SetName`) trên Entity instance được lấy từ Repository. Truyền dữ liệu từ `CreateUpdateDto`.
  - **Cấm:** Không sử dụng `ObjectMapper.Map<CreateUpdateDto, Entity>` hoặc `MapToEntity` cho việc tạo/cập nhật.
  - **Lý do:** Đảm bảo Entity luôn hợp lệ, tuân thủ đóng gói và bất biến (DDD).

- **Loại bỏ AutoMapper DTO -> Entity:** Cấu hình `CreateMap<CreateUpdateCountryDto, Country>()` đã bị xóa khỏi `CountryApplicationAutoMapperProfile`.

</rewritten_file> 