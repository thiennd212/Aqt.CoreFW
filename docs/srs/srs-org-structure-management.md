# Tài liệu Đặc tả Yêu cầu Phần mềm (SRS) - Module Quản lý Cơ cấu Tổ chức

**Version:** 1.2
**Date:** 2024-07-26

**Change Log:**
*   **1.0 (2024-07-26):** Phiên bản ban đầu.
*   **1.1 (2024-07-26):** Cập nhật để phản ánh việc sử dụng Dynamic HTTP API của ABP.
*   **1.2 (2024-07-26):** Bổ sung trường `MaLienThong` và `DiaChi` cho `OrganizationUnit` sử dụng ABP Object Extension System.

---

## 1. Giới thiệu

### 1.1 Mục đích
Tài liệu này đặc tả các yêu cầu chức năng và phi chức năng cho module "Quản lý Cơ cấu Tổ chức". Module này cho phép quản trị viên hệ thống và những người dùng được phân quyền quản lý các đơn vị/phòng ban (bao gồm thông tin mở rộng như Mã liên thông, Địa chỉ), chức vụ, thông tin người dùng mở rộng, và quan trọng nhất là quản lý vị trí công tác của người dùng (bao gồm đơn vị, chức vụ và vai trò tương ứng). Module được xây dựng trên nền tảng ABP Framework, tái sử dụng và mở rộng các thành phần cốt lõi, sử dụng tính năng Dynamic HTTP API và tuân thủ các đặc thù của domain chính phủ Việt Nam.

### 1.2 Phạm vi
Module bao gồm các chức năng sau:
*   Quản lý danh mục Chức vụ (CRUD).
*   Quản lý thông tin mở rộng của Người dùng (UserProfile) liên kết với tài khoản ABP User (CRUD).
*   Quản lý danh mục Đơn vị/Phòng ban (`OrganizationUnit`) dưới dạng cây phân cấp, bao gồm các thông tin chuẩn và thông tin mở rộng (`MaLienThong`, `DiaChi`) (CRUD, sử dụng thành phần và hệ thống mở rộng của ABP).
*   Quản lý Vị trí công tác: Gán người dùng vào một hoặc nhiều đơn vị với chức vụ và vai trò (Role) cụ thể.
*   Cho phép người dùng đăng nhập và chọn một vị trí công tác mặc định hoặc chuyển đổi giữa các vị trí được gán.
*   Đồng bộ hóa thông tin vai trò (Role) từ các vị trí công tác vào tài khoản người dùng ABP (`AbpIdentityUserRole`) để hỗ trợ kiểm tra quyền tổng quát.
*   Phân quyền chi tiết cho các chức năng quản lý.
*   Các chức năng nghiệp vụ được cung cấp thông qua Application Services và tự động expose thành API bởi Dynamic HTTP API.

