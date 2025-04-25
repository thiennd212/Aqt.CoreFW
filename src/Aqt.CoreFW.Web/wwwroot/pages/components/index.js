$(function () {
    let l = abp.localization.getResource('CoreFW');

    // Service Proxy - Kiểm tra tên namespace và service trong file generated-proxy.js
    let componentService = aqt.coreFW.application.components.procedureComponent; // Tên proxy thường có hậu tố AppService

    // Modals với lazy-loaded JS
    let createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Components/CreateModal',
        scriptUrl: '/Pages/Components/createModal.js', // Đường dẫn từ wwwroot
        modalClass: 'createModal'
    });

    let editModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Components/EditModal',
        scriptUrl: '/Pages/Components/editModal.js', // Đường dẫn từ wwwroot
        modalClass: 'editComponent'
    });

    // DataTable Instance
    let dataTable = null;

    // Function to get current filter values
    let getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            type: $('#TypeFilter').val() === "" ? null : parseInt($('#TypeFilter').val()),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val())
        };
    };

    // Function to initialize DataTable
    function initializeDataTable() {
        if (dataTable) { dataTable.destroy(); }

        dataTable = $('#ComponentTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[3, "asc"], [1, "asc"]], // Sắp xếp theo Order, sau đó Code
                searching: false,
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(componentService.getList, getFilterInputs),
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'),
                                    icon: "fas fa-pencil-alt",
                                    // Hiển thị nếu có quyền Update HOẶC ManageLinks
                                    visible: () => permissions.canEdit || permissions.canManageLinks,
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fas fa-trash",
                                    visible: () => permissions.canDelete,
                                    confirmMessage: (data) => l('AreYouSureToDeleteComponent', data.record.name || data.record.code),
                                    action: (data) => {
                                        componentService.delete(data.record.id)
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
                    { title: l('DisplayName:Component.Code'), data: "code" },
                    { title: l('DisplayName:Component.Name'), data: "name" },
                    { title: l('DisplayName:Component.Order'), data: "order", className: 'text-center' },
                    {
                        title: l('DisplayName:Component.Type'), data: "type", className: 'text-center',
                        render: (data) => l('Enum:ComponentType.' + data)
                    },
                    {
                        title: l('DisplayName:Component.Status'), data: "status", className: 'text-center',
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:ComponentStatus.' + data)}</span>`
                    },
                    { title: l('DisplayName:Component.Description'), data: "description", orderable: false }
                ]
            })
        );
    }

    // --- Initialization ---
    initializeDataTable();

    // --- Event Handlers ---
    $('#SearchButton').click(function () { dataTable.ajax.reload(); });
    $('#SearchFilter').on('keypress', function (e) { if (e.which === 13) dataTable.ajax.reload(); });
    $('#StatusFilter, #TypeFilter').change(function () { dataTable.ajax.reload(); });
    $('#NewComponentButton').click(function (e) { e.preventDefault(); createModal.open(); });

    createModal.onResult(function () { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });
    editModal.onResult(function () { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });

    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        let filterInput = getFilterInputs();
        let sortInfo = dataTable ? dataTable.order()[0] : null;
        let sorting = '';

        if (sortInfo) {
            let columnIndex = sortInfo[0];
            let sortDirection = sortInfo[1];
            // Map index cột hiển thị sang tên trường DTO API - Đảm bảo map đúng thứ tự cột trong columnDefs
            let columnMap = { 1: 'code', 2: 'name', 3: 'order', 4: 'type', 5: 'status', 6: 'description' };
            let columnName = columnMap[columnIndex];
            if (columnName) {
                sorting = columnName + ' ' + sortDirection;
            } else {
                sorting = 'order asc, name asc'; // Default
            }
        } else {
            sorting = 'order asc, name asc'; // Default
        }

        let params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null) params.append('Status', filterInput.status);
        if (filterInput.type !== null) params.append('Type', filterInput.type);
        if (sorting) params.append('Sorting', sorting);

        // Endpoint API cho export Excel
        let exportUrl = abp.appPath + 'api/app/procedure-component/as-excel?' + params.toString(); // Chú ý tên controller/endpoint
        location.href = exportUrl;
    });
});