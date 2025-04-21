$(function () {
    var l = abp.localization.getResource('CoreFW');
    var districtAppService = aqt.coreFW.application.districts.district; // Namespace proxy JS - CHECK YOUR PROJECT

    var createModal = new abp.ModalManager(abp.appPath + 'Districts/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Districts/EditModal');

    var dataTable = $('#DistrictsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[2, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(districtAppService.getList, function () {
                return {
                    filter: $('#SearchFilter').val(),
                    status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
                    provinceId: $('#ProvinceFilter').val() === "" ? null : $('#ProvinceFilter').val()
                };
            }),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: permissions.canEdit, // Assumes 'permissions' variable exists from C#
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: permissions.canDelete, // Assumes 'permissions' variable exists from C#
                                confirmMessage: function (data) {
                                    return l('AreYouSureToDeleteDistrict', data.record.name);
                                },
                                action: function (data) {
                                    districtAppService
                                        .delete(data.record.id)
                                        .then(function () {
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                { title: l('DisplayName:District.Code'), data: "code" },
                { title: l('DisplayName:District.Name'), data: "name" },
                { title: l('DisplayName:District.ProvinceName'), data: "provinceName", orderable: false },
                { title: l('DisplayName:District.Order'), data: "order" },
                {
                    title: l('DisplayName:District.Status'), data: "status",
                    render: (data) => l('Enum:DistrictStatus.' + data)
                },
                { title: l('DisplayName:District.Description'), data: "description", orderable: false },
            ]
        })
    );

    // Load province filter dropdown - This part was added for functionality, not explicitly in plan's JS block
    function populateProvinceFilter() {
        districtAppService.getProvinceLookup().done(function (result) {
            var provinceFilter = $('#ProvinceFilter');
            provinceFilter.find('option:not(:first)').remove();
            result.items.forEach(function (item) {
                provinceFilter.append(new Option(item.name, item.id));
            });
        });
    }
    populateProvinceFilter();

    $('#NewDistrictButton').on('click', function (e) {
        e.preventDefault();
        createModal.open();
    });

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#SearchButton').on('click', function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        const filterInput = {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
            provinceId: $('#ProvinceFilter').val() === "" ? null : $('#ProvinceFilter').val()
        };

        const params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null && filterInput.status !== undefined) params.append('Status', filterInput.status);
        if (filterInput.provinceId) params.append('ProvinceId', filterInput.provinceId);

        const exportUrl = abp.appPath + 'api/app/district/as-excel?' + params.toString();
        location.href = exportUrl;
    });
});