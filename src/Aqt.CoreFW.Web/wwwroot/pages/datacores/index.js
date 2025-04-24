$(function () {
    let l = abp.localization.getResource('CoreFW');

    // Service Proxies - Đảm bảo namespace chính xác
    let dataCoreService = aqt.coreFW.application.dataCores.dataCore; // Correct proxy name
    let dataGroupService = aqt.coreFW.application.dataGroups.dataGroup; // Proxy for DataGroup lookup
    // Modals
    let createModal = new abp.ModalManager(abp.appPath + 'DataCores/CreateModal');
    let editModal = new abp.ModalManager(abp.appPath + 'DataCores/EditModal');

    // DataTable Instance
    let dataTable = null;

    // Function to load DataGroup filter options using the Service Proxy
    function loadDataGroupFilter() {
        $('#DataGroupFilter option:gt(0)').remove(); // Clear existing except "All"

        // Call the service proxy directly
        dataGroupService.getLookup()
            .then(function (result) { // Use .then for promises
                if (result && result.items && result.items.length > 0) {
                    result.items.forEach(function (item) {
                        // Assuming the lookup DTO has 'id' and 'displayName' or similar
                        // Adjust property names if your DataGroupLookupDto is different
                        $('#DataGroupFilter').append($('<option>', {
                            value: item.id, // Use ID from the lookup DTO
                            text: `${item.name} (${item.code || ''})` // Combine name and code
                        }));
                    });
                }
            })
            .catch(function (error) { // Use .catch for errors
                console.error("Error loading DataGroup filter via service:", error);
                // Optionally notify the user
            });
    }


    // Function to get current filter values from the UI
    let getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
            dataGroupId: $('#DataGroupFilter').val() === "" ? null : $('#DataGroupFilter').val()
        };
    };

    // Function to initialize the DataTable
    function initializeDataTable() {
        if (dataTable) { dataTable.destroy(); }

        dataTable = $('#DataCoresTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[4, "asc"], [1, "asc"]], // Sort by Order then Code
                searching: false, // Use custom filters
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(dataCoreService.getList, getFilterInputs),
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'),
                                    icon: "fas fa-pencil-alt",
                                    visible: permissions.canEdit, // Check boolean permission
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fas fa-trash",
                                    visible: permissions.canDelete, // Check boolean permission
                                    confirmMessage: (data) => l('AreYouSureToDeleteDataCore', data.record.name || data.record.code),
                                    action: (data) => {
                                        dataCoreService.delete(data.record.id)
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
                    { title: l('DisplayName:DataCore.Code'), data: "code" },
                    { title: l('DisplayName:DataCore.Name'), data: "name" },
                    {
                        title: l('DisplayName:DataCore.DataGroupId'), data: "dataGroupName", orderable: false,
                        render: (data, type, row) => data ? `${data} (${row.dataGroupCode || ''})` : '' // Display Name (Code)
                    },
                    { title: l('DisplayName:DataCore.Order'), data: "order" },
                    {
                        title: l('DisplayName:DataCore.Status'), data: "status", className: 'text-center',
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:DataCoreStatus.' + data)}</span>`
                    },
                    { title: l('DisplayName:DataCore.Description'), data: "description", orderable: false }
                ]
            })
        );
    }

    // --- Initialization ---
    loadDataGroupFilter();
    initializeDataTable();

    // --- Event Handlers ---

    // Search button click or Enter in search filter
    $('#SearchButton').click(function () { dataTable.ajax.reload(); });
    $('#SearchFilter').on('keypress', function (e) { if (e.which === 13) dataTable.ajax.reload(); });

    // Filter dropdown changes trigger table reload
    $('#StatusFilter, #DataGroupFilter').change(function () {
        dataTable.ajax.reload();
    });

    // New DataCore button click
    $('#NewDataCoreButton').click(function (e) {
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
            // Map visible column index to API DTO field name for sorting
            let columnMap = { 1: 'code', 2: 'name', 3: 'dataGroupName', 4: 'order', 5: 'status', 6: 'description' };
            let columnName = columnMap[columnIndex];
            if (columnName) {
                sorting = columnName + ' ' + sortDirection;
            } else {
                sorting = 'order asc, name asc'; // Default
            }
        } else {
            sorting = 'order asc, name asc'; // Default
        }

        // Construct query parameters for the API endpoint
        let params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null) params.append('Status', filterInput.status);
        if (filterInput.dataGroupId) params.append('DataGroupId', filterInput.dataGroupId);
        if (sorting) params.append('Sorting', sorting);
        // Paging parameters (MaxResultCount, SkipCount) are typically ignored by the export endpoint

        // Construct the final URL - verify the controller route if needed
        let exportUrl = abp.appPath + 'api/app/data-core/as-excel?' + params.toString();
        location.href = exportUrl; // Trigger file download
    });
});