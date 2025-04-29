$(function () {
    let l = abp.localization.getResource('CoreFW');
    // Đảm bảo proxy namespace là chính xác sau khi build
    let bDocumentService = aqt.coreFW.application.bDocuments.bDocument;

    // Khởi tạo Modal Manager cho Edit
    let editModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'BDocuments/EditModal',
        scriptUrl: abp.appPath + 'Pages/BDocuments/editModal.js', // Kiểm tra lại đường dẫn JS
        modalClass: 'editBDocument' // Class để CSS tùy chỉnh nếu cần
    });

    // Khởi tạo Modal Manager cho Create
    let createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'BDocuments/CreateModal', // Đường dẫn đến CreateModal.cshtml
        scriptUrl: abp.appPath + 'Pages/BDocuments/createModal.js', // Đường dẫn đến JS của CreateModal
        modalClass: 'createBDocument'
    });

    let dataTable = null; // Biến lưu instance của DataTable

    // Hàm helper để lấy các giá trị filter hiện tại
    let getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(), // Filter text chung
            procedureId: $('#ProcedureFilter').val() || null, // Filter theo ProcedureId từ dropdown
            statusId: $('#StatusFilter').val() || null // Filter theo StatusId từ dropdown
        };
    };

    // Hàm khởi tạo hoặc tải lại DataTable
    function initializeDataTable() {
        // Hủy DataTable cũ nếu đã tồn tại
        if (dataTable) {
            dataTable.destroy();
            dataTable = null; // Reset biến
        }

        // Khởi tạo DataTable mới
        dataTable = $('#BDocumentTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true, // Bật chế độ server-side
                paging: true, // Bật phân trang
                order: [[5, "desc"]], // Sắp xếp mặc định theo cột CreationTime (index 5) giảm dần
                searching: false, // Tắt searching mặc định của DataTable (vì dùng filter riêng)
                scrollX: true, // Bật scroll ngang nếu bảng quá rộng
                ajax: abp.libs.datatables.createAjax(bDocumentService.getList, getFilterInputs), // Gọi API getList với filter
                columnDefs: [ // Định nghĩa các cột
                    {
                        title: l('Actions'), // Tiêu đề cột Actions
                        rowAction: { // Cấu hình các nút action cho từng dòng
                            items: [
                                // Nút Edit
                                {
                                    text: l('Edit'),
                                    icon: "fas fa-pencil-alt",
                                    // Chỉ hiển thị nếu có quyền Update
                                    visible: permissions.canEdit,
                                    // Hành động khi click: Mở EditModal với ID của dòng
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                // Nút Delete
                                {
                                    text: l('Delete'),
                                    icon: "fas fa-trash",
                                    // Chỉ hiển thị nếu có quyền Delete
                                    visible: permissions.canDelete,
                                    // Hiển thị thông báo xác nhận trước khi xóa
                                    confirmMessage: (data) => l('AreYouSureToDeleteBDocument', data.record.maHoSo || data.record.id),
                                    // Hành động khi click: Gọi API Delete và reload bảng
                                    action: (data) => {
                                        bDocumentService.delete(data.record.id)
                                            .then(() => {
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload(); // Tải lại dữ liệu bảng
                                            });
                                    }
                                }
                                // Có thể thêm các action khác ở đây (View, GenerateFile, etc.)
                            ]
                        },
                        width: "100px" // Giảm độ rộng cột Actions
                    },
                    // Các cột dữ liệu khác
                    { title: l('DisplayName:BDocument.MaHoSo'), data: "maHoSo", width: "150px" },
                    { title: l('DisplayName:BDocument.TenChuHoSo'), data: "tenChuHoSo" },
                    { title: l('DisplayName:BDocument.ProcedureId'), data: "procedureName", width: "250px" }, // Hiển thị tên Procedure
                    {
                        title: l('DisplayName:BDocument.TrangThaiHoSoId'), data: "trangThaiHoSoName", width: "150px",
                        // Render badge màu cho trạng thái
                        render: (data, type, row) => {
                            let color = row.trangThaiHoSoColorCode || 'secondary'; // Lấy mã màu, mặc định là secondary
                            let name = data || l('NotSet'); // Tên trạng thái hoặc 'Chưa đặt'
                            return `<span class="badge bg-${color}">${name}</span>`;
                        }
                    },
                    {
                        title: l('CreationTime'), data: "creationTime", width: "180px",
                        // Định dạng ngày giờ theo culture hiện tại
                        render: (data) => data ? luxon.DateTime.fromISO(data, { locale: abp.localization.currentCulture.name }).toLocaleString(luxon.DateTime.DATETIME_SHORT) : ''
                    }
                ]
            })
        );
    }

    // --- Event Handlers ---

    // Click nút Search hoặc Enter trong ô Search -> Reload DataTable
    $('#SearchButton').click(() => dataTable.ajax.reload());
    $('#SearchFilter').on('keypress', (e) => { if (e.which === 13) dataTable.ajax.reload(); });

    // Thay đổi giá trị trong dropdown filter -> Reload DataTable
    $('#ProcedureFilter, #StatusFilter').change(() => dataTable.ajax.reload());

    // Xử lý khi người dùng chọn một Thủ tục trong dropdown để tạo mới
    $('#ProcedureSelectionForCreate').on('change', function () {
        let selectedProcedureId = $(this).val();
        let canCreate = permissions.canCreate; // Lấy quyền tạo từ biến JS đã truyền
        // Bật/tắt nút "Thêm mới" dựa vào việc đã chọn thủ tục và có quyền tạo hay không
        $('#NewBDocumentButton').prop('disabled', !selectedProcedureId || !canCreate);
    });

    // Xử lý khi nhấn nút "Thêm mới"
    $('#NewBDocumentButton').on('click', function () {
        let selectedProcedureId = $('#ProcedureSelectionForCreate').val();
        if (selectedProcedureId) {
            // Mở CreateModal và truyền procedureId đã chọn vào
            createModal.open({ procedureId: selectedProcedureId });
        } else {
            // Thông báo nếu chưa chọn thủ tục (mặc dù nút đã bị disable)
            abp.notify.warn(l('PleaseSelectProcedureToCreate'));
        }
    });

    // Xử lý khi CreateModal trả về kết quả (sau khi tạo thành công)
    createModal.onResult(function () {
        dataTable.ajax.reload(); // Tải lại bảng dữ liệu
        abp.notify.success(l('SuccessfullyCreated')); // Hiển thị thông báo thành công
        $('#ProcedureSelectionForCreate').val(''); // Reset dropdown chọn thủ tục
        $('#NewBDocumentButton').prop('disabled', true); // Disable lại nút New
    });

    // Xử lý khi EditModal trả về kết quả (sau khi sửa thành công)
    editModal.onResult(() => {
        dataTable.ajax.reload(); // Tải lại bảng dữ liệu
        abp.notify.success(l('SuccessfullySaved')); // Hiển thị thông báo thành công
    });

    // Xử lý nút Export Excel
    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        let filterInput = getFilterInputs(); // Lấy các filter hiện tại

        // Lấy thông tin sắp xếp hiện tại từ DataTable
        let sortInfo = dataTable ? dataTable.order()[0] : null;
        let sorting = 'creationTime desc'; // Sắp xếp mặc định nếu không có
        if (sortInfo) {
            let columnIndex = sortInfo[0];
            let sortDirection = sortInfo[1];
            // Map index cột sang tên trường để sort (cần khớp với backend)
            let sortableMap = { 1: 'maHoSo', 2: 'tenChuHoSo', 3: 'procedureId', 4: 'trangThaiHoSoId', 5: 'creationTime' };
            let columnName = sortableMap[columnIndex];
            if (columnName) sorting = `${columnName} ${sortDirection}`;
        }

        // Tạo URLSearchParams để truyền filter và sorting
        let params = new URLSearchParams();
        if (filterInput.filter) params.append('FilterText', filterInput.filter); // Đổi tên param cho khớp API
        if (filterInput.procedureId) params.append('ProcedureId', filterInput.procedureId);
        if (filterInput.statusId) params.append('TrangThaiHoSoId', filterInput.statusId); // Đổi tên param cho khớp API
        if (sorting) params.append('Sorting', sorting);

        // Endpoint API để export (Cần tạo ở Application và HttpApi layers)
        let exportUrl = abp.appPath + 'api/app/b-document/as-excel-file?' + params.toString(); // API trả về File Content
        // Chuyển hướng trình duyệt đến URL để tải file
        location.href = exportUrl;
    });

    // --- Initialization ---
    initializeDataTable(); // Khởi tạo DataTable lần đầu
    // Kích hoạt sự kiện change ban đầu để kiểm tra trạng thái nút New
    $('#ProcedureSelectionForCreate').trigger('change');
});