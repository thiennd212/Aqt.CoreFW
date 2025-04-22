$(function () {
    var l = abp.localization.getResource('CoreFW');
    // Cập nhật service proxy (đảm bảo namespace khớp với dự án của bạn)
    var accountTypeAppService = aqt.coreFW.application.accountTypes.accountType; // Kiểm tra namespace chính xác

    // Cập nhật đường dẫn modal
    var createModal = new abp.ModalManager(abp.appPath + 'AccountTypes/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'AccountTypes/EditModal');

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

        // Cập nhật ID bảng và các columnDefs
        dataTable = $('#AccountTypesTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[3, "asc"], [2, "asc"]], // Sắp xếp theo Order rồi Name (Cột 0 là Actions)
                searching: false,
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(accountTypeAppService.getList, getFilterInputs), // Cập nhật service
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'),
                                    icon: "fas fa-pencil-alt",
                                    visible: permissions.canEdit, // Sử dụng biến permissions
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fas fa-trash",
                                    visible: permissions.canDelete, // Sử dụng biến permissions
                                    // Cập nhật confirm message key
                                    confirmMessage: (data) => l('AreYouSureToDeleteAccountType', data.record.name || data.record.code),
                                    action: (data) => {
                                        // Cập nhật service delete
                                        accountTypeAppService.delete(data.record.id)
                                            .then(() => {
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            })
                                            .catch((error) => {
                                                let message = error.message || l('ErrorOccurred');
                                                if (error.details) { message += "\\n" + error.details; }
                                                abp.notify.error(message);
                                            });
                                    }
                                }
                            ]
                        }
                    },
                    // Cập nhật localization keys và data properties
                    { title: l('DisplayName:AccountType.Code'), data: "code" },
                    { title: l('DisplayName:AccountType.Name'), data: "name" },
                    { title: l('DisplayName:AccountType.Order'), data: "order" },
                    {
                        title: l('DisplayName:AccountType.Status'), data: "status",
                        // Cập nhật localization key cho Enum
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:AccountTypeStatus.' + data)}</span>`
                    },
                    { title: l('DisplayName:AccountType.Description'), data: "description", orderable: false },
                ]
            })
        );
    }

    initializeDataTable();

    $('#SearchButton').on('click', () => dataTable.ajax.reload());
    $('#StatusFilter').on('change', () => dataTable.ajax.reload());
    $('#SearchFilter').on('keypress', (e) => { if (e.which === 13) dataTable.ajax.reload(); });
    // Cập nhật ID nút New
    $('#NewAccountTypeButton').on('click', (e) => { e.preventDefault(); createModal.open(); });

    createModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });
    editModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });

    // Cập nhật Export Excel
    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        const filterInput = getFilterInputs();
        const sortInfo = dataTable ? dataTable.order()[0] : null;
        let sorting = '';
        if (sortInfo) {
            const columnIndex = sortInfo[0];
            const sortDirection = sortInfo[1];
            // Cập nhật column map (phù hợp với thứ tự cột TRỪ cột Actions)
            const columnMap = { 1: 'code', 2: 'name', 3: 'order', 4: 'status' }; // Bắt đầu từ 1
            const columnName = columnMap[columnIndex];
            if (columnName) {
                sorting = columnName + ' ' + sortDirection;
            } else {
                sorting = 'order asc, name asc'; // Sắp xếp mặc định nếu click cột Actions hoặc không xác định
            }
        } else {
            sorting = 'order asc, name asc'; // Sắp xếp mặc định
        }

        const params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null && filterInput.status !== undefined) params.append('Status', filterInput.status);
        if (sorting) params.append('Sorting', sorting);

        // Cập nhật API endpoint (kiểm tra URL API của bạn)
        // Thường là /api/app/{tên-module}/{tên-phương-thức-xuất-excel}
        const exportUrl = abp.appPath + 'api/app/account-type/as-excel?' + params.toString(); // Đảm bảo /api/app/account-type là đúng
        location.href = exportUrl;
    });
});