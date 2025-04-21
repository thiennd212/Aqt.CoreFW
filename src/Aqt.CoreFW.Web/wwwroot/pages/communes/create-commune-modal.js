// src/Aqt.CoreFW.Web/wwwroot/pages/communes/create-commune-modal.js

// --- UPDATED: Structure for ABP Lazy Loading ---
abp.modals.CommuneCreateModal = function () { // Class name matches modalClass in index.js
    const l = abp.localization.getResource('CoreFW'); // Use const

    // Function called by ModalManager upon initialization
    function initModal(modalManager, args) {
        // --- Get the CommuneAppService proxy --- 
        // Assuming it's available in the global namespace as defined in index.js
        // If not, you might need a more robust way to access it.
        const communeAppService = aqt.coreFW.application.communes.commune; // Use const
        if (!communeAppService) {
            console.error("CreateModal: Could not find the communeAppService proxy (aqt.coreFW.application.communes.commune).");
            abp.notify.error(l ? l('ClientSideError') : "Client-side error: Cannot access application service.");
            return; 
        }
        // --- End Get CommuneAppService proxy ---

        const $form = modalManager.getForm(); // Use const
        if (!$form || $form.length === 0) {
            console.error("CreateModal: Could not find the form element using modalManager.getForm().");
            return;
        }

        const $provinceSelect = $form.find('select[name="CommuneViewModel.ProvinceId"]'); // Use const
        const $districtSelect = $form.find('select[name="CommuneViewModel.DistrictId"]'); // Use const

        // Helper function to update district dropdown
        function updateDistrictDropdown(districts) { // Accepts the array of district items
            const selectDefaultText = l ? l('SelectAnOption') : '--- Chọn Quận/Huyện ---'; // Use const
            $districtSelect.empty();
            $districtSelect.append($('<option>', { value: '', text: selectDefaultText }));
            if (districts && districts.length > 0) {
                $.each(districts, function (i, district) {
                     // Assuming district lookup returns items with 'id' and 'name'
                    $districtSelect.append($('<option>', { value: district.id, text: district.name }));
                });
            }
        }

        // Attach change event listener
        $provinceSelect.on('change', function () {
            const selectedProvinceId = $(this).val(); // Use const
            $districtSelect.prop('disabled', !selectedProvinceId);

            if (!selectedProvinceId) {
                updateDistrictDropdown([]); // Clear districts if no province selected
                return;
            }

            // --- UPDATED: Call AppService proxy directly --- 
            communeAppService.getDistrictLookup(selectedProvinceId)
                .then(function (result) {
                    // Assuming result has an 'items' array like other lookups
                    updateDistrictDropdown(result.items);
                })
                .catch(function (error) {
                    console.error("CreateModal: Error loading districts via AppService:", error);
                    const errorMsg = l ? l('ErrorLoadingDistricts') : 'Error loading districts.'; // Use const
                    abp.notify.error(errorMsg);
                    updateDistrictDropdown([]); // Reset dropdown on error
                });
            // --- END UPDATED --- 
        });

        // Initial state handling
        if ($provinceSelect.val()) {
            $provinceSelect.trigger('change'); // Trigger to load initial districts
        } else {
            $districtSelect.prop('disabled', true);
            updateDistrictDropdown([]);
        }

    } // End of initModal

    // Return the required object for ModalManager
    return {
        initModal: initModal
    };
}; // End of abp.modals.CommuneCreateModal