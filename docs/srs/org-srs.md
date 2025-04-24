Quản lý đơn vị/phòng ban (OrganizationUnit):
- Mã
- Tên
- Trạng thái
- Thứ tự
- Mô tả
- Cấp cha
- Ngày đồng bộ gần nhất
- Id bản ghi đồng bộ
- Mã bản ghi đồng bộ

**Lưu ý:
- Tái sử dụng OrganizationUnit có sẵn của ABP Framework
- OrganizationUnitManager trong Volo.Abp.Identity, tham khảo https://abp.io/docs/api/abp/3.2/Volo.Abp.Identity.OrganizationUnitManager.html
- Module Entity Extensions tham khảo https://abp.io/docs/latest/framework/architecture/modularity/extending/module-entity-extensions#database-mapping
- Cấu hình MapEfCoreProperty trong CoreFWEfCoreEntityExtensionMappings để tạo các cột được thêm.
- Sử dụng jstree để hiển thị hình cây, tham khảo https://www.jstree.com/
- Tạo các quyền cho OrganizationUnit do Volo.Abp.Identity không định nghĩa quyền cho OrganizationUnit