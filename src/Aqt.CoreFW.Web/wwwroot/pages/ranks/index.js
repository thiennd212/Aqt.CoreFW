$(function () {
    var l = abp.localization.getResource('CoreFW');
    // Namespace cần khớp với cấu trúc dự án của bạn
    var rankAppService = aqt.coreFW.application.ranks.rank;

    var createModal = new abp.ModalManager(abp.appPath + 'Ranks/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Ranks/EditModal');

    var dataTable = null;

    var getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val())
        };
    };

    function initializeDataTable() {
        if (dataTable) {
            dataTable.destroy();
        }

        dataTable = $('#RanksTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                // Sắp xếp mặc định theo kế hoạch: Order asc, Name asc
                order: [[3, "asc"], [2, "asc"]],
                searching: false,
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(rankAppService.getList, getFilterInputs),
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'),
                                    icon: "fas fa-pencil-alt",
                                    // Đọc quyền từ biến global 'permissions'
                                    visible: permissions.canEdit,
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fas fa-trash",
                                    // Đọc quyền từ biến global 'permissions'
                                    visible: permissions.canDelete,
                                    // Sử dụng key localization đã định nghĩa
                                    confirmMessage: (data) => l('AreYouSureToDelete', data.record.name || data.record.code),
                                    action: (data) => {
                                        rankAppService.delete(data.record.id)
                                            .then(() => {
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            })
                                            .catch((error) => {
                                                // Hiển thị lỗi chi tiết hơn nếu có
                                                let message = error.message || l('ErrorOccurred'); // Cần key ErrorOccurred
                                                if (error.details) { message += "\\n" + error.details; }
                                                abp.notify.error(message);
                                            });
                                    }
                                }
                            ]
                        }
                    },
                    // Index cột cần khớp với thứ tự hiển thị
                    { title: l('Rank:Code'), data: "code" },           // Index 1
                    { title: l('Rank:Name'), data: "name" },           // Index 2
                    { title: l('Rank:Order'), data: "order" },         // Index 3
                    {                                                  // Index 4
                        title: l('Rank:Status'), data: "status",
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:RankStatus:' + data)}</span>` // Sửa lại key
                    },
                    { title: l('Rank:Description'), data: "description", orderable: false }, // Index 5
                ]
            })
        );
    }

    initializeDataTable();

    // Event listeners
    $('#SearchButton').on('click', () => dataTable.ajax.reload());
    $('#StatusFilter').on('change', () => dataTable.ajax.reload());
    $('#SearchFilter').on('keypress', (e) => { if (e.which === 13) dataTable.ajax.reload(); });
    $('#NewRankButton').on('click', (e) => { e.preventDefault(); createModal.open(); });

    // Modal results
    createModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });
    editModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });

    // Excel export
    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        const filterInput = getFilterInputs();
        const sortInfo = dataTable ? dataTable.order()[0] : null;
        let sorting = '';
        if (sortInfo) {
            // Map index cột hiển thị sang tên thuộc tính
            const columnIndex = sortInfo[0];
            const sortDirection = sortInfo[1];
            const columnMap = { 1: 'code', 2: 'name', 3: 'order', 4: 'status' }; // Khớp với columnDefs
            const columnName = columnMap[columnIndex];
            if (columnName) {
                sorting = columnName + ' ' + sortDirection;
            } else {
                // Sắp xếp mặc định nếu không tìm thấy cột (ví dụ: cột Actions)
                sorting = 'order asc, name asc';
            }
        } else {
            sorting = 'order asc, name asc'; // Sắp xếp mặc định
        }

        const params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null && filterInput.status !== undefined) params.append('Status', filterInput.status);
        if (sorting) params.append('Sorting', sorting);

        // Tạo URL đúng cách
        const exportUrl = abp.appPath + 'api/app/rank/as-excel?' + params.toString();
        // Chuyển hướng trình duyệt để tải file
        window.location.href = exportUrl; // Sử dụng window.location.href
    });
});