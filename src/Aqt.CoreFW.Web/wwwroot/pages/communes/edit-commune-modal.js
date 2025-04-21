abp.modals.CommuneEditModal = function () { // Class name matches modalClass in index.js
    const l = abp.localization.getResource('CoreFW'); // Use const for localization resource
    let initialDistrictId = null; // Use let as it can be reassigned

    function initModal(modalManager, args) {
        // Use const for variables that are not reassigned within this scope
        const communeAppService = aqt.coreFW.application.communes.commune;
        if (!communeAppService) {
            console.error("EditModal: Could not find the communeAppService proxy.");
            abp.notify.error(l ? l('ClientSideError') : "Client-side error: Cannot access application service.");
            return;
        }

        const $form = modalManager.getForm();
        if (!$form || $form.length === 0) {
            console.error("EditModal: Could not find the form element.");
            return;
        }

        const $provinceSelect = $form.find('select[name="CommuneViewModel.ProvinceId"]');
        const $districtSelect = $form.find('select[name="CommuneViewModel.DistrictId"]');

        initialDistrictId = $districtSelect.val(); // Assign initial value

        function updateDistrictDropdown(districts) {
            const selectDefaultText = l ? l('SelectAnOption') : '--- Chọn Quận/Huyện ---'; // Use const
            $districtSelect.empty();
            $districtSelect.append($('<option>', { value: '', text: selectDefaultText }));
            let districtFoundAndSelected = false; // Use let as it's updated in the loop
            if (districts && districts.length > 0) {
                $.each(districts, function (i, district) {
                    const $option = $('<option>', { value: district.id, text: district.name }); // Use const
                    if (district.id === initialDistrictId) {
                        $option.prop('selected', true);
                        districtFoundAndSelected = true;
                    }
                    $districtSelect.append($option);
                });
            }
            if (!districtFoundAndSelected) {
                initialDistrictId = null; // Reassign using let is allowed
            }
        }

        $provinceSelect.on('change', function () {
            const selectedProvinceId = $(this).val(); // Use const
            
            initialDistrictId = $districtSelect.val(); // Reassign using let is allowed
            
            $districtSelect.prop('disabled', !selectedProvinceId);

            if (!selectedProvinceId) {
                updateDistrictDropdown([]);
                return;
            }

            communeAppService.getDistrictLookup(selectedProvinceId)
                .then(function (result) {
                    updateDistrictDropdown(result.items);
                })
                .catch(function (error) {
                    console.error("EditModal: Error loading districts via AppService:", error);
                    const errorMsg = l ? l('ErrorLoadingDistricts') : 'Error loading districts.'; // Use const
                    abp.notify.error(errorMsg);
                    updateDistrictDropdown([]);
                });
        });

        // No trigger needed on load

    } // End of initModal

    return {
        initModal: initModal
    };
}; // End of abp.modals.CommuneEditModal 