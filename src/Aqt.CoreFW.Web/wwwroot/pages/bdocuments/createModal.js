// Sử dụng IIFE (Immediately Invoked Function Expression) để tránh xung đột biến toàn cục
(function ($) {
    // Khởi tạo ModalManager cho CreateModal (nếu cần gọi lại chính nó hoặc modal khác)
    // var modalManager = new abp.ModalManager(...);

    // Lấy đối tượng localization resource
    var l = abp.localization.getResource('CoreFW');

    // Tham chiếu đến form và các container quan trọng
    var form = $('#CreateBDocumentForm'); // ID của form trong CreateModal.cshtml
    var declarationContainer = form.find('[id^="DeclarationFormContainer_"]'); // Tìm container form động (ID bắt đầu bằng...)
    // Tìm input ẩn chứa JSON của form động dựa vào ID của container
    var declarationFormDataInput = $('#' + declarationContainer.attr('id').replace('DeclarationFormContainer_', 'hiddenFormData_'));
    var attachedDocsTableBody = form.find('#AttachedDocumentsTable tbody');

    // Tham chiếu đến File App Service proxy (đảm bảo tên proxy đúng)
    var fileAppService = easyAbp.fileManagement.files.file; // Hoặc aqt.coreFW.application.contracts.files.file nếu bạn tạo proxy riêng

    // --- File Upload Logic ---
    function initializeFileUploads(container) {
        // Tìm tất cả input[type=file] trong container được chỉ định
        container.find('.file-upload-input').each(function () {
            var $fileInput = $(this);
            // Lấy target hidden input và info div từ data-attributes
            var targetHiddenId = $fileInput.data('target-hidden');
            var targetInfoId = $fileInput.data('target-info');
            if (!targetHiddenId || !targetInfoId) {
                console.error("File input is missing data-target-hidden or data-target-info attribute.", this);
                return; // Bỏ qua nếu thiếu attribute
            }
            var $hiddenInput = $(targetHiddenId);
            var $infoDiv = $(targetInfoId);

            // Gỡ bỏ event handler cũ và gắn event handler mới
            $fileInput.off('change').on('change', function (event) {
                var file = event.target.files[0];
                if (!file) {
                    $infoDiv.html(''); // Xóa thông tin nếu không chọn file
                    $hiddenInput.val(''); // Xóa FileId
                    return;
                }
                // Hiển thị trạng thái đang upload (ví dụ: spinner)
                $infoDiv.html('<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Loading...</span></div> ' + l('Uploading...'));
                $fileInput.prop('disabled', true); // Vô hiệu hóa input khi đang upload

                var formData = new FormData();
                formData.append('file', file); // Tên 'file' phải khớp với tham số ở backend (OnPostUploadFileAsync)

                // Gọi API Upload bằng AJAX
                $.ajax({
                    url: abp.appPath + 'api/app/file/upload', // Endpoint API upload
                    type: 'POST',
                    data: formData,
                    processData: false, // Không xử lý data
                    contentType: false, // Không set contentType
                    headers: { // Gửi AntiForgeryToken qua header
                        RequestVerificationToken: abp.security.antiForgery.getToken()
                    },
                    success: function (result) {
                        $fileInput.prop('disabled', false); // Bật lại input
                        if (result && result.id) { // Upload thành công và trả về ID file
                            $hiddenInput.val(result.id); // Gán FileId vào hidden input
                            // Hiển thị thông tin file và nút xóa
                            var fileSizeDisplay = result.length ? abp.utils.byteSize(result.length) : '';
                            $infoDiv.html(
                                `<i class="fas fa-check-circle text-success me-1"></i> ${result.fileName} (${fileSizeDisplay}) ` +
                                `<button type="button" class="btn btn-xs btn-outline-danger remove-file ms-1" data-file-id="${result.id}" data-target-hidden="${targetHiddenId}" data-target-input="#${$fileInput.attr('id')}" title="${l('DeleteFile')}"><i class="fas fa-times"></i></button>`
                            );
                            initializeRemoveFileButtons($infoDiv); // Gắn sự kiện cho nút xóa mới
                        } else { // Có lỗi trả về từ server (dù request thành công)
                            var errorMsg = result && result.error ? result.error : l('FileUploadFailed');
                            $infoDiv.html(`<i class="fas fa-exclamation-circle text-danger me-1"></i> ${errorMsg}`);
                            $hiddenInput.val(''); // Xóa FileId nếu lỗi
                            $fileInput.val(''); // Xóa file đã chọn khỏi input
                        }
                    },
                    error: function (jqXhr) { // Lỗi kết nối hoặc server error (500, 4xx...)
                        $fileInput.prop('disabled', false); // Bật lại input
                        let errorMsg = l('FileUploadFailed');
                        if (jqXhr.responseJSON && jqXhr.responseJSON.error && jqXhr.responseJSON.error.message) {
                            errorMsg = jqXhr.responseJSON.error.message;
                        }
                        $infoDiv.html(`<i class="fas fa-exclamation-circle text-danger me-1"></i> ${errorMsg}`);
                        $hiddenInput.val('');
                        $fileInput.val('');
                        console.error("File upload AJAX error:", jqXhr);
                    }
                });
            });
        });
    }

    // --- Remove Uploaded File Logic ---
    function initializeRemoveFileButtons(container) {
        // Tìm các nút xóa file trong container
        container.find('.remove-file').off('click').on('click', function () {
            var $button = $(this);
            var fileIdToRemove = $button.data('file-id'); // Lấy File ID cần xóa (tùy chọn)
            var targetHiddenId = $button.data('target-hidden');
            var targetInputId = $button.data('target-input');
            if (!targetHiddenId || !targetInputId) return;

            var $infoDiv = $button.parent(); // div chứa nút xóa
            var $hiddenInput = $(targetHiddenId);
            var $fileInput = $(targetInputId);

            // Hiển thị xác nhận trước khi xóa
            abp.message.confirm(l('AreYouSureToDeleteFile'), l('AreYouSure')).then((confirmed) => {
                if (confirmed) {
                    // Xóa thông tin hiển thị và giá trị trong input
                    $hiddenInput.val('');
                    $fileInput.val(''); // Xóa file khỏi input control
                    $infoDiv.html(''); // Xóa nội dung div info

                    // Optional: Gọi API để xóa file vật lý khỏi storage nếu cần
                    // if (fileIdToRemove) {
                    //     fileAppService.delete(fileIdToRemove)
                    //         .then(() => console.log("File deleted from storage:", fileIdToRemove))
                    //         .catch((err) => console.error("Error deleting file from storage:", err));
                    // }
                }
            });
        });
    }

    // --- Download Template Logic ---
    function initializeDownloadTemplateLinks(container) {
        container.find('.download-template').off('click').on('click', function (e) {
            e.preventDefault(); // Ngăn hành vi mặc định của thẻ <a>
            var templatePath = $(this).data('path');
            if (templatePath) {
                // Tạo URL đến action handler trong PageModel
                var downloadUrl = abp.appPath + `BDocuments/CreateModal?handler=DownloadTemplate&templatePath=${encodeURIComponent(templatePath)}`;
                // Mở URL trong tab mới để tải file
                window.open(downloadUrl, '_blank');
            } else {
                console.warn("Download template link is missing data-path attribute.");
            }
        });
    }

    // --- Dynamic Form Data Collection ---
    function collectDynamicFormData() {
        var formData = {};
        // Chỉ tìm các control bên trong container của form động
        declarationContainer.find('[data-field-name]').each(function () {
            var $input = $(this);
            var fieldName = $input.data('field-name');
            if (!fieldName) return; // Bỏ qua nếu thiếu data-field-name

            var value;
            var inputType = $input.attr('type');
            var tagName = $input.prop('tagName').toLowerCase();

            if (tagName === 'textarea' || tagName === 'select') {
                value = $input.val();
            } else if (inputType === 'checkbox') {
                // Luôn lấy giá trị boolean true/false cho checkbox
                value = $input.is(':checked');
            } else if (inputType === 'radio') {
                // Chỉ lấy giá trị của radio được check trong group có cùng name
                if ($input.is(':checked')) {
                    value = $input.val();
                    // Gán trực tiếp vào formData vì mỗi group chỉ có 1 giá trị được chọn
                    formData[fieldName] = value;
                }
                // Với radio, không cần gán `value` chung ở cuối, nên return sớm
                return;
            } else { // Các loại input khác (text, date, number, etc.)
                value = $input.val();
            }

            // Gán giá trị vào object formData (trừ radio đã được xử lý)
            if (inputType !== 'radio') {
                formData[fieldName] = value;
            }
        });

        // Chuyển object thành chuỗi JSON
        var jsonString = JSON.stringify(formData);
        // Gán chuỗi JSON vào hidden input tương ứng
        declarationFormDataInput.val(jsonString);
        console.log("Collected Declaration JSON:", jsonString); // Log để debug
        return jsonString; // Trả về chuỗi JSON (tùy chọn)
    }

    // --- Initialization on Modal Load ---
    // Hàm này sẽ được gọi khi modal được tải xong
    $(function () {
        console.log("Create BDocument Modal loaded.");

        // Khởi tạo các chức năng cho bảng file đính kèm
        if (attachedDocsTableBody.length > 0) {
            initializeFileUploads(attachedDocsTableBody);
            // initializeRemoveFileButtons được gọi trong success của upload
            initializeDownloadTemplateLinks(attachedDocsTableBody);
        } else {
            console.warn("Attached documents table body not found.");
        }

        // Bắt sự kiện thay đổi trên các control của form động để cập nhật JSON
        // Điều này hữu ích nếu có logic phụ thuộc vào giá trị form động ngay trên client
        // Nếu không, có thể bỏ qua và chỉ gọi collectDeclarationFormData() khi submit
        if (declarationContainer.length > 0) {
            declarationContainer.on('change input', '[data-field-name]', function () {
                // Có thể gọi collectDeclarationFormData() ở đây nếu cần cập nhật liên tục
                // Hoặc thực hiện các logic client-side khác dựa trên thay đổi
                console.log(`Field '${$(this).data('field-name')}' changed.`);
            });
        } else {
            console.warn("Declaration form container not found.");
        }


        // Đảm bảo JSON được thu thập LẦN CUỐI CÙNG ngay trước khi form submit
        form.on('submit', function (e) {
            // Chỉ thu thập nếu có container form động
            if (declarationContainer.length > 0 && declarationFormDataInput.length > 0) {
                console.log("Collecting final declaration JSON before submit...");
                collectDeclarationFormData();
            } else {
                console.log("No declaration form to collect data from.");
            }
            // Không cần e.preventDefault() ở đây vì modal submit sẽ tự xử lý AJAX
        });

        // Khởi tạo các control đặc biệt khác nếu cần (VD: datepicker, select2...)
        // declarationContainer.find('.datepicker').datepicker({...});
        // attachedDocsTableBody.find('.select2').select2({...});

        console.log("Create BDocument Modal initialization complete.");
    });

})(jQuery); // Kết thúc IIFE