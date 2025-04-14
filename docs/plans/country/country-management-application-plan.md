# Kế hoạch chi tiết: Tầng Application (`Aqt.CoreFW.Application`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.Application`.

## 1. AutoMapper Profile

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Application/Countries`
- **Tệp:** Tạo file `CountryApplicationAutoMapperProfile.cs`
- **Nội dung:**
  ```csharp
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Aqt.CoreFW.Domain.Countries.Entities;
  using AutoMapper;
  using Volo.Abp.ObjectExtending; // Required for IgnoreAuditedObjectProperties
  using Volo.Abp.AutoMapper;
  using Volo.Abp.DependencyInjection; // Required for GuidGenerator
  using Volo.Abp.Guids; // Required for GuidGenerator

  namespace Aqt.CoreFW.Application.Countries;

  public class CountryApplicationAutoMapperProfile : Profile
  {
      // Inject IGuidGenerator để tạo Guid khi mapping
      private readonly IGuidGenerator _guidGenerator;

      public CountryApplicationAutoMapperProfile(IGuidGenerator guidGenerator)
      {
          _guidGenerator = guidGenerator;

          // Mapping từ Entity sang DTO hiển thị
          CreateMap<Country, CountryDto>();

          // Mapping từ DTO tạo/sửa sang Entity
          // Bỏ qua các thuộc tính audit và Id vì chúng được quản lý bởi ABP/EF Core
          CreateMap<CreateUpdateCountryDto, Country>()
              .IgnoreAuditedObjectProperties() // Bỏ qua các thuộc tính audit base
              .Ignore(x => x.Id)               // Id sẽ được tạo mới hoặc lấy từ existing entity
              // Dùng ConstructUsing để đảm bảo Id mới được tạo khi Create
              .ConstructUsing(dto => new Country(_guidGenerator.Create(), dto.Code, dto.Name));

          // Mapping từ DTO hiển thị sang DTO tạo/sửa (dùng cho Edit modal)
          CreateMap<CountryDto, CreateUpdateCountryDto>();

          // Mapping cho Lookup DTO
          CreateMap<Country, CountryLookupDto>();
      }
  }
  ```

## 2. AppService Implementation

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Application/Countries`
- **Tệp:** Tạo file `CountryAppService.cs`
- **Nội dung:**
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Application.Contracts.Countries;
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Aqt.CoreFW.Domain.Countries.Repositories;
  using Aqt.CoreFW.Domain.Countries.Entities;
  using Aqt.CoreFW.Domain.Shared; // For Error Codes
  using Aqt.CoreFW.Localization; // For Localization
  using Aqt.CoreFW.Permissions; // For Permissions
  using Microsoft.AspNetCore.Authorization;
  using Volo.Abp;
  using Volo.Abp.Application.Dtos;
  using Volo.Abp.Application.Services;
  using Volo.Abp.Domain.Repositories;
  using System.Linq.Dynamic.Core; // For WhereIf

  namespace Aqt.CoreFW.Application.Countries;

  [Authorize(CoreFWPermissions.Countries.Default)] // Quyền mặc định để truy cập
  public class CountryAppService :
      CrudAppService<               // Kế thừa CrudAppService
          Country,                  // Entity
          CountryDto,               // DTO đọc
          Guid,                     // Kiểu khóa chính
          GetCountriesInput,        // Input cho GetList
          CreateUpdateCountryDto>,  // Input cho Create/Update
      ICountryAppService            // Implement interface
  {
      private readonly ICountryRepository _countryRepository;

      public CountryAppService(
          IRepository<Country, Guid> repository, // Standard repository for base CrudAppService
          ICountryRepository countryRepository) // Custom repository
          : base(repository)
      {
          _countryRepository = countryRepository;
          LocalizationResource = typeof(CoreFWResource); // Set localization resource
          // Set các policy name cho CRUD operations
          GetPolicyName = CoreFWPermissions.Countries.Default;
          GetListPolicyName = CoreFWPermissions.Countries.Default;
          CreatePolicyName = CoreFWPermissions.Countries.Create;
          UpdatePolicyName = CoreFWPermissions.Countries.Edit;
          DeletePolicyName = CoreFWPermissions.Countries.Delete;
      }

      // Ghi đè phương thức Create để thêm kiểm tra unique code
      [Authorize(CoreFWPermissions.Countries.Create)]
      public override async Task<CountryDto> CreateAsync(CreateUpdateCountryDto input)
      {
          // Kiểm tra trùng mã trước khi tạo
          if (await _countryRepository.CodeExistsAsync(input.Code))
          {
              throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryCodeAlreadyExists, input.Code]);
          }

          // Sử dụng base.CreateAsync để thực hiện mapping và insert chuẩn
          return await base.CreateAsync(input);
      }

      // Ghi đè phương thức Update để thêm kiểm tra unique code (loại trừ chính nó)
      [Authorize(CoreFWPermissions.Countries.Edit)]
      public override async Task<CountryDto> UpdateAsync(Guid id, CreateUpdateCountryDto input)
      {
          // Lấy entity hiện tại
          var entity = await GetEntityByIdAsync(id);

          // Kiểm tra trùng mã (loại trừ Id hiện tại)
          if (entity.Code != input.Code && await _countryRepository.CodeExistsAsync(input.Code, id))
          {
              throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryCodeAlreadyExists, input.Code]);
          }

          // Map dữ liệu từ input DTO vào entity đã tồn tại
          // MapToEntity là phương thức được bảo vệ (protected) của CrudAppService
          MapToEntity(input, entity);

          // Cập nhật entity
          await Repository.UpdateAsync(entity, autoSave: true);

          // Map lại entity đã cập nhật sang DTO để trả về
          // MapToGetOutputDto là phương thức được bảo vệ (protected) của CrudAppService
          return MapToGetOutputDto(entity);
      }

       // Ghi đè phương thức Delete để kiểm tra ràng buộc Province/City
       [Authorize(CoreFWPermissions.Countries.Delete)]
       public override async Task DeleteAsync(Guid id)
       {
           // Kiểm tra xem quốc gia có tỉnh/thành nào không
           if (await _countryRepository.HasProvincesAsync(id))
           {
               var entity = await GetEntityByIdAsync(id); // Lấy tên để hiển thị lỗi
               throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryHasProvincesCannotDelete, entity.Name ?? entity.Code]);
           }

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
      protected override async Task<IQueryable<Country>> CreateFilteredQueryAsync(GetCountriesInput input)
      {
          var queryable = await Repository.GetQueryableAsync();
          return queryable
              .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                  c => c.Code.Contains(input.Filter) || c.Name.Contains(input.Filter));
          // Lưu ý: ABP CrudAppService sẽ tự động áp dụng Paging và Sorting sau bước này
          // Nếu GetListAsync đã được ghi đè hoàn toàn (như ở trên), phương thức này có thể không cần thiết.
      }
  }
  ```

</rewritten_file> 