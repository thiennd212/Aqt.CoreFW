# Kế hoạch chi tiết: Tầng Web (`Aqt.CoreFW.Web`) - Quản lý Quốc gia

Phần này mô tả các thành phần cần tạo hoặc cập nhật trong tầng `Aqt.CoreFW.Web`.

## 1. Menu

- **Vị trí 1:** Cập nhật file `src/Aqt.CoreFW.Web/Menus/CoreFWMenus.cs`
- **Nội dung cần thêm:**
  ```csharp
  public class CoreFWMenus
  {
      private const string Prefix = "CoreFW";
      public const string Home = Prefix + ".Home";

      // Thêm menu item cho Countries
      public const string Countries = Prefix + ".Countries";
      // ... các menu items khác ...
  }
  ```
- **Vị trí 2:** Cập nhật file `src/Aqt.CoreFW.Web/Menus/CoreFWMenuContributor.cs`
- **Nội dung cần thêm trong phương thức `ConfigureMainMenuAsync` (hoặc tương đương):**
  ```csharp
  using System.Threading.Tasks;
  using Aqt.CoreFW.Localization;
  using Aqt.CoreFW.Permissions;
  using Volo.Abp.UI.Navigation;
  using Microsoft.Extensions.Localization;

  namespace Aqt.CoreFW.Web.Menus;

  public class CoreFWMenuContributor : IMenuContributor
  {
      public async Task ConfigureMenuAsync(MenuConfigurationContext context)
      {
          if (context.Menu.Name == StandardMenus.Main)
          {
              await ConfigureMainMenuAsync(context);
          }
      }

      private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
      {
          var l = context.GetLocalizer<CoreFWResource>();

          context.Menu.AddItem(
              new ApplicationMenuItem(
                  CoreFWMenus.Home,
                  l["Menu:Home"],
                  "~/",
                  icon: "fas fa-home",
                  order: 0
              )
          );

          // Lấy menu Administration (hoặc tạo group mới nếu muốn)
          var administration = context.Menu.GetAdministration();

          // Thêm mục Countries nếu user có quyền
          if (await context.IsGrantedAsync(CoreFWPermissions.Countries.Default))
          {
              administration.AddItem(new ApplicationMenuItem(
                  CoreFWMenus.Countries,
                  l["Menu:Countries"],
                  "/Countries" // Đường dẫn tới trang quản lý Countries
              ).RequirePermissions(CoreFWPermissions.Countries.Default)); // Yêu cầu quyền để thấy menu
          }

          // Cấu hình các menu item khác...
      }
  }
  ```

