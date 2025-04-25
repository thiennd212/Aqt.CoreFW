$(function () {
    let l = abp.localization.getResource('CoreFW');

    // Service Proxies - Đảm bảo namespace chính xác
    // !! Kiểm tra lại tên proxy được generate !!
    let procedureService = aqt.coreFW.application.procedures.procedure; // Updated Proxy name

    // Modals
    let createModal = new abp.ModalManager(abp.appPath + 'Procedures/CreateModal'); // Updated path
    let editModal = new abp.ModalManager(abp.appPath + 'Procedures/EditModal'); // Updated path

    // DataTable Instance
    let dataTable = null;

    // Function to get current filter values
    let getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val())
        };
    };

    // Function to initialize DataTable
    function initializeDataTable() {
        if (dataTable) { dataTable.destroy(); }

        dataTable = $('#ProcedureTable').DataTable( // Updated table ID
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[3, "asc"], [1, "asc"]], // Sắp xếp theo Order, sau đó Code
                searching: false, // Dùng filter tùy chỉnh
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(procedureService.getList, getFilterInputs), // Updated service call
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'),
                                    icon: "fas fa-pencil-alt",
                                    visible: permissions.canEdit, // Biến permission từ C#
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fas fa-trash",
                                    visible: permissions.canDelete, // Biến permission từ C#
                                    // Updated key localization and service
                                    confirmMessage: (data) => l('AreYouSureToDeleteProcedure', data.record.name || data.record.code),
                                    action: (data) => {
                                        procedureService.delete(data.record.id) // Updated service call
                                            .then(() => {
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            })
                                            .catch((error) => {
                                                abp.notify.error(error.message || l('ErrorOccurred'));
                                            });
                                    }
                                }
                            ]
                        }
                    },
                    // Cập nhật các key localization và data properties
                    { title: l('DisplayName:Procedure.Code'), data: "code" }, // Updated key
                    { title: l('DisplayName:Procedure.Name'), data: "name" }, // Updated key
                    { title: l('DisplayName:Procedure.Order'), data: "order" }, // Updated key
                    {
                        title: l('DisplayName:Procedure.Status'), data: "status", className: 'text-center', // Updated key
                        // Updated key localization and enum value
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:ProcedureStatus.' + data)}</span>`
                    },
                    { title: l('DisplayName:Procedure.Description'), data: "description", orderable: false }, // Updated key
                    // Có thể thêm cột LastSyncedDate nếu cần hiển thị
                    // {
                    //     title: l('DisplayName:Procedure.LastSyncedDate'), data: "lastSyncedDate", orderable: true, // Updated key
                    //     render: (data) => data ? new Date(data).toLocaleDateString() : '' // Định dạng ngày
                    // }
                ]
            })
        );
    }

    // --- Initialization ---
    initializeDataTable();

    // --- Event Handlers ---

    // Search button click or Enter
    $('#SearchButton').click(function () { dataTable.ajax.reload(); });
    $('#SearchFilter').on('keypress', function (e) { if (e.which === 13) dataTable.ajax.reload(); });

    // Filter dropdown changes
    $('#StatusFilter').change(function () {
        dataTable.ajax.reload();
    });

    // New Procedure button click
    $('#NewProcedureButton').click(function (e) { // Updated button ID
        e.preventDefault();
        createModal.open();
    });

    // Reload table on modal success
    createModal.onResult(function () {
        dataTable.ajax.reload();
        abp.notify.success(l('SuccessfullySaved'));
    });
    editModal.onResult(function () {
        dataTable.ajax.reload();
        abp.notify.success(l('SuccessfullySaved'));
    });

    // Excel Export Button Click (if exists)
    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        let filterInput = getFilterInputs();
        let sortInfo = dataTable ? dataTable.order()[0] : null;
        let sorting = '';

        if (sortInfo) {
            let columnIndex = sortInfo[0];
            let sortDirection = sortInfo[1];
            // Map index cột hiển thị sang tên trường DTO API
            // !! Điều chỉnh map theo thứ tự cột mới !!
            let columnMap = { 1: 'code', 2: 'name', 3: 'order', 4: 'status', 5: 'description' }; // Cập nhật nếu cần
            let columnName = columnMap[columnIndex];
            if (columnName) {
                sorting = columnName + ' ' + sortDirection;
            } else {
                sorting = 'order asc, name asc'; // Default sort
            }
        } else {
            sorting = 'order asc, name asc'; // Default sort
        }

        let params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null) params.append('Status', filterInput.status);
        if (sorting) params.append('Sorting', sorting);

        // !! Cập nhật API endpoint !!
        let exportUrl = abp.appPath + 'api/app/procedure/as-excel?' + params.toString(); // Updated endpoint
        location.href = exportUrl; // Trigger download
    });
});