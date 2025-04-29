(function ($) {
    let l = abp.localization.getResource('CoreFW');
    let form = $('#CreateComponentForm'); // ID của form trong CreateModal.cshtml
    //let uploadModal = new abp.ModalManager("FileManagement/UploadModal");
    let uploadModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'FileManagement/UploadModal',
        scriptUrl: '/Pages/FileManagement/uploadModal.js', // Đường dẫn từ wwwroot
        modalClass: 'uploadModal'
    });
    $('#UploadButton').click(function (e) { e.preventDefault(); uploadModal.open(); });

    // Khởi tạo Select2 (nếu dùng)
    form.find('select[name="ComponentViewModel.ProcedureIds"]').attr('multiple', 'multiple');
    form.find('select[name="ComponentViewModel.ProcedureIds"]').select2({ dropdownParent: form.closest('.modal-content'), width: '100%', theme: 'bootstrap-5' });

})(jQuery);