### 1.3 Định nghĩa, Từ viết tắt
*   **ABP:** ABP Framework - Một framework mã nguồn mở cho việc xây dựng ứng dụng web hiện đại dựa trên ASP.NET Core.
*   **User (AbpUser):** Thực thể người dùng cơ bản được cung cấp bởi module Identity của ABP, lưu trữ thông tin đăng nhập.
*   **Role (AbpRole):** Thực thể vai trò (nhóm quyền) được cung cấp bởi module Identity của ABP.
*   **OrganizationUnit (OU):** Thực thể Đơn vị/Phòng ban được cung cấp bởi module OrganizationUnits của ABP, hỗ trợ cấu trúc cây.
*   **UserProfile:** Thực thể tùy chỉnh (Custom Entity) lưu trữ thông tin mở rộng của người dùng (Họ tên, SĐT, Địa chỉ, Ảnh...) liên kết 1-1 với AbpUser.
*   **JobTitle:** Thực thể tùy chỉnh lưu trữ thông tin Chức vụ (Tên chức vụ, Mã, Mô tả...).
*   **Position:** Thực thể tùy chỉnh đại diện cho một vị trí công tác cụ thể, là sự kết hợp của User, OrganizationUnit, và JobTitle.
*   **PositionRole:** Bảng trung gian (Join Entity) liên kết một Position với một hoặc nhiều Role, xác định quyền hạn cho vị trí đó.
*   **Primary Position:** Vị trí công tác được đánh dấu là mặc định cho người dùng khi đăng nhập hoặc khi không chọn vị trí cụ thể (nếu có nhiều vị trí).
*   **Active Position:** Vị trí công tác mà người dùng đang chọn để làm việc trong phiên đăng nhập hiện tại.
*   **SRS:** Software Requirements Specification - Đặc tả Yêu cầu Phần mềm.
*   **CRUD:** Create, Read, Update, Delete - Các thao tác cơ bản với dữ liệu.
*   **UI:** User Interface - Giao diện người dùng.
*   **API:** Application Programming Interface - Giao diện lập trình ứng dụng.
*   **DTO:** Data Transfer Object - Đối tượng dùng để truyền dữ liệu giữa các lớp.
*   **Dynamic HTTP API:** Tính năng của ABP Framework tự động tạo các HTTP API endpoint cho các Application Services.
*   **Object Extension System (Extra Properties):** Hệ thống của ABP cho phép thêm thuộc tính tùy chỉnh vào các thực thể có sẵn mà không cần sửa đổi mã nguồn gốc.
*   **MaLienThong:** Mã định danh dùng cho việc liên thông, trao đổi dữ liệu giữa các hệ thống (thường trong khối chính phủ).
*   **DiaChi:** Địa chỉ của Đơn vị/Phòng ban.

### 1.4 Tài liệu tham khảo
*   ABP Framework Documentation (abp.io) - bao gồm Dynamic HTTP API & Object Extension System
*   .NET Documentation (microsoft.com)
*   LeptonX Lite Theme Documentation
*   Phân tích chi tiết Use Case Quản lý cơ cấu tổ chức (từ các trao đổi trước)

---

## 2. Mô tả Tổng thể

### 2.1 Bối cảnh Sản phẩm
Module Quản lý Cơ cấu Tổ chức là một phần cốt lõi của hệ thống ứng dụng tổng thể. Nó cung cấp nền tảng quản lý thông tin người dùng, cấu trúc phòng ban và vị trí công tác, làm cơ sở cho việc phân quyền và xác định ngữ cảnh nghiệp vụ cho các module khác trong hệ thống. Module này tích hợp chặt chẽ và mở rộng các tính năng của ABP Identity và OrganizationUnits.

### 2.2 Chức năng Sản phẩm (Tổng quan)
*   Quản lý danh mục Chức vụ.
*   Quản lý thông tin chi tiết Người dùng.
*   Quản lý cấu trúc Đơn vị/Phòng ban dạng cây (bao gồm Mã liên thông, Địa chỉ).
*   Quản lý việc gán Người dùng vào các Vị trí công tác (OU + Chức vụ) kèm theo Vai trò.
*   Xử lý logic đăng nhập và chuyển đổi Vị trí công tác cho Người dùng.
*   Đồng bộ Vai trò tổng hợp của Người dùng vào hệ thống ABP Identity.
*   Phân quyền quản trị và sử dụng các chức năng.
*   Cung cấp các chức năng này thông qua Application Services được expose tự động thành API.

### 2.3 Đặc điểm Người dùng
*   **Quản trị hệ thống (System Administrator):** Có toàn quyền cấu hình, quản lý tất cả các danh mục (Chức vụ, OU), quản lý Người dùng, Vị trí và phân quyền.
*   **Quản trị viên đơn vị (OU Manager - Tùy chọn):** Có thể được phân quyền quản lý Người dùng và Vị trí trong phạm vi Đơn vị/Phòng ban mình quản lý.
*   **Người dùng cuối (End User):** Xem thông tin cá nhân, xem các vị trí công tác được gán, chọn/chuyển đổi vị trí công tác đang hoạt động, thực hiện các chức năng nghiệp vụ khác dựa trên quyền hạn của vị trí đang hoạt động.

