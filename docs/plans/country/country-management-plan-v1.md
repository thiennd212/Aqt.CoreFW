# Kế hoạch triển khai chức năng quản lý danh mục Quốc gia

## 0. Phân tích nghiệp vụ

### 0.1. Mục tiêu
- Xây dựng chức năng cho phép người quản trị hệ thống hoặc người dùng được cấp quyền quản lý danh mục các Quốc gia trong hệ thống.
- Đảm bảo dữ liệu Quốc gia (mã, tên) là nhất quán, chính xác và làm cơ sở cho các danh mục phụ thuộc khác (như Tỉnh/Thành phố).

### 0.2. Đối tượng sử dụng
- Quản trị viên hệ thống hoặc người dùng được cấp quyền quản lý danh mục Quốc gia.

### 0.3. Yêu cầu chức năng chính (CRUD)
- **Xem danh sách (Read):** Hiển thị danh sách các Quốc gia đã có trong hệ thống dưới dạng bảng. Hỗ trợ tìm kiếm theo Mã hoặc Tên Quốc gia, phân trang và sắp xếp.
- **Thêm mới (Create):** Cho phép người dùng thêm một Quốc gia mới vào danh mục. Yêu cầu nhập Mã Quốc gia (Code) và Tên Quốc gia (Name).
- **Sửa (Update):** Cho phép người dùng chỉnh sửa thông tin (Mã, Tên) của một Quốc gia đã tồn tại.
- **Xóa (Delete):** Cho phép người dùng xóa một Quốc gia khỏi danh mục. Sử dụng cơ chế xóa mềm (Soft Delete). Cần có bước xác nhận trước khi xóa và kiểm tra ràng buộc dữ liệu (không xóa nếu còn Tỉnh/Thành phố).

### 0.4. Yêu cầu dữ liệu
- **Mã Quốc gia (Code):**
    - Kiểu dữ liệu: Chuỗi (String).
    - Bắt buộc nhập.
    - Độ dài tối đa: 5 ký tự (ví dụ, theo `CountryConsts`).
    - Phải là duy nhất (unique) trên toàn hệ thống.
- **Tên Quốc gia (Name):**
    - Kiểu dữ liệu: Chuỗi (String).
    - Bắt buộc nhập.
    - Độ dài tối đa: 100 ký tự (ví dụ, theo `CountryConsts`).
- **Thông tin Audit:** Lưu trữ thông tin về người tạo, thời gian tạo, người sửa cuối cùng, thời gian sửa cuối cùng, trạng thái xóa mềm (IsDeleted). (Kế thừa từ `FullAuditedAggregateRoot`).

### 0.5. Yêu cầu giao diện người dùng (UI)
- **Màn hình danh sách:**
    - Ô tìm kiếm để lọc theo Mã hoặc Tên.
    - Bảng hiển thị các cột: Mã Quốc gia, Tên Quốc gia.
    - Nút "Thêm mới Quốc gia".
    - Các nút hành động (Sửa, Xóa) trên mỗi dòng của bảng.
    - Phân trang.
- **Modal Thêm mới/Sửa:**
    - Form nhập liệu cho Mã và Tên Quốc gia.
    - Các nút Lưu và Hủy.

### 0.6. Yêu cầu về phân quyền
- Định nghĩa các quyền riêng biệt cho việc xem danh sách, thêm, sửa, xóa Quốc gia.
- Chỉ những người dùng được gán quyền tương ứng mới có thể thực hiện các thao tác đó. Giao diện cần ẩn/hiện các nút chức năng dựa trên quyền của người dùng.

### 0.7. Quy tắc nghiệp vụ
- Mã Quốc gia không được trùng lặp trên toàn hệ thống. Hệ thống phải kiểm tra và thông báo lỗi nếu người dùng cố gắng tạo hoặc sửa thành một mã đã tồn tại.
- **Không cho phép xóa Quốc gia nếu Quốc gia đó vẫn còn Tỉnh/Thành phố liên kết.** Cần kiểm tra ràng buộc này trước khi thực hiện xóa mềm.

**Lưu ý chung:** Luôn luôn cập nhật kết quả mỗi bước sau khi hoàn thành vào tệp kế hoạch này.

## Tóm tắt Tiến độ Thực hiện Dự kiến

- [x] **Bước 1: Tầng Domain (`Aqt.CoreFW.Domain`)** - Xem chi tiết: [country-management-domain-plan.md](./country-management-domain-plan.md)
- [x] **Bước 2: Tầng Domain.Shared (`Aqt.CoreFW.Domain.Shared`)** - Xem chi tiết: [country-management-domain-shared-plan.md](./country-management-domain-shared-plan.md)
- [x] **Bước 3: Tầng Application.Contracts (`Aqt.CoreFW.Application.Contracts`)** - Xem chi tiết: [country-management-app-contracts-plan.md](./country-management-app-contracts-plan.md)
- [x] **Bước 4: Tầng Application (`Aqt.CoreFW.Application`)** - Xem chi tiết: [country-management-application-plan.md](./country-management-application-plan.md)
- [x] **Bước 5: Tầng EntityFrameworkCore (`Aqt.CoreFW.EntityFrameworkCore`)** - Xem chi tiết: [country-management-efcore-plan.md](./country-management-efcore-plan.md)
- [x] **Bước 6: Tầng Web (`Aqt.CoreFW.Web`)** - Xem chi tiết: [country-management-web-plan.md](./country-management-web-plan.md)
- [ ] **Bước 7: Các bước triển khai và kiểm thử cuối cùng (Thực hiện thủ công)** - Xem chi tiết: [country-management-final-steps-plan.md](./country-management-final-steps-plan.md) 