$(function () {
    let l = abp.localization.getResource('CoreFW');

    // Service Proxies - Đảm bảo namespace chính xác
    // !! Kiểm tra lại tên proxy được generate !!
    let attachedDocumentService = aqt.coreFW.application.attachedDocuments.attachedDocument; // Proxy mới
    let procedureService = aqt.coreFW.application.procedures.procedure; // Proxy cho Procedure lookup (Giả định)

    // Modals
    let createModal = new abp.ModalManager(abp.appPath + 'AttachedDocuments/CreateModal'); // Đường dẫn modal mới
    let editModal = new abp.ModalManager(abp.appPath + 'AttachedDocuments/EditModal'); // Đường dẫn modal mới

    // DataTable Instance
    let dataTable = null;

    // Function to load Procedure filter options
    function loadProcedureFilter() { // Đổi tên hàm
        $('#ProcedureFilter option:gt(0)').remove(); // Clear existing except "All"

        procedureService.getLookup() // Gọi service Procedure (Giả định)
            .then(function (result) {
                if (result && result.items && result.items.length > 0) {
                    result.items.forEach(function (item) {
                        $('#ProcedureFilter').append($('<option>', {
                            value: item.id,
                            text: `${item.name} (${item.code || ''})` // Giả định ProcedureLookupDto có Name, Code
                        }));
                    });
                }
            })
            .catch(function (error) {
                console.error("Error loading Procedure filter:", error);
            });
    }


    // Function to get current filter values
    let getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
            procedureId: $('#ProcedureFilter').val() === "" ? null : $('#ProcedureFilter').val() // Filter theo procedureId
        };
    };

    // Function to initialize DataTable
    function initializeDataTable() {
        if (dataTable) { dataTable.destroy(); }

        dataTable = $('#AttachedDocumentsTable').DataTable( // ID bảng mới
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[4, "asc"], [1, "asc"]], // Sắp xếp theo Order, sau đó Code
                searching: false, // Dùng filter tùy chỉnh
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(attachedDocumentService.getList, getFilterInputs), // Gọi service mới
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
                                    confirmMessage: (data) => l('AreYouSureToDeleteAttachedDocument', data.record.name || data.record.code), // Key mới
                                    action: (data) => {
                                        attachedDocumentService.delete(data.record.id) // Service mới
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
                    { title: l('DisplayName:AttachedDocument.Code'), data: "code" }, // Key mới
                    { title: l('DisplayName:AttachedDocument.Name'), data: "name" }, // Key mới
                    {
                        title: l('DisplayName:AttachedDocument.ProcedureId'), data: "procedureName", orderable: false, // Key mới, data từ ProcedureName
                        render: (data, type, row) => data ? `${data} (${row.procedureCode || ''})` : '' // Hiển thị Name (Code) của Procedure
                    },
                    { title: l('DisplayName:AttachedDocument.Order'), data: "order" }, // Key mới
                    {
                        title: l('DisplayName:AttachedDocument.Status'), data: "status", className: 'text-center', // Key mới
                        // Key localization và enum value mới
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:AttachedDocumentStatus.' + data)}</span>` // Key enum mới
                    },
                    { title: l('DisplayName:AttachedDocument.Description'), data: "description", orderable: false } // Key mới
                ]
            })
        );
    }

    // --- Initialization ---
    loadProcedureFilter(); // Load filter Procedure
    initializeDataTable();

    // --- Event Handlers ---

    // Search button click or Enter
    $('#SearchButton').click(function () { dataTable.ajax.reload(); });
    $('#SearchFilter').on('keypress', function (e) { if (e.which === 13) dataTable.ajax.reload(); });

    // Filter dropdown changes
    $('#StatusFilter, #ProcedureFilter').change(function () { // Filter Procedure
        dataTable.ajax.reload();
    });

    // New AttachedDocument button click
    $('#NewAttachedDocumentButton').click(function (e) { // ID nút mới
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
            // Map index cột hiển thị sang tên trường DTO API (CẬP NHẬT MAP!!)
            let columnMap = { 1: 'code', 2: 'name', 3: 'procedureName', 4: 'order', 5: 'status', 6: 'description' };
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
        if (filterInput.procedureId) params.append('ProcedureId', filterInput.procedureId); // Tham số mới
        if (sorting) params.append('Sorting', sorting);

        // !! Cập nhật API endpoint !!
        let exportUrl = abp.appPath + 'api/app/attached-document/as-excel?' + params.toString(); // Endpoint mới
        location.href = exportUrl; // Trigger download
    });
});