### 2.4 Các ràng buộc
*   **Công nghệ:** Hệ thống được phát triển bằng .NET (phiên bản cụ thể sẽ được xác định), ABP Framework, Entity Framework Core. Giao diện người dùng là MVC Razor Pages với theme LeptonX Lite. Cơ sở dữ liệu là SQL Server (hoặc CSDL quan hệ tương thích khác).
*   **Tái sử dụng ABP:** Phải tái sử dụng tối đa các thực thể và dịch vụ cốt lõi của ABP: AbpIdentityUser, AbpIdentityRole, OrganizationUnit, UserManager, RoleManager, OrganizationUnitManager, hệ thống phân quyền (Permissions).
*   **Mở rộng ABP Entity:** Việc bổ sung các trường tùy chỉnh (`MaLienThong`, `DiaChi`) cho thực thể `OrganizationUnit` **phải** được thực hiện thông qua **ABP Object Extension System (Extra Properties)** để đảm bảo tính tương thích và khả năng nâng cấp.
*   **Kiến trúc:** Sử dụng tính năng Dynamic HTTP API của ABP Framework để tự động expose các Application Services thành các HTTP API endpoint, giảm thiểu việc viết code API Controller thủ công.
*   **Cơ sở dữ liệu:** Thiết lập đầy đủ khóa ngoại (Foreign Keys), ràng buộc duy nhất (Unique Constraints) cho: UserProfile.UserId, JobTitle.MaChucVu (nếu có), Position(UserId, OrganizationUnitId, JobTitleId) (khuyến nghị), PositionRole(PositionId, RoleId). Đảm bảo các trường bắt buộc là Not Null. Nên sử dụng ISoftDelete cho các thực thể tùy chỉnh. Các thuộc tính mở rộng của `OrganizationUnit` sẽ được lưu trữ trong cột `ExtraProperties` (thường là JSON) của bảng `AbpOrganizationUnits`.
*   **Nghiệp vụ:** Mỗi người dùng chỉ có tối đa một vị trí được đánh dấu là Chính (IsPrimary=true). Phải có logic đồng bộ Role từ Position vào AbpIdentityUserRole. Phải xử lý rõ ràng trường hợp người dùng đăng nhập mà không có vị trí nào.
*   **Hiệu năng:** Hệ thống phải đáp ứng tốt với quy mô ~10.000 người dùng và dữ liệu liên quan (Positions, PositionRoles). Các truy vấn, đặc biệt là lúc đăng nhập, chuyển vị trí, hiển thị cây OU phải được tối ưu. Cần xem xét hiệu năng truy vấn trên các trường Extra Properties nếu có lọc/tìm kiếm thường xuyên.
*   **Bảo mật:** Tuân thủ các nguyên tắc bảo mật của ABP Framework. Phân quyền chi tiết dựa trên hệ thống Permission của ABP.
*   **UI:** Giao diện phải tuân thủ theme LeptonX Lite, thân thiện, dễ sử dụng.

### 2.5 Giả định và Phụ thuộc
*   ABP Framework đã được cài đặt và cấu hình cơ bản, bao gồm cả Dynamic HTTP API và Object Extension System.
*   Cơ sở dữ liệu đã được thiết lập.
*   Theme LeptonX Lite đã được tích hợp.
*   Các module phụ thuộc (Identity, OrganizationUnits) của ABP đã được cài đặt.

---

## 3. Yêu cầu Cụ thể

### 3.1 Yêu cầu Chức năng
*Các yêu cầu chức năng dưới đây sẽ được thực hiện chủ yếu thông qua các Application Services. Các service này sẽ tự động được expose thành HTTP API bởi ABP.*

