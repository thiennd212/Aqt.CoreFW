$(function () {
    var l = abp.localization.getResource('CoreFW');
    // Đảm bảo namespace của service proxy là chính xác
    var workflowStatusAppService = aqt.coreFW.application.workflowStatuses.workflowStatus;

    var createModal = new abp.ModalManager(abp.appPath + 'WorkflowStatuses/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'WorkflowStatuses/EditModal');

    var dataTable = null; // Khai báo dataTable ở phạm vi ngoài để có thể truy cập từ các hàm khác

    // Hàm để lấy các giá trị filter
    var getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            // Kiểm tra giá trị rỗng trước khi chuyển đổi sang boolean
            isActive: $('#IsActiveFilter').val() === "" ? null : ($('#IsActiveFilter').val() === 'true')
        };
    };

    // Hàm khởi tạo hoặc khởi tạo lại DataTable
    function initializeDataTable() {
        // Hủy DataTable cũ nếu đã tồn tại để tránh lỗi
        if (dataTable) {
            dataTable.destroy();
        }

        dataTable = $('#WorkflowStatusesTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[1, "asc"]], // Sắp xếp theo cột 'Code' mặc định
                searching: false,    // Tắt tính năng tìm kiếm tích hợp của DataTables, dùng filter riêng
                scrollX: true,       // Bật cuộn ngang nếu cần
                ajax: abp.libs.datatables.createAjax(workflowStatusAppService.getList, getFilterInputs), // Sử dụng hàm lấy filter
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'),
                                    icon: "fa fa-pencil-alt", // Thêm icon
                                    visible: permissions.canEdit, // Truy cập trực tiếp biến permissions
                                    action: (data) => {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fa fa-trash", // Thêm icon
                                    visible: permissions.canDelete, // Truy cập trực tiếp biến permissions
                                    confirmMessage: (data) => l('AreYouSureToDeleteWorkflowStatus', data.record.name || data.record.code),
                                    action: (data) => {
                                        workflowStatusAppService.delete(data.record.id)
                                            .then(() => {
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload(); // Tải lại dữ liệu bảng
                                            })
                                            .catch((error) => {
                                                // Hiển thị thông báo lỗi chi tiết hơn nếu có
                                                let message = error.message || l('ErrorOccurred');
                                                if (error.details) {
                                                    message += "\\n" + error.details;
                                                }
                                                abp.notify.error(message);
                                            });
                                    }
                                }
                            ]
                        }
                    },
                    { title: l('WorkflowStatusCode'), data: "code" },
                    { title: l('WorkflowStatusName'), data: "name" },
                    { title: l('WorkflowStatusDescription'), data: "description", orderable: false }, // Mô tả thường không cần sắp xếp
                    { title: l('WorkflowStatusOrder'), data: "order" },
                    {
                        title: l('WorkflowStatusColorCode'), data: "colorCode",
                        // Render màu sắc để dễ nhìn
                        render: (data) => data ? `<span style="display:inline-block; width:15px; height:15px; background-color:${data}; margin-right: 5px; border: 1px solid #ccc; vertical-align: middle;"></span> ${data}` : '',
                        orderable: false // Mã màu thường không cần sắp xếp
                    },
                    {
                        title: l('WorkflowStatusIsActive'), data: "isActive",
                        // Sử dụng badge để hiển thị trạng thái Active/Inactive rõ ràng
                        render: (data) => `<span class="badge ${data ? 'bg-success' : 'bg-secondary'}">${data ? l('Active') : l('Inactive')}</span>`
                    }
                ]
            })
        );
    }

    initializeDataTable(); // Gọi hàm để khởi tạo bảng lần đầu

    // --- Gắn các sự kiện ---
    // Sử dụng .on() để gắn sự kiện, nhất quán với JobTitles
    $('#SearchButton').on('click', () => dataTable.ajax.reload());

    // Cho phép tìm kiếm khi nhấn Enter trong ô filter
    $('#SearchFilter').on('keypress', (e) => {
        if (e.which === 13) { // 13 là mã phím Enter
            dataTable.ajax.reload();
        }
    });

    // Tải lại bảng khi thay đổi bộ lọc trạng thái
    $('#IsActiveFilter').on('change', () => dataTable.ajax.reload());

    // Mở modal tạo mới khi nhấn nút
    $('#NewWorkflowStatusButton').on('click', (e) => {
        e.preventDefault(); // Ngăn hành vi mặc định của nút (nếu có)
        createModal.open();
    });

    // Xử lý sau khi modal được đóng và có kết quả (thành công)
    createModal.onResult(() => {
        dataTable.ajax.reload(); // Tải lại dữ liệu
        abp.notify.success(l('SuccessfullySaved')); // Thông báo thành công
    });

    editModal.onResult(() => {
        dataTable.ajax.reload();
        abp.notify.success(l('SuccessfullySaved'));
    });

    // Xử lý nút Xuất Excel (Giữ nguyên logic như trước)
    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        // 1. Lấy filter hiện tại
        const filterInput = getFilterInputs();

        // 2. Lấy sắp xếp từ DataTable (nếu có)
        const sortInfo = dataTable ? dataTable.order()[0] : null;
        let sorting = '';
        if (sortInfo) {
            const columnIndex = sortInfo[0];
            const sortDirection = sortInfo[1];
            const columnName = dataTable.settings().init().columnDefs[columnIndex]?.data;
            if (columnName) {
                sorting = columnName + ' ' + sortDirection;
            }
        }

        // 3. Tạo query string từ filterInput
        const params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.isActive !== null && filterInput.isActive !== undefined) {
            params.append('IsActive', filterInput.isActive);
        }
        if (sorting) {
            params.append('Sorting', sorting);
        }

        // 4. Tạo URL API export
        const exportUrl = abp.appPath + 'api/app/workflow-status/as-excel?' + params.toString();

        // 5. Tải file bằng location.href
        location.href = exportUrl;
    });

});