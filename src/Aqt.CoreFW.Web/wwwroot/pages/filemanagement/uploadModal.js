(function ($) {
    let l = abp.localization.getResource('CoreFW');
    let form = $('#UploadForm');
    $("#file").fileinput({
        showUpload: false,
        minFileSize: -1,
        maxFileSize: 5242880,//5 * 1024 * 1024
        maxFileCount: 2,
        allowedFileExtensions: ['jpg', 'png', 'gif', 'doc', 'docx', 'xls', 'xlsx', 'pdf'],
        previewFileType: 'any',
        theme: "fa5",
        uploadAsync: false,
        showPreview: false
    });

})(jQuery);