**FR-001: Quản lý Chức vụ (Job Titles)**
*   **Mô tả:** Hệ thống cho phép Quản trị viên thực hiện các thao tác CRUD với danh mục Chức vụ.
*   **Chi tiết:**
    *   Thêm mới Chức vụ với các thông tin: Tên chức vụ (bắt buộc), Mã chức vụ (tùy chọn, duy nhất), Mô tả.
    *   Xem danh sách Chức vụ (phân trang, tìm kiếm, sắp xếp).
    *   Xem chi tiết một Chức vụ.
    *   Cập nhật thông tin Chức vụ.
    *   Xóa Chức vụ (ưu tiên Soft Delete).
*   **Service:** `JobTitleAppService`

**FR-002: Quản lý Thông tin Người dùng (User Profiles)**
*   **Mô tả:** Hệ thống cho phép quản lý thông tin mở rộng liên kết với tài khoản người dùng ABP.
*   **Chi tiết:**
    *   Khi tạo AbpUser, hệ thống có thể tự động tạo UserProfile liên kết (hoặc tạo khi người dùng cập nhật lần đầu).
    *   Người dùng có thể tự cập nhật thông tin UserProfile của mình (Họ tên, SĐT, Địa chỉ, Ảnh đại diện...).
    *   Quản trị viên có thể xem và cập nhật UserProfile của người dùng khác.
    *   Đảm bảo mối liên kết 1-1 duy nhất giữa UserProfile và AbpUser.
*   **Service:** `UserProfileAppService` / Tích hợp vào Quản lý Identity

**FR-003: Quản lý Đơn vị/Phòng ban (Organization Units)**
*   **Mô tả:** Hệ thống cho phép Quản trị viên quản lý cấu trúc đơn vị/phòng ban dạng cây, sử dụng thành phần `OrganizationUnit` của ABP và mở rộng bằng các thuộc tính tùy chỉnh.
*   **Chi tiết:**
    *   Thêm mới Đơn vị với các thông tin: Tên hiển thị (bắt buộc), Đơn vị cha (tùy chọn), Mã OU (Code - ABP chuẩn), **Mã liên thông** (`MaLienThong` - tùy chỉnh, kiểu chuỗi, có thể yêu cầu duy nhất hoặc không tùy nghiệp vụ), **Địa chỉ** (`DiaChi` - tùy chỉnh, kiểu chuỗi).
    *   Xem cấu trúc cây Đơn vị (có thể tùy chọn hiển thị thêm Mã liên thông).
    *   Xem danh sách Đơn vị (dạng phẳng hoặc cây, hiển thị các thông tin chuẩn và tùy chỉnh).
    *   Xem chi tiết một Đơn vị, bao gồm cả `MaLienThong` và `DiaChi`.
    *   Cập nhật thông tin Đơn vị, bao gồm cả `MaLienThong` và `DiaChi`.
    *   Xóa Đơn vị (ABP hỗ trợ Soft Delete). Xử lý các Position liên quan khi xóa OU.
    *   Giao diện hiển thị dạng cây thân thiện.
*   **Service:** `OrganizationUnitAppService` (ABP có sẵn hoặc mở rộng). Các DTOs liên quan (vd: `OrganizationUnitDto`, `CreateOrganizationUnitDto`, `UpdateOrganizationUnitDto`) cần được cập nhật để bao gồm `MaLienThong` và `DiaChi`. Việc đọc/ghi các thuộc tính này sẽ thông qua cơ chế `ExtraProperties` của ABP.

