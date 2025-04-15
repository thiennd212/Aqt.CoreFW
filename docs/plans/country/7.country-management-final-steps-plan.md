# Kế hoạch chi tiết: Các bước triển khai và kiểm thử cuối cùng - Quản lý Quốc gia

**LƯU Ý:** Các bước trong kế hoạch này sẽ được thực hiện thủ công bởi người dùng.

Phần này mô tả các bước cần thực hiện sau khi đã hoàn thành code cho tất cả các tầng (Bước 1 đến Bước 6).

## 1. Các bước triển khai (Thực hiện thủ công)

1.  **Build Solution:** Chạy lệnh `dotnet build Aqt.CoreFW.sln` (hoặc build từ Visual Studio) để đảm bảo không có lỗi biên dịch.
2.  **Tạo Migration:**
    - Mở terminal hoặc command prompt.
    - Chuyển đến thư mục `src/Aqt.CoreFW.DbMigrator` (hoặc thư mục chứa dự án DbMigrator).
    - Chạy lệnh: `dotnet ef migrations add Added_Countries_Feature -p ../Aqt.CoreFW.EntityFrameworkCore/ -s .`
    *(Lưu ý: Điều chỉnh đường dẫn `-p` nếu cấu trúc thư mục khác. Lệnh này chỉ định rằng migration sẽ được thêm vào dự án EntityFrameworkCore nhưng thực thi từ context của DbMigrator)*
3.  **Kiểm tra Migration:** Mở file migration vừa được tạo trong thư mục `Migrations` của dự án `Aqt.CoreFW.EntityFrameworkCore`. Xem xét cẩn thận các lệnh tạo bảng, cột, khóa chính, index để đảm bảo chúng đúng với thiết kế trong `CountryConfiguration.cs`.
4.  **Áp dụng Migration:** Chạy dự án `Aqt.CoreFW.DbMigrator` (thường là một ứng dụng Console) để cập nhật schema của database.
5.  **Tạo/Cập nhật JS Proxies:**
    - Mở terminal hoặc command prompt.
    - Chuyển đến thư mục gốc của solution hoặc thư mục dự án Web (`src/Aqt.CoreFW.Web`).
    - Chạy lệnh: `abp generate-proxy -t js --module web`
    *Lệnh này sẽ tạo hoặc cập nhật các file JavaScript proxy trong `wwwroot/scripts/proxy/` của dự án Web, cho phép gọi các AppService từ JavaScript phía client.*
6.  **Build lại và Chạy ứng dụng Web:** Build lại toàn bộ solution và chạy dự án `Aqt.CoreFW.Web`.

## 2. Kiểm tra và xác nhận (Checklist - Thực hiện thủ công)

Thực hiện các kiểm tra sau trên ứng dụng Web đang chạy:

1.  **Phân quyền:**
    - [ ] Đăng nhập user **có** quyền `CoreFW.Countries.Default`: Menu "Countries" hiển thị đúng vị trí.
    - [ ] Truy cập trang `/Countries` thành công.
    - [ ] Đăng nhập user **không** có quyền `CoreFW.Countries.Default`: Menu "Countries" bị ẩn.
    - [ ] Truy cập trực tiếp URL `/Countries`: Bị từ chối (lỗi 403 hoặc trang lỗi quyền).
    - [ ] Trang danh sách: Nút "Thêm mới" hiển thị/ẩn đúng theo quyền `CoreFW.Countries.Create`.
    - [ ] Bảng danh sách: Nút "Sửa" trong cột Actions hiển thị/ẩn đúng theo quyền `CoreFW.Countries.Edit`.
    - [ ] Bảng danh sách: Nút "Xóa" trong cột Actions hiển thị/ẩn đúng theo quyền `CoreFW.Countries.Delete`.
2.  **Validation:**
    - [ ] Mở modal **Thêm mới**:
        - [ ] Để trống Mã -> Click Lưu -> Hiển thị lỗi validation.
        - [ ] Để trống Tên -> Click Lưu -> Hiển thị lỗi validation.
        - [ ] Nhập Mã dài hơn `MaxCodeLength` -> Click Lưu -> Hiển thị lỗi validation.
        - [ ] Nhập Tên dài hơn `MaxNameLength` -> Click Lưu -> Hiển thị lỗi validation.
        - [ ] Nhập Mã đã tồn tại -> Click Lưu -> Hiển thị lỗi `CountryCodeAlreadyExists`.
    - [ ] Mở modal **Sửa**:
        - [ ] Xóa trắng Mã -> Click Lưu -> Hiển thị lỗi validation.
        - [ ] Xóa trắng Tên -> Click Lưu -> Hiển thị lỗi validation.
        - [ ] Sửa Mã thành một mã đã tồn tại (của quốc gia khác) -> Click Lưu -> Hiển thị lỗi `CountryCodeAlreadyExists`.
3.  **Chức năng CRUD:**
    - [ ] **Thêm mới:** Nhập Mã, Tên hợp lệ -> Click Lưu -> Modal đóng, bảng cập nhật, hiển thị thông báo thành công.
    - [ ] **Xem danh sách:**
        - [ ] Dữ liệu Mã, Tên hiển thị đúng.
        - [ ] Phân trang hoạt động (tạo đủ dữ liệu để kiểm tra).
        - [ ] Sắp xếp theo Mã, Tên hoạt động.
        - [ ] Nhập text vào ô tìm kiếm -> Click Tìm kiếm/Enter -> Bảng load lại đúng dữ liệu.
    - [ ] **Sửa:** Click nút Sửa -> Modal hiện đúng thông tin -> Thay đổi Tên/Mã -> Click Lưu -> Modal đóng, bảng cập nhật, thông báo thành công.
    - [ ] **Xóa:**
        - [ ] Click nút Xóa -> Hiển thị hộp thoại xác nhận đúng.
        - [ ] Click Hủy -> Không có gì thay đổi.
        - [ ] Click Xác nhận -> Bản ghi biến mất, thông báo thành công.
        - [ ] Kiểm tra DB: bản ghi có `IsDeleted = true`.
        - [ ] *Kiểm tra ràng buộc:* (Cần có dữ liệu Tỉnh/Thành phố liên kết) Thử xóa Quốc gia có Tỉnh/Thành -> Click Xác nhận -> Hiển thị lỗi `CannotDeleteCountryWithProvinces`, bản ghi không bị xóa. **(Lưu ý: Phần kiểm tra này sẽ thực hiện sau khi Province được tạo và code liên quan được bỏ comment)**.
4.  **Giao diện:**
    - [ ] Bảng, modal, nút, input hiển thị đúng, không lỗi layout.
    - [ ] Responsive (nếu có yêu cầu).
5.  **Localization:**
    - [ ] Chuyển ngôn ngữ (nếu có) -> Tất cả text (menu, tiêu đề trang, label cột, nút, thông báo lỗi, thông báo thành công) hiển thị đúng ngôn ngữ.

## 3. Lưu ý khác (Thực hiện thủ công)

- Đảm bảo các bước phụ thuộc (như tạo bảng Provinces nếu cần kiểm tra ràng buộc xóa) đã hoàn thành. **(Lưu ý: Hiện tại chưa hoàn thành, cần bỏ comment code ở `CountryRepository` và `CountryAppService` sau khi Province được tạo)**.
- Kiểm tra kỹ `using` statements trong tất cả các file code mới hoặc được sửa đổi.
- Xem xét việc thêm `[DisableAuditing]` vào các phương thức `GetListAsync`, `GetAsync`, `GetLookupAsync` trong `CountryAppService` nếu không cần ghi log cho các hành động đọc này. 