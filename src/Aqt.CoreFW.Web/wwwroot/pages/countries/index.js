$(function () {
    // Get localization resource
    var l = abp.localization.getResource('CoreFW');

    // Get the Country AppService proxy (Namespace might vary based on generation)
    // Verify the exact namespace after running 'abp generate-proxy -t js'
    var countryService = aqt.coreFW.application.countries.country;

    // Initialize ModalManagers for Create and Edit modals
    var createModal = new abp.ModalManager(abp.appPath + 'Countries/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Countries/EditModal');

    var dataTable = null; // Hold the DataTable instance

    // Function to get filter values from the UI
    var getFilters = function () {
        return {
            filter: $('#SearchFilter').val() // Get value from the search input
        };
    }

    // Function to initialize the DataTable
    function initializeDataTable() {
        // Destroy previous instance if it exists to avoid conflicts
        if (dataTable) {
            dataTable.destroy();
        }

        dataTable = $('#CountriesTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[1, "asc"]], // Default sort by the second column (Code)
                searching: false, // Disable built-in search, using custom filter
                scrollX: true,   // Enable horizontal scrolling if needed
                ajax: abp.libs.datatables.createAjax(countryService.getList, getFilters), // Configure AJAX source
                columnDefs: [ // Define table columns
                    {
                        title: l('Actions'),
                        rowAction: { // Define row actions (Edit, Delete)
                            items:
                            [
                                { // Edit Button
                                    text: l('Edit'),
                                    icon: "fa fa-pencil-alt",
                                    // Show only if user has Edit permission
                                    visible: abp.auth.isGranted('CoreFW.Countries.Edit'),
                                    action: function (data) {
                                        // Open the Edit modal with the record's ID
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                { // Delete Button
                                    text: l('Delete'),
                                    icon: "fa fa-trash",
                                    // Show only if user has Delete permission
                                    visible: abp.auth.isGranted('CoreFW.Countries.Delete'),
                                    // Confirmation message before deleting
                                    confirmMessage: function (data) {
                                        return l('AreYouSureToDeleteCountry', data.record.name || data.record.code);
                                    },
                                    action: function (data) {
                                        // Call the delete service method
                                        countryService.delete(data.record.id)
                                            .then(function () {
                                                // Show success notification and reload table
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            }).catch(function (error) {
                                                // Show error message if delete fails (e.g., due to constraints)
                                                abp.message.error(error.message || l('Error'), l('Error'));
                                            });
                                    }
                                }
                            ]
                        }
                    },
                    {
                        title: l('CountryCode'), // Column for Country Code
                        data: "code",
                        orderable: true // Allow sorting
                    },
                    {
                        title: l('CountryName'), // Column for Country Name
                        data: "name",
                        orderable: true // Allow sorting
                    }
                ]
            })
        );
    }

    initializeDataTable(); // Initial setup of the DataTable

    // Reload table data when Create modal is successfully submitted
    createModal.onResult(function () {
        dataTable.ajax.reload();
        abp.notify.success(l('CreatedSuccessfully'));
    });

    // Reload table data when Edit modal is successfully submitted
    editModal.onResult(function () {
        dataTable.ajax.reload();
        abp.notify.success(l('UpdatedSuccessfully'));
    });

    // Open Create modal when the "New Country" button is clicked
    $('#NewCountryButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    // Reload table data when the "Search" button is clicked
    $('#SearchButton').click(function (e) {
        e.preventDefault();
        dataTable.ajax.reload(); // Reloads data using getFilters
    });

    // Reload table data when Enter key is pressed in the search input
    $('#SearchFilter').on('keypress', function (e) {
        if (e.which === 13) { // 13 is the Enter key code
            dataTable.ajax.reload();
        }
    });
}); 