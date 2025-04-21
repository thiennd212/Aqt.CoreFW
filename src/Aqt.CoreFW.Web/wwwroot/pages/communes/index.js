$(function () {
    var l = abp.localization.getResource('CoreFW');
    // Correct ABP JS Proxy namespace (check your project's generated proxy)
    var communeAppService = aqt.coreFW.application.communes.commune; // Updated service name

    var createModal = new abp.ModalManager(abp.appPath + 'Communes/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Communes/EditModal');

    var dataTable = null; // Declare globally

    // Function to get filter values including Province and District
    var getFilterInputs = function () {
        return {
            filter: $('#SearchFilter').val(),
            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
            provinceId: $('#ProvinceFilter').val() === "" ? null : $('#ProvinceFilter').val(),
            districtId: $('#DistrictFilter').val() === "" ? null : $('#DistrictFilter').val() // Get selected District GUID
        };
    };

    // Function to load province lookup data into the filter dropdown
    function loadProvinceFilter() {
        communeAppService.getProvinceLookup()
            .then(function (result) {
                var select = $('#ProvinceFilter');
                select.find('option:gt(0)').remove(); // Remove existing options except "All"

                result.items.forEach(function (province) {
                    select.append($('<option>', {
                        value: province.id,
                        text: province.name
                    }));
                });
                // Optional: select.select2(...);
            });
    }

    // Function to load district lookup data based on selected province
    function loadDistrictFilter(provinceId) {
        var select = $('#DistrictFilter');
        select.find('option:gt(0)').remove(); // Remove existing options except "All"
        select.prop('disabled', !provinceId); // Disable if no province selected

        if (!provinceId) {
            // Optional: select.select2(...);
            return;
        }

        communeAppService.getDistrictLookup(provinceId)
            .then(function (result) {
                result.items.forEach(function (district) {
                    select.append($('<option>', {
                        value: district.id,
                        text: district.name
                    }));
                });
                // Optional: select.select2(...);
            });
    }

    // Function to initialize or reinitialize DataTable - **Kế hoạch không có hàm này, khởi tạo trực tiếp**
    // function initializeDataTable() { ... } // <- Không có trong kế hoạch

    // --- DataTable Initialization (Trực tiếp theo kế hoạch) ---
    dataTable = $('#CommunesTable').DataTable( // Updated table ID in plan
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[2, "asc"]], // Default sort by District -> Province -> Name (theo kế hoạch)
            searching: false, // Custom filters
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(communeAppService.getList, getFilterInputs),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                icon: "fa fa-pencil-alt",
                                visible: permissions.canEdit, // Đọc từ biến permissions toàn cục như plan .cshtml
                                action: (data) => { editModal.open({ id: data.record.id }); }
                            },
                            {
                                text: l('Delete'),
                                icon: "fa fa-trash",
                                visible: permissions.canDelete, // Đọc từ biến permissions toàn cục như plan .cshtml
                                confirmMessage: (data) => l('AreYouSureToDeleteCommune', data.record.name || data.record.code), // Localization key từ plan
                                action: (data) => {
                                    communeAppService.delete(data.record.id)
                                        .then(() => {
                                            abp.notify.success(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        })
                                        .catch((error) => {
                                            // Xử lý lỗi theo plan
                                            let message = error.message || l('ErrorOccurred');
                                            if (error.details) { message += "\\n" + error.details; } // Dùng \\n như plan
                                            abp.notify.error(message); // Plan không có title lỗi
                                        });
                                }
                            }
                        ]
                    }
                },
                // Các cột còn lại theo kế hoạch
                { title: l('DisplayName:Commune.Code'), data: "code" },
                { title: l('DisplayName:Commune.Name'), data: "name" },
                { title: l('DisplayName:Commune.DistrictName'), data: "districtName", orderable: true }, // Added District Name in plan
                { title: l('DisplayName:Commune.ProvinceName'), data: "provinceName", orderable: true },
                { title: l('DisplayName:Commune.Order'), data: "order" },
                {
                    title: l('DisplayName:Commune.Status'), data: "status",
                    // Render theo plan
                    render: (data) => `<span class="badge ${data === 1 ? 'bg-success' : 'bg-secondary'}">${l('Enum:CommuneStatus.' + data)}</span>` // localization key từ plan
                },
                { title: l('DisplayName:Commune.Description'), data: "description", orderable: false },
                // Sync time bị comment trong plan
                // { title: l('DisplayName:Commune.LastSyncedTime'), data: "lastSyncedTime", render: function (data) { return data ? moment(data).format('YYYY-MM-DD HH:mm') : ''; } },
            ]
        })
    );

    // --- Initialization Calls ---
    loadProvinceFilter();
    loadDistrictFilter(null); // Load empty districts initially

    // --- Event Handlers ---
    $('#ProvinceFilter').on('change', function () {
        const selectedProvinceId = $(this).val() === "" ? null : $(this).val();
        loadDistrictFilter(selectedProvinceId);
        dataTable.ajax.reload(); // Reload table when province changes
    });

    $('#SearchButton').on('click', () => dataTable.ajax.reload());
    $('#SearchFilter').on('keypress', (e) => { if (e.which === 13) dataTable.ajax.reload(); });
    $('#StatusFilter, #DistrictFilter').on('change', () => dataTable.ajax.reload()); // Reload table when District changes
    $('#NewCommuneButton').on('click', (e) => { e.preventDefault(); createModal.open(); });

    // Modal Success Callbacks
    createModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });
    editModal.onResult(() => { dataTable.ajax.reload(); abp.notify.success(l('SuccessfullySaved')); });

    // AJAX listener for dynamic dropdowns in modals (theo plan)
    $(document).on('change', '#CreateCommuneForm select[name="CommuneViewModel.ProvinceId"], #EditCommuneForm select[name="CommuneViewModel.ProvinceId"]', function () {
        const selectedProvinceId = $(this).val();
        const districtDropdown = $(this).closest('form').find('select[name="CommuneViewModel.DistrictId"]');
        // Lấy đường dẫn trang từ attribute asp-page như trong plan
        const url = $(this).closest('form').attr('asp-page') + '?handler=DistrictsByProvince&provinceId=' + (selectedProvinceId || '');

        districtDropdown.find('option:gt(0)').remove(); // Clear existing districts (trừ option đầu tiên nếu có)
        districtDropdown.prop('disabled', !selectedProvinceId);

        if (selectedProvinceId) {
            abp.ajax({ url: url, type: 'GET' }) // Plan không ghi rõ dataType
                .then(function (result) {
                    // Plan không có kiểm tra result, chỉ lặp trực tiếp
                    // if (result && result.length > 0) { ... } // <- Bỏ qua kiểm tra này để giống plan
                    result.forEach(function (item) { // Lặp trực tiếp như plan
                        districtDropdown.append($('<option>', { value: item.value, text: item.text }));
                    });
                    // Optionally re-initialize Select2 if used
                }); // Plan không có .catch
        } else {
            // Optionally re-initialize Select2 if used
        }
    });


    // Export Excel Button Handler (theo plan)
    $('#ExportExcelButton')?.on('click', function (e) {
        e.preventDefault();
        const filterInput = getFilterInputs(); // Lấy filter

        // Lấy thông tin sắp xếp theo plan
        const sortInfo = dataTable ? dataTable.order()[0] : null;
        let sorting = '';
        if (sortInfo) {
            const columnIndex = sortInfo[0];
            const sortDirection = sortInfo[1];
            // Column map theo plan
            const columnMap = { 1: 'code', 2: 'name', 3: 'districtName', 4: 'provinceName', 5: 'order', 6: 'status' };
            const columnName = columnMap[columnIndex];
            if (columnName) {
                sorting = columnName + ' ' + sortDirection;
            } else {
                // Sắp xếp mặc định cho export theo plan
                sorting = 'provinceName asc, districtName asc, name asc';
            }
        } else {
            // Sắp xếp mặc định cho export theo plan
            sorting = 'provinceName asc, districtName asc, name asc';
        }

        // Build query params theo plan
        const params = new URLSearchParams();
        if (filterInput.filter) params.append('Filter', filterInput.filter);
        if (filterInput.status !== null && filterInput.status !== undefined) params.append('Status', filterInput.status);
        if (filterInput.provinceId) params.append('ProvinceId', filterInput.provinceId);
        if (filterInput.districtId) params.append('DistrictId', filterInput.districtId); // Add district filter theo plan
        if (sorting) params.append('Sorting', sorting);

        // API endpoint theo plan
        const exportUrl = abp.appPath + 'api/app/commune/as-excel?' + params.toString();

        // Điều hướng như plan
        location.href = exportUrl;
    });
});