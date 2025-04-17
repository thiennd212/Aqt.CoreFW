$(function () {
    var l = abp.localization.getResource('CoreFW');
    var jobTitleService = aqt.coreFW.application.jobTitles.jobTitle;

    var createModal = new abp.ModalManager(abp.appPath + 'JobTitles/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'JobTitles/EditModal');

    var dataTable = null; // Khai báo dataTable ở ngoài

    var getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            isActive: $('#IsActiveFilter').val() === "" ? null : ($('#IsActiveFilter').val().toLowerCase() === 'true')
        };
    };

    // Hàm khởi tạo hoặc khởi tạo lại DataTable
    function initializeDataTable() {
        // Hủy instance cũ nếu tồn tại để tránh lỗi
        if (dataTable) {
            dataTable.destroy();
        }

        // Sử dụng normalizeConfiguration để bao bọc toàn bộ cấu hình
        dataTable = $('#JobTitlesTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[1, "asc"]], // Mặc định sắp xếp theo cột thứ 2 (Code)
                searching: false, // Tắt searching mặc định
                scrollX: true,
                // ajax source được đặt bên trong cấu hình
                ajax: abp.libs.datatables.createAjax(jobTitleService.getList, getFilterInputs),
                // processing: true, // Thường được normalizeConfiguration xử lý
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items:
                            [
                                {
                                    text: l('Edit'),
                                    icon: "fa fa-pencil-alt",
                                    visible: permissions.canEdit,
                                    action: function (data) {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fa fa-trash",
                                    visible: permissions.canDelete,
                                    confirmMessage: function (data) {
                                        return l('AreYouSureToDeleteJobTitle', data.record.name || data.record.code);
                                    },
                                    action: function (data) {
                                        jobTitleService.delete(data.record.id)
                                            .then(function () {
                                                abp.notify.info(l('SuccessfullyDeleted'));
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
                        title: l('JobTitleCode'),
                        data: "code",
                        orderable: true
                    },
                    {
                        title: l('JobTitleName'),
                        data: "name",
                        orderable: true
                    },
                    {
                        title: l('JobTitleDescription'),
                        data: 'description',
                        orderable: false,
                        render: function (data, type, row) {
                             return data ? `<span title="${data}">${abp.utils.truncateStringWithPostfix(data, 50)}</span>` : '';
                        }
                    },
                    {
                        title: l('JobTitleIsActive'),
                        data: 'isActive',
                        orderable: true,
                        render: function (data) {
                            return data
                                ? '<i class="fa fa-check text-success"></i>'
                                : '<i class="fa fa-times text-danger"></i>';
                        }
                    }
                ]
            })
        );
    }

    initializeDataTable(); // Gọi hàm để khởi tạo lần đầu

    createModal.onResult(function () {
        dataTable.ajax.reload();
        abp.notify.success(l('SuccessfullyCreated'));
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
        abp.notify.success(l('SuccessfullyUpdated'));
    });

    $('#NewJobTitleButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    $('#SearchButton').click(function (e) {
        e.preventDefault();
        dataTable.ajax.reload(); // Chỉ cần reload, không cần initialize lại
    });

    $('#SearchFilter').on('keypress', function (e) { // Dùng .on() thay vì .keypress()
        if (e.which === 13) { // 13 is Enter key
            dataTable.ajax.reload();
            // return false; // Không cần return false ở đây
        }
    });

     $('#IsActiveFilter').change(function() {
         dataTable.ajax.reload();
    });

    // Xử lý sự kiện click nút Export Excel
    $('#ExportExcelButton').click(function (e) {
        e.preventDefault();
        // Lấy các tham số lọc hiện tại
        var filterInput = getFilterInputs();
        // Lấy thông tin sắp xếp từ DataTable (nếu cần thiết và DataTable đã được khởi tạo)
        var sortInfo = dataTable ? dataTable.order()[0] : null; // Lấy cột và hướng sắp xếp đầu tiên
        var sorting = '';
        if (sortInfo) {
            var columnIndex = sortInfo[0];
            var sortDirection = sortInfo[1];
            // Lấy tên cột từ cấu hình `columnDefs.data`. Cần đảm bảo cấu hình `data` tồn tại.
            var columnName = dataTable.settings().init().columnDefs[columnIndex].data;
            if (columnName) { // Chỉ thêm sorting nếu lấy được tên cột hợp lệ
                 sorting = columnName + ' ' + sortDirection;
            }
        }

        // Tạo URL cho endpoint export
        // Sử dụng service proxy để lấy URL đúng
        var url = abp.appPath + 'api/app/job-title/as-excel?'; // Giả định base path API

        // Thêm các tham số lọc và sắp xếp vào URL, chỉ thêm nếu có giá trị
        var params = [];
        if (filterInput.filter) {
            params.push('Filter=' + encodeURIComponent(filterInput.filter));
        }
        if (filterInput.isActive !== null) {
            params.push('IsActive=' + filterInput.isActive);
        }
        if (sorting) {
             params.push('Sorting=' + encodeURIComponent(sorting));
        }

        // Nối các tham số vào URL
        url += params.join('&');

        // Kích hoạt download
        location.href = url;
    });

});