**FR-004: Quản lý Vị trí công tác (Positions)**
*   **Mô tả:** Hệ thống cho phép Quản trị viên gán người dùng vào các vị trí công tác và quản lý các vị trí này.
*   **Chi tiết:**
    *   Thêm mới Vị trí: Chọn Người dùng (AbpUser), chọn Đơn vị (OU từ cây), chọn Chức vụ (JobTitle từ danh sách).
    *   Gán Vai trò (AbpRole): Chọn một hoặc nhiều Role từ danh sách Role của hệ thống để gán cho Vị trí vừa tạo (lưu vào PositionRole).
    *   Đánh dấu Vị trí là Chính (IsPrimary): Cho phép đặt một vị trí làm mặc định cho người dùng (đảm bảo chỉ có 1 vị trí chính/người dùng).
    *   Xem danh sách Vị trí của một Người dùng (hiển thị OU, Chức vụ).
    *   Xem danh sách Người dùng thuộc một OU (có thể lọc theo Chức vụ).
    *   Cập nhật Vai trò đã gán cho một Vị trí.
    *   Xóa một Vị trí của người dùng (Xóa bản ghi Position và các bản ghi PositionRole liên quan). Khi xóa phải kích hoạt đồng bộ Role lại cho User.
*   **Service:** `PositionAppService`

**FR-005: Đồng bộ hóa Vai trò Người dùng (Role Synchronization)**
*   **Mô tả:** Hệ thống tự động cập nhật danh sách vai trò tổng hợp của người dùng trong `AbpIdentityUserRole` dựa trên tất cả các vai trò được gán cho các vị trí của họ.
*   **Chi tiết:**
    *   **Trigger:** Sau khi thêm mới Position, xóa Position, hoặc cập nhật danh sách Role của một Position.
    *   **Logic (Hàm `UpdateAbpUserRoles(userId)` trong Domain/Application Service):**
        1.  Lấy tất cả các `Position` của `userId`.
        2.  Lấy danh sách duy nhất (distinct) tất cả các `RoleId` từ bảng `PositionRole` liên quan đến các `Position` trên.
        3.  Lấy danh sách `Role` hiện tại của `userId` từ `AbpIdentityUserRole` (qua `UserManager`).
        4.  So sánh hai danh sách, xác định Role cần thêm và Role cần xóa khỏi `AbpIdentityUserRole`.
        5.  Gọi `UserManager.AddToRolesAsync` và `UserManager.RemoveFromRolesAsync` để cập nhật.
*   **Service:** Logic nội bộ được kích hoạt bởi `PositionAppService` / Domain Service

**FR-006: Đăng nhập và Chọn Vị trí**
*   **Mô tả:** Hệ thống xử lý việc chọn vị trí công tác khi người dùng đăng nhập.
*   **Chi tiết:**
    *   Sau khi xác thực thành công:
        *   Nếu người dùng không có Position nào: Đăng nhập bình thường, quyền hạn có thể bị giới hạn.
        *   Nếu người dùng có 1 Position: Tự động chọn Position đó làm Active Position. Lấy Roles từ Position đó để xác định quyền hạn cho phiên làm việc.
        *   Nếu người dùng có >1 Position: Hiển thị giao diện cho phép người dùng chọn 1 Position để làm việc (có thể gợi ý vị trí Primary). Sau khi chọn, lấy Roles của Position đó để xác định quyền hạn.
    *   Thông tin Active Position (Id, Tên OU, Tên Chức vụ) cần được lưu trữ trong context của phiên làm việc (ví dụ: Claims Principal).
*   **Service:** Logic đăng nhập / Authentication handler / Có thể có phương thức AppService riêng

**FR-007: Chuyển đổi Vị trí công tác**
*   **Mô tả:** Người dùng có nhiều hơn một vị trí có thể chuyển đổi vị trí đang hoạt động trong phiên làm việc.
*   **Chi tiết:**
    *   Giao diện người dùng (ví dụ: menu dropdown ở header) hiển thị danh sách các Position khác của người dùng.
    *   Khi người dùng chọn một Position khác:
        1.  Hệ thống cập nhật Active Position trong context phiên làm việc (có thể gọi một API để thực hiện).
        2.  Lấy danh sách Role tương ứng với Position mới.
        3.  Cập nhật lại Claims Principal hiện tại của người dùng để phản ánh bộ Role mới. Các kiểm tra quyền sau đó sẽ dựa trên bộ Role này.
*   **Service:** `PositionAppService` (ví dụ: phương thức `SwitchActivePosition`)

**FR-008: Phân quyền Chức năng**
*   **Mô tả:** Hệ thống sử dụng cơ chế phân quyền của ABP để kiểm soát truy cập các chức năng quản lý thông qua các Application Services.
*   **Chi tiết:**
    *   Định nghĩa các Permissions mới (ví dụ: `OrganizationStructure.JobTitles.Management`, `OrganizationStructure.Positions.Management`, `OrganizationStructure.OrganizationUnits.Management`).
    *   Gán các Permission này cho các Role phù hợp (ví dụ: Role 'Admin').
    *   Áp dụng kiểm tra quyền bên trong các phương thức của Application Services hoặc sử dụng `[Authorize]` attribute trên các phương thức đó. Dynamic API sẽ tôn trọng các kiểm tra quyền này.
*   **Service:** Áp dụng trên các AppServices liên quan

### 3.2 Yêu cầu Phi chức năng

**NFR-001: Hiệu năng**
*   Thời gian phản hồi cho thao tác đăng nhập và chọn vị trí (nếu có): < 2 giây.
*   Thời gian tải danh sách (Users, Positions, JobTitles, OUs) có phân trang thông qua API động: < 3 giây.
*   Thời gian tải và hiển thị cây OU ban đầu (dữ liệu lấy qua API động): < 3 giây (cần cache).
*   Tối ưu hóa truy vấn CSDL: sử dụng indexing hiệu quả, projection (`Select`), tránh N+1 query, sử dụng `AsNoTracking` cho truy vấn chỉ đọc. Cần đánh giá hiệu năng truy vấn trên các trường Extra Properties (MaLienThong, DiaChi) nếu dùng để lọc hoặc tìm kiếm thường xuyên và cân nhắc tạo index phù hợp trên cột JSON nếu CSDL hỗ trợ.
*   Khả năng chịu tải: Hệ thống hoạt động ổn định với ~10.000 người dùng và dữ liệu liên quan.

**NFR-002: Khả năng mở rộng (Scalability)**
*   Kiến trúc module cho phép mở rộng trong tương lai về số lượng người dùng và dữ liệu mà không cần thiết kế lại cơ bản. Sử dụng các giải pháp cache phân tán (như Redis thông qua `IDistributedCache`) nếu triển khai trên nhiều instance.

**NFR-003: Độ tin cậy (Reliability)**
*   Đảm bảo tính toàn vẹn dữ liệu thông qua các ràng buộc CSDL và logic nghiệp vụ. Sử dụng Unit of Work của ABP để đảm bảo các thao tác liên quan được thực hiện trong cùng một transaction. Xử lý lỗi và ngoại lệ một cách tường minh trong Application Services.

**NFR-004: Tính khả dụng (Usability)**
*   Giao diện người dùng (MVC Razor Pages + LeptonX Lite) phải trực quan, dễ sử dụng. Các thao tác quản lý (thêm/sửa/xóa) và chuyển đổi vị trí phải rõ ràng, thuận tiện.

**NFR-005: Khả năng bảo trì (Maintainability)**
*   Code tuân thủ các nguyên tắc SOLID và các best practice của ABP Framework. Cấu trúc code rõ ràng, phân tách các lớp (Domain, Application, Infrastructure, UI). Có comment giải thích các đoạn code phức tạp nếu cần. Việc sử dụng Object Extension System giúp tăng khả năng bảo trì khi nâng cấp ABP.

**NFR-006: Bảo mật (Security)**
*   Tận dụng đầy đủ các tính năng bảo mật của ABP: Authentication, Authorization (áp dụng trên Application Service methods), Anti-Forgery, Input Validation (DTO validation). Đảm bảo chỉ người dùng có quyền mới thực hiện được các thao tác quản lý. Dữ liệu nhạy cảm (nếu có) phải được xử lý an toàn.

### 3.3 Yêu cầu Giao diện (Interface Requirements)

*   **Giao diện người dùng (UI):**
    *   Sử dụng MVC Razor Pages với theme LeptonX Lite.
    *   Cần có các thành phần UI chuẩn như bảng dữ liệu (phân trang, sắp xếp, tìm kiếm), form nhập liệu (có validation), modal dialog, component hiển thị cây (Tree view) cho OU, dropdown/select list, khu vực hiển thị thông tin người dùng và cho phép chuyển đổi vị trí.
    *   **Cập nhật UI cho OU:** Form nhập liệu/cập nhật Đơn vị/Phòng ban cần có thêm trường để nhập `MaLienThong` và `DiaChi`. Bảng/cây hiển thị danh sách OU cần hiển thị các trường này.
    *   Giao diện sẽ tương tác với backend thông qua các API được tạo tự động.
*   **Giao diện lập trình ứng dụng (API):**
    *   Các chức năng nghiệp vụ được cung cấp bởi các Application Services (ví dụ: `JobTitleAppService`, `UserProfileAppService`, `PositionAppService`, `OrganizationUnitAppService`) sẽ được tự động expose thành các HTTP API endpoint theo quy ước bởi tính năng Dynamic HTTP API của ABP Framework.
    *   **Cập nhật API cho OU:** Các DTOs sử dụng bởi `OrganizationUnitAppService` phải được cập nhật để chứa `MaLienThong` và `DiaChi`. Application Service sẽ xử lý việc mapping giữa DTO và `ExtraProperties` của entity `OrganizationUnit`.
    *   Các Razor Pages sẽ tương tác với các API này thông qua các proxy JavaScript được tạo tự động bởi ABP hoặc thông qua các lời gọi AJAX trực tiếp đến các endpoint quy ước.
    *   Sử dụng DTOs để truyền dữ liệu giữa client (Razor Pages) và server (Application Services).

### 3.4 Yêu cầu Dữ liệu (Data Requirements)

*   **Mô hình Thực thể Dữ liệu:**
    *   Sử dụng các thực thể ABP: `AbpIdentityUser`, `AbpIdentityRole`, `AbpOrganizationUnit`.
    *   **Mở rộng `AbpOrganizationUnit`:** Các trường `MaLienThong` (string, nullable) và `DiaChi` (string, nullable) sẽ được thêm vào `AbpOrganizationUnit` thông qua **ABP Object Extension System**. Chúng sẽ được lưu trữ trong cột `ExtraProperties` (JSON) của bảng `AbpOrganizationUnits` trong cơ sở dữ liệu.
    *   Định nghĩa các thực thể tùy chỉnh: `AppUserProfile`, `AppJobTitle`, `AppPosition`, `AppPositionRole` (bảng nối).
    *   **Thực thể:**
        *   `AbpIdentityUser` (ABP)
        *   `AbpIdentityRole` (ABP)
        *   `AbpOrganizationUnit` (ABP, *được mở rộng với MaLienThong, DiaChi*)
        *   `AppUserProfile` (Custom)
        *   `AppJobTitle` (Custom)
        *   `AppPosition` (Custom)
        *   `AppPositionRole` (Custom)
*   **Mối quan hệ:**
    *   UserProfile 1--1 User
    *   Position }o--1 User
    *   Position }o--1 OrganizationUnit
    *   Position }o--1 JobTitle
    *   Position ||--o{ Role (thông qua PositionRole)
*   **Chuyển đổi/Khởi tạo Dữ liệu:** Cần có kế hoạch hoặc công cụ để nhập dữ liệu ban đầu (nếu có) cho OU, JobTitle, và ánh xạ User hiện có (nếu hệ thống cũ được nâng cấp). Cần xem xét việc nhập dữ liệu cho `MaLienThong` và `DiaChi` nếu có.