$(function () {
    var l = abp.localization.getResource('CoreFW');
    // !!! Kiểm tra lại namespace JS Proxy nếu cần thiết !!!
    var provinceAppService = aqt.coreFW.application.provinces.province;

    var createModal = new abp.ModalManager(abp.appPath + 'Provinces/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Provinces/EditModal');

    var dataTable = null;

    var getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
            countryId: $('#CountryFilter').val() === "" ? null : $('#CountryFilter').val()
        };
    };

    function loadCountryFilter() {
        provinceAppService.getCountryLookup()
            .then(function (result) {
                var select = $('#CountryFilter');
                select.find('option:gt(0)').remove(); // Giữ lại option "All"

                result.items.forEach(function (country) {
                    select.append($('<option>', {
                        value: country.id,
                        text: country.name
                    }));
                });
                // Optional: Khởi tạo Select2 nếu bạn sử dụng
            });
    }

    function initializeDataTable() {
        if (dataTable) {
            dataTable.destroy();
        }

        dataTable = $('#ProvincesTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[1, "asc"]], // Sắp xếp mặc định theo cột Mã (index 1)
                searching: false, // Tắt searching mặc định, dùng filter tùy chỉnh
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(provinceAppService.getList, getFilterInputs),
                columnDefs: [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit'),
                                    icon: "fas fa-pencil-alt",
                                    // Sử dụng biến permissions đã truyền từ C#
                                    visible: permissions.canEdit,
                                    action: (data) => { editModal.open({ id: data.record.id }); }
                                },
                                {
                                    text: l('Delete'),
                                    icon: "fas fa-trash",
                                    visible: permissions.canDelete,
                                    confirmMessage: (data) => l('AreYouSureToDeleteProvince', data.record.name || data.record.code),
                                    action: (data) => {
                                        provinceAppService.delete(data.record.id)
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
                    { title: l('DisplayName:Province.Code'), data: "code" },
                    { title: l('DisplayName:Province.Name'), data: "name" },
                    { title: l('DisplayName:Province.CountryName'), data: "countryName", orderable: true },
                    { title: l('DisplayName:Province.Order'), data: "order" },
                    {
                        title: l('DisplayName:Province.Status'), data: "status",
                        render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:ProvinceStatus:' + data)}</span>`
                    },
                    { title: l('DisplayName:Province.Description'), data: "description", orderable: false },
                    // { title: l('DisplayName:Province.LastSyncedTime'), data: "lastSyncedTime", render: function (data) { return data ? moment(data).format('YYYY-MM-DD HH:mm') : ''; } },
                ]
            })
        );
    }

    loadCountryFilter();
    initializeDataTable();

    $('#SearchButton').on('click', () => dataTable.ajax.reload());
    $('#SearchFilter').on('keypress', (e) => { if (e.which === 13) dataTable.ajax.reload(); });
    $('#StatusFilter, #CountryFilter').on('change', () => dataTable.ajax.reload());
    $('#NewProvinceButton').on('click', (e) => { e.preventDefault(); createModal.open(); });

    createModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });
    editModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });

    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        const filterInput = getFilterInputs();
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

        const params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null && filterInput.status !== undefined) params.append('Status', filterInput.status);
        if (filterInput.countryId) params.append('CountryId', filterInput.countryId);
        if (sorting) params.append('Sorting', sorting);

        // !!! Kiểm tra lại URL API nếu cần thiết !!!
        const exportUrl = abp.appPath + 'api/app/province/as-excel?' + params.toString();
        location.href = exportUrl;
    });
});