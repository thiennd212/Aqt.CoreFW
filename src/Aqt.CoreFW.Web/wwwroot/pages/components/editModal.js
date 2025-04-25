// Sử dụng IIFE
(function ($) {
    var l = abp.localization.getResource('CoreFW');
    var form = $('#EditComponentForm'); // ID của form trong EditModal.cshtml

    // Khởi tạo Select2 (nếu dùng)
    form.find('select[name="ComponentViewModel.ProcedureIds"]').attr('multiple', 'multiple');
    form.find('select[name="ComponentViewModel.ProcedureIds"]').select2({ dropdownParent: form.closest('.modal-content'), width: '100%' });

})(jQuery);