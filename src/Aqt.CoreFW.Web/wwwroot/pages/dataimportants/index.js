$(function () {
    let l = abp.localization.getResource('CoreFW');

    // Service Proxies - Đảm bảo namespace chính xác
    // !! Kiểm tra lại tên proxy được generate !!
    let dataImportantService = aqt.coreFW.application.dataImportants.dataImportant; // Proxy cho DataImportant
    let dataGroupService = aqt.coreFW.application.dataGroups.dataGroup; // Proxy cho DataGroup lookup

    // Modals
    let createModal = new abp.ModalManager(abp.appPath + 'DataImportants/CreateModal'); // Đường dẫn modal mới
    let editModal = new abp.ModalManager(abp.appPath + 'DataImportants/EditModal'); // Đường dẫn modal mới

    // DataTable Instance
    let dataTable = null;

    // Function to load DataGroup filter options
    function loadDataGroupFilter() {
        $('#DataGroupFilter option:gt(0)').remove(); // Clear existing except "All"

        dataGroupService.getLookup()
            .then(function (result) {
                if (result && result.items && result.items.length > 0) {
                    result.items.forEach(function (item) {
                        $('#DataGroupFilter').append($('<option>', {
                            value: item.id,
                            text: `${item.name} (${item.code || ''})`
                        }));
                    });
                }
            })
            .catch(function (error) {
                console.error("Error loading DataGroup filter:", error);
            });
    }


    // Function to get current filter values
    let getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
            dataGroupId: $('#DataGroupFilter').val() === "" ? null : $('#DataGroupFilter').val()
        };
    };

    // Function to initialize DataTable
    function initializeDataTable() {
        if (dataTable) { dataTable.destroy(); }

        dataTable = $('#DataImportantsTable').DataTable( // ID bảng mới
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[4, "asc"], [1, "asc"]], // Sắp xếp theo Order, sau đó Code
                searching: false, // Dùng filter tùy chỉnh
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(dataImportantService.getList, getFilterInputs), // Gọi service mới
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
                                    // Key localization và service mới
                                    confirmMessage: (data) => l('AreYouSureToDeleteDataImportant', data.record.name || data.record.code),
                                    action: (data) => {
                                        dataImportantService.delete(data.record.id)
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
                    { title: l('DisplayName:DataImportant.Code'), data: "code" },
                    { title: l('DisplayName:DataImportant.Name'), data: "name" },
                    {
                        title: l('DisplayName:DataImportant.DataGroupId'), data: "dataGroupName", orderable: false,
                        render: (data, type, row) => data ? `${data} (${row.dataGroupCode || ''})` : '' // Hiển thị Name (Code)
                    },
                    { title: l('DisplayName:DataImportant.Order'), data: "order" },
                    {
                        title: l('DisplayName:DataImportant.Status'), data: "status", className: 'text-center',
                        // Key localization và enum value mới
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:DataImportantStatus.' + data)}</span>`
                    },
                    { title: l('DisplayName:DataImportant.Description'), data: "description", orderable: false }
                ]
            })
        );
    }

    // --- Initialization ---
    loadDataGroupFilter();
    initializeDataTable();

    // --- Event Handlers ---

    // Search button click or Enter
    $('#SearchButton').click(function () { dataTable.ajax.reload(); });
    $('#SearchFilter').on('keypress', function (e) { if (e.which === 13) dataTable.ajax.reload(); });

    // Filter dropdown changes
    $('#StatusFilter, #DataGroupFilter').change(function () {
        dataTable.ajax.reload();
    });

    // New DataImportant button click
    $('#NewDataImportantButton').click(function (e) { // ID nút mới
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
            let columnMap = { 1: 'code', 2: 'name', 3: 'dataGroupName', 4: 'order', 5: 'status', 6: 'description' };
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
        if (filterInput.dataGroupId) params.append('DataGroupId', filterInput.dataGroupId);
        if (sorting) params.append('Sorting', sorting);

        // !! Cập nhật API endpoint !!
        let exportUrl = abp.appPath + 'api/app/data-important/as-excel?' + params.toString();
        location.href = exportUrl; // Trigger download
    });
});