## 2. Razor Pages và ViewModels

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Web/Pages/Countries`

- **Tệp 1: ViewModel chung:** Tạo file `CountryViewModel.cs`
  ```csharp
  using System;
  using System.ComponentModel.DataAnnotations;
  using Aqt.CoreFW.Domain.Shared.Countries;
  using Microsoft.AspNetCore.Mvc;

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class CountryViewModel
  {
      [HiddenInput]
      public Guid Id { get; set; }

      [Required]
      [StringLength(CountryConsts.MaxCodeLength)]
      [Display(Name = "CountryCode")] // Tên hiển thị sẽ được lấy từ localization
      public string Code { get; set; }

      [Required]
      [StringLength(CountryConsts.MaxNameLength)]
      [Display(Name = "CountryName")] // Tên hiển thị sẽ được lấy từ localization
      public string Name { get; set; }
  }
  ```

- **Tệp 2: Trang danh sách:** Tạo file `Index.cshtml`
  ```cshtml
  @page
  @using Aqt.CoreFW.Permissions
  @using Microsoft.AspNetCore.Authorization
  @using Volo.Abp.AspNetCore.Mvc.UI.Layout
  @using Aqt.CoreFW.Web.Pages.Countries
  @using Aqt.CoreFW.Localization
  @using Microsoft.Extensions.Localization
  @using Aqt.CoreFW.Web.Menus
  @model IndexModel
  @inject IStringLocalizer<CoreFWResource> L
  @inject IAuthorizationService AuthorizationService
  @inject IPageLayout PageLayout
  @{
      PageLayout.Content.Title = L["Countries"].Value;
      PageLayout.Content.BreadCrumb.Add(L["Menu:Countries"].Value);
      PageLayout.Content.MenuItemName = CoreFWMenus.Countries;
  }

  @section scripts {
      <abp-script src="/Pages/Countries/index.js" /> @* JS cho trang này *@
  }

  @section content_toolbar {
      @if (await AuthorizationService.IsGrantedAsync(CoreFWPermissions.Countries.Create))
      {
          <abp-button id="NewCountryButton"
                      text="@L["NewCountry"].Value"
                      icon="plus"
                      button-type="Primary" />
      }
  }

  <abp-card>
      <abp-card-body>
          @* Bộ lọc tìm kiếm *@
          <abp-row class="mb-3">
               <abp-column size-md="_6">
                    <input type="text" id="SearchFilter" class="form-control" placeholder="@L["Search"]..." />
               </abp-column>
                <abp-column size-md="_6" class="text-end">
                     <abp-button id="SearchButton"
                               text="@L["Search"].Value"
                               icon="search"
                               button-type="Primary"/>
                </abp-column>
          </abp-row>

          @* Bảng dữ liệu *@
          <abp-table striped-rows="true" id="CountriesTable"></abp-table>
      </abp-card-body>
  </abp-card>
  ```

- **Tệp 3: PageModel danh sách:** Tạo file `Index.cshtml.cs`
  ```csharp
  using Aqt.CoreFW.Web.Pages;
  using Microsoft.AspNetCore.Mvc.RazorPages;

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class IndexModel : CoreFWPageModel
  {
      public void OnGet() { /* Không cần logic load ban đầu */ }
  }
  ```

- **Tệp 4: Modal Thêm mới:** Tạo file `CreateModal.cshtml`
  ```cshtml
  @page "/Countries/CreateModal"
  @using Microsoft.AspNetCore.Mvc.Localization
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
  @using Aqt.CoreFW.Localization
  @using Aqt.CoreFW.Web.Pages.Countries
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form
  @model CreateModalModel
  @inject IHtmlLocalizer<CoreFWResource> L
  @{
      Layout = null;
  }
  @* Sử dụng abp-dynamic-form với ViewModel *@
  <abp-dynamic-form abp-model="CountryViewModel" asp-page="/Countries/CreateModal">
      <abp-modal>
          <abp-modal-header title="@L["NewCountry"].Value"></abp-modal-header>
          <abp-modal-body>
              @* Tự động render input dựa trên thuộc tính của CountryViewModel *@
              <abp-form-content />
          </abp-modal-body>
          <abp-modal-footer buttons="@(AbpModalButtons.Cancel | AbpModalButtons.Save)"></abp-modal-footer>
      </abp-modal>
  </abp-dynamic-form>
  ```

- **Tệp 5: PageModel Thêm mới:** Tạo file `CreateModal.cshtml.cs`
  ```csharp
  using System.Threading.Tasks;
  using Aqt.CoreFW.Application.Contracts.Countries;
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Aqt.CoreFW.Web.Pages;
  using Microsoft.AspNetCore.Mvc;
  using Volo.Abp.ObjectMapping;

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class CreateModalModel : CoreFWPageModel
  {
      [BindProperty]
      public CountryViewModel CountryViewModel { get; set; }

      private readonly ICountryAppService _countryAppService;

      public CreateModalModel(ICountryAppService countryAppService)
      {
          _countryAppService = countryAppService;
          CountryViewModel = new CountryViewModel();
      }

      public void OnGet()
      {
          // Không cần logic gì khi GET modal trống
      }

      public async Task<IActionResult> OnPostAsync()
      {
          // Map từ ViewModel sang DTO trước khi gọi service
          var dto = ObjectMapper.Map<CountryViewModel, CreateUpdateCountryDto>(CountryViewModel);
          await _countryAppService.CreateAsync(dto);
          return NoContent();
      }
  }
  ```

- **Tệp 6: Modal Sửa:** Tạo file `EditModal.cshtml`
  ```cshtml
  @page "/Countries/EditModal"
  @using Microsoft.AspNetCore.Mvc.Localization
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
  @using Aqt.CoreFW.Localization
  @using Aqt.CoreFW.Web.Pages.Countries
  @using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form
  @model EditModalModel
  @inject IHtmlLocalizer<CoreFWResource> L
  @{
      Layout = null;
  }
  @* Sử dụng abp-dynamic-form với ViewModel *@
  <abp-dynamic-form abp-model="CountryViewModel" asp-page="/Countries/EditModal">
      <abp-modal>
          <abp-modal-header title="@L["EditCountry"].Value"></abp-modal-header>
          <abp-modal-body>
              @* Input ẩn cho Id đã có trong ViewModel *@
              @* Tự động render input dựa trên thuộc tính của CountryViewModel *@
              <abp-form-content />
          </abp-modal-body>
          <abp-modal-footer buttons="@(AbpModalButtons.Cancel | AbpModalButtons.Save)"></abp-modal-footer>
      </abp-modal>
  </abp-dynamic-form>
  ```

- **Tệp 7: PageModel Sửa:** Tạo file `EditModal.cshtml.cs`
  ```csharp
  using System;
  using System.Threading.Tasks;
  using Aqt.CoreFW.Application.Contracts.Countries;
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Aqt.CoreFW.Web.Pages;
  using Microsoft.AspNetCore.Mvc;
  using Volo.Abp.ObjectMapping;

  namespace Aqt.CoreFW.Web.Pages.Countries;

  public class EditModalModel : CoreFWPageModel
  {
      [HiddenInput]
      [BindProperty(SupportsGet = true)]
      public Guid Id { get; set; }

      [BindProperty]
      public CountryViewModel CountryViewModel { get; set; }

      private readonly ICountryAppService _countryAppService;

      public EditModalModel(ICountryAppService countryAppService)
      {
          _countryAppService = countryAppService;
      }

      public async Task OnGetAsync()
      {
          // Lấy dữ liệu DTO từ service
          var dto = await _countryAppService.GetAsync(Id);
          // Map sang ViewModel để bind vào form
          CountryViewModel = ObjectMapper.Map<CountryDto, CountryViewModel>(dto);
      }

      public async Task<IActionResult> OnPostAsync()
      {
          // Map từ ViewModel sang DTO trước khi gọi service
          var dto = ObjectMapper.Map<CountryViewModel, CreateUpdateCountryDto>(CountryViewModel);
          await _countryAppService.UpdateAsync(CountryViewModel.Id, dto);
          return NoContent();
      }
  }
  ```

## 3. AutoMapper Profile (Web)

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Web/Countries` (nếu chưa có)
- **Tệp:** Tạo file `CountryWebAutoMapperProfile.cs`
- **Nội dung:**
  ```csharp
  using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
  using Aqt.CoreFW.Web.Pages.Countries;
  using AutoMapper;

  namespace Aqt.CoreFW.Web.Countries;

  public class CountryWebAutoMapperProfile : Profile
  {
      public CountryWebAutoMapperProfile()
      {
          // Mapping giữa ViewModel và CreateUpdateCountryDto
          CreateMap<CountryViewModel, CreateUpdateCountryDto>();
          CreateMap<CreateUpdateCountryDto, CountryViewModel>();

          // Mapping từ CountryDto sang CountryViewModel (dùng cho Edit modal OnGet)
          CreateMap<CountryDto, CountryViewModel>();
          // Lưu ý: Nếu cần map từ CountryViewModel về CountryDto thì thêm dòng dưới
          // CreateMap<CountryViewModel, CountryDto>();
      }
  }
  ```
- **Đăng ký:** Không cần đăng ký thủ công nếu `options.AddMaps<CoreFWWebModule>();` đã có trong `CoreFWWebModule.cs`.

## 4. JavaScript

- **Vị trí:** Tạo thư mục `src/Aqt.CoreFW.Web/wwwroot/pages/countries`
- **Tệp:** Tạo file `index.js`
- **Nội dung:**
  ```javascript
  $(function () {
      // Lấy resource localization
      var l = abp.localization.getResource('CoreFW');

      // Lấy service proxy (Kiểm tra namespace chính xác sau khi generate proxy)
      var countryService = aqt.coreFW.application.contracts.countries.country;

      // Khởi tạo ModalManager
      var createModal = new abp.ModalManager(abp.appPath + 'Countries/CreateModal');
      var editModal = new abp.ModalManager(abp.appPath + 'Countries/EditModal');
      var dataTable = null;

      // Hàm lấy bộ lọc
       var getFilters = function() {
           return {
               filter: $('#SearchFilter').val()
           };
       }

      // Khởi tạo DataTable
       function initializeDataTable() {
           if (dataTable) {
                dataTable.destroy();
           }
           dataTable = $('#CountriesTable').DataTable(
              abp.libs.datatables.normalizeConfiguration({
                  serverSide: true,
                  paging: true,
                  order: [[1, "asc"]], // Sắp xếp mặc định theo cột thứ 2 (Code)
                  searching: false,
                  scrollX: true,
                  ajax: abp.libs.datatables.createAjax(countryService.getList, getFilters),
                  columnDefs: [
                      {
                          title: l('Actions'),
                          rowAction: {
                              items: [
                                  { // Nút Sửa
                                      text: l('Edit'),
                                      icon: "fa fa-pencil-alt",
                                      visible: abp.auth.isGranted('CoreFW.Countries.Edit'),
                                      action: function (data) {
                                          editModal.open({ id: data.record.id });
                                      }
                                  },
                                  { // Nút Xóa
                                      text: l('Delete'),
                                      icon: "fa fa-trash",
                                      visible: abp.auth.isGranted('CoreFW.Countries.Delete'),
                                      confirmMessage: function (data) {
                                          return l('AreYouSureToDeleteCountry', data.record.name || data.record.code);
                                      },
                                      action: function (data) {
                                          countryService.delete(data.record.id)
                                              .then(function () {
                                                  abp.notify.success(l('SuccessfullyDeleted'));
                                                  dataTable.ajax.reload();
                                              }).catch(function (error) {
                                                  abp.message.error(error.message || l('Error'), l('Error'));
                                               });
                                      }
                                  }
                              ]
                          }
                      },
                      {
                          title: l('CountryCode'),
                          data: "code",
                          orderable: true
                      },
                      {
                          title: l('CountryName'),
                          data: "name",
                          orderable: true
                      }
                  ]
              })
          );
      }

      initializeDataTable();

      // Sự kiện khi modal Tạo thành công
      createModal.onResult(function () {
          dataTable.ajax.reload();
      });

      // Sự kiện khi modal Sửa thành công
      editModal.onResult(function () {
          dataTable.ajax.reload();
      });

      // Sự kiện click nút "Thêm mới"
      $('#NewCountryButton').click(function (e) {
          e.preventDefault();
          createModal.open();
      });

      // Sự kiện click nút "Tìm kiếm"
      $('#SearchButton').click(function (e) {
           e.preventDefault();
           dataTable.ajax.reload();
       });

       // Sự kiện nhấn Enter trong ô tìm kiếm
       $('#SearchFilter').on('keypress', function(e) {
           if(e.which === 13) { // Enter key code
               dataTable.ajax.reload();
           }
       });
  });
  ```
  