$(function () {
    var l = abp.localization.getResource('CoreFW');
    // *** QUAN TRỌNG: Đảm bảo tên service này khớp với proxy được tạo tự động ***
    var dataGroupAppService = aqt.coreFW.application.dataGroups.dataGroup;

    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'DataGroups/CreateModal',
        // scriptUrl: '/Pages/DataGroups/create-modal.js'
    });
    var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'DataGroups/EditModal',
        // scriptUrl: '/Pages/DataGroups/edit-modal.js'
    });

    var dataTable = null;

    function loadParentFilter() {
        var $parentFilter = $('#ParentFilter');
        var selectedValue = $parentFilter.val();
        $parentFilter.find('option:gt(1)').remove();

        dataGroupAppService.getLookup()
            .then(function (result) {
                if (result && result.items && result.items.length > 0) {
                    result.items.forEach(function (item) {
                        $parentFilter.append($('<option>', {
                            value: item.id,
                            text: `${item.name} (${item.code})`
                        }));
                    });
                    if (selectedValue) {
                        $parentFilter.val(selectedValue);
                    }
                }
                // $parentFilter.select2({ placeholder: l('SelectParentGroup'), allowClear: true });
            })
            .catch(function (error) {
                abp.notify.error(l('ErrorLoadingParentFilter'));
                console.error("Error loading parent filter via AppService:", error);
            });
    }

    var getFilterInputs = function () {
        var parentFilterVal = $('#ParentFilter').val();
        var parentId = null;
        var parentIdIsNull = null;

        if (parentFilterVal === "null") {
            parentIdIsNull = true;
        } else if (parentFilterVal !== "") {
            parentId = parentFilterVal;
        }

        return {
            filter: $('#SearchFilter').val().trim(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
            parentId: parentId,
            parentIdIsNull: parentIdIsNull
        };
    };

    function initializeDataTable() {
        if (dataTable) { dataTable.destroy(); }

        dataTable = $('#DataGroupsTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[3, "asc"], [1, "asc"]], // Sắp xếp theo Order, rồi Code
                searching: false,
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(dataGroupAppService.getList, getFilterInputs),
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'), icon: "fas fa-pencil-alt",
                                    visible: permissions.canEdit,
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                {
                                    text: l('Delete'), icon: "fas fa-trash",
                                    visible: permissions.canDelete,
                                    confirmMessage: (data) => l('AreYouSureToDeleteDataGroup', data.record.name || data.record.code),
                                    action: (data) => {
                                        dataGroupAppService.delete(data.record.id)
                                            .then(() => {
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                                loadParentFilter(); // Reload parent filter after delete
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
                    { title: l('DisplayName:DataGroup.Code'), data: "code" },
                    { title: l('DisplayName:DataGroup.Name'), data: "name" },
                    { title: l('DisplayName:DataGroup.Order'), data: "order" },
                    {
                        title: l('DisplayName:DataGroup.Status'), data: "status",
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:DataGroupStatus.' + data)}</span>`
                    },
                    // Hiển thị thông tin cha (nếu có)
                    {
                        title: l('DisplayName:DataGroup.ParentId'), data: "parentName", orderable: false, // Sort theo ParentName có thể phức tạp
                        render: (data, type, row) => data ? `${data} (${row.parentCode || ''})` : '' // Hiển thị tên (mã)
                    },
                    { title: l('DisplayName:DataGroup.Description'), data: "description", orderable: false },
                ]
            })
        );
    }

    loadParentFilter();
    initializeDataTable();

    $('#SearchButton').on('click', function () { dataTable.ajax.reload(); });
    $('#SearchFilter').on('keypress', function (e) { if (e.which === 13) dataTable.ajax.reload(); });
    $('#StatusFilter, #ParentFilter').on('change', function () { dataTable.ajax.reload(); });
    $('#NewDataGroupButton').on('click', (e) => { e.preventDefault(); createModal.open(); });
    createModal.onResult(() => { dataTable.ajax.reload(); loadParentFilter(); abp.notify.success(l('SuccessfullySaved')); });
    editModal.onResult(() => { dataTable.ajax.reload(); loadParentFilter(); abp.notify.success(l('SuccessfullySaved')); });

    $('#ExportExcelButton')?.on('click', function (e) {
         e.preventDefault();
         const filterInput = getFilterInputs();
         const order = dataTable.order();
         let sorting = 'order asc, name asc';
         if (order && order.length > 0) {
             const sortInfo = order[0];
             const columnIndex = sortInfo[0];
             const sortDirection = sortInfo[1];
             const columnMap = { 1: 'code', 2: 'name', 3: 'order', 4: 'status' };
             const columnName = columnMap[columnIndex];
             if (columnName) { sorting = `${columnName} ${sortDirection}`; }
         }
         const params = new URLSearchParams({ Sorting: sorting });
         if (filterInput.filter) params.append('FilterText', filterInput.filter);
         if (filterInput.status !== null) params.append('Status', filterInput.status);
         if (filterInput.parentId) params.append('ParentId', filterInput.parentId);
         if (filterInput.parentIdIsNull === true) params.append('ParentIdIsNull', 'true');

         // *** QUAN TRỌNG: Đảm bảo API endpoint này tồn tại và đúng ***
         const exportUrl = abp.appPath + 'api/app/data-group/as-excel?' + params.toString();
         location.href = exportUrl;
    });
});