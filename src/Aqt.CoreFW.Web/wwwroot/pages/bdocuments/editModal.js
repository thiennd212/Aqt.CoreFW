// Sử dụng IIFE (Immediately Invoked Function Expression)
(function ($) {
    // Khởi tạo ModalManager cho EditModal (nếu cần gọi lại chính nó hoặc modal khác)
    // var modalManager = new abp.ModalManager(...);

    // Lấy đối tượng localization resource
    var l = abp.localization.getResource('CoreFW');

    // Tham chiếu đến form và các container quan trọng
    var form = $('#EditBDocumentForm'); // <<< ID của form trong EditModal.cshtml
    var declarationContainer = form.find('[id^="DynamicFormContainer_form_"]'); // Tìm container form động
    var attachedDocsTableBody = form.find('#AttachedDocumentsTable tbody');

    // Tham chiếu đến File App Service proxy
    var fileAppService = easyAbp.fileManagement.files.file;

    // --- File Upload Logic (Giống Create) ---
    function initializeFileUploads(container) {
        container.find('.file-upload-input').each(function () {
            var $fileInput = $(this);
            var targetHiddenId = $fileInput.data('target-hidden');
            var targetInfoId = $fileInput.data('target-info');
            if (!targetHiddenId || !targetInfoId) {
                console.error("File input is missing data-target-hidden or data-target-info attribute.", this);
                return;
            }
            var $hiddenInput = $(targetHiddenId);
            var $infoDiv = $(targetInfoId);

            $fileInput.off('change').on('change', function (event) {
                var file = event.target.files[0];
                if (!file) {
                     // Không xóa info cũ nếu chỉ hủy chọn file mới
                     // $infoDiv.html('');
                     // $hiddenInput.val(''); // Giữ lại FileId cũ nếu có
                    return;
                }
                $infoDiv.html('<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Loading...</span></div> ' + l('Uploading...'));
                $fileInput.prop('disabled', true);

                var formData = new FormData();
                formData.append('file', file);

                $.ajax({
                    url: abp.appPath + 'BDocuments/EditModal?handler=UploadFile', // <<< URL handler cho EditModal
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    headers: {
                        RequestVerificationToken: abp.security.antiForgery.getToken()
                    },
                    success: function (result) {
                        $fileInput.prop('disabled', false);
                        if (result && result.id) {
                            $hiddenInput.val(result.id);
                            var fileSizeDisplay = result.byteSize ? app.formatByteSize(result.byteSize) : '';
                            // Xóa nội dung cũ và thêm thông tin mới + nút xóa
                            $infoDiv.empty().html(
                                `<i class="fas fa-check-circle text-success me-1"></i> ${result.fileName} (${fileSizeDisplay}) ` +
                                `<button type="button" class="btn btn-sm btn-outline-danger remove-uploaded-file-btn ms-1" data-file-id="${result.id}" data-target-hidden="${targetHiddenId}" data-target-input="#${$fileInput.attr('id')}" title="${l('DeleteFile')}"><i class="fas fa-times"></i></button>`
                            );
                            initializeRemoveFileButtons($infoDiv); // Gắn lại sự kiện cho nút xóa mới
                        } else {
                            var errorMsg = result && result.error ? result.error : l('FileUploadFailed');
                            // Hiển thị lỗi dưới input thay vì ghi đè info file cũ (nếu có)
                            $infoDiv.append(`<div class='text-danger upload-error-message'><i class="fas fa-exclamation-circle me-1"></i> ${errorMsg}</div>`);
                            // Không xóa hidden input để giữ file cũ nếu upload lỗi
                            $fileInput.val('');
                        }
                    },
                    error: function (jqXhr) {
                        $fileInput.prop('disabled', false);
                        let errorMsg = l('FileUploadFailed');
                        if (jqXhr.responseJSON && jqXhr.responseJSON.error && jqXhr.responseJSON.error.message) {
                            errorMsg = jqXhr.responseJSON.error.message;
                        }
                        $infoDiv.append(`<div class='text-danger upload-error-message'><i class="fas fa-exclamation-circle me-1"></i> ${errorMsg}</div>`);
                        $hiddenInput.val('');
                        $fileInput.val('');
                        console.error("File upload AJAX error:", jqXhr);
                    }
                });
            });
        });
    }

    // --- Remove Uploaded File Logic (Giống Create, nhưng class nút là remove-uploaded-file-btn) ---
    function initializeRemoveFileButtons(container) {
        container.find('.remove-uploaded-file-btn').off('click').on('click', function () {
            var $button = $(this);
            var fileIdToRemove = $button.data('file-id');
            var targetHiddenId = $button.data('target-hidden');
            var targetInputId = $button.data('target-input');
            if (!targetHiddenId || !targetInputId) return;

            var $infoDiv = $button.closest('.file-upload-info'); // Tìm div cha chứa nút
            var $hiddenInput = $(targetHiddenId);
            var $fileInput = $(targetInputId);

            abp.message.confirm(l('AreYouSureToDeleteFile'), l('AreYouSure')).then((confirmed) => {
                if (confirmed) {
                    $hiddenInput.val(''); // Quan trọng: Xóa FileId khỏi hidden input
                    $fileInput.val('');
                    $infoDiv.empty(); // Xóa toàn bộ nội dung div info (thông tin file cũ và nút xóa)

                    // Optional: Gọi API xóa vật lý
                    // if (fileIdToRemove) { ... }
                }
            });
        });
    }

    // --- Download Template Logic (Giống Create) ---
    function initializeDownloadTemplateLinks(container) {
        container.find('.download-template').off('click').on('click', function (e) {
            e.preventDefault();
            var templatePath = $(this).data('path');
            if (templatePath) {
                var downloadUrl = abp.appPath + `BDocuments/EditModal?handler=DownloadTemplate&templatePath=${encodeURIComponent(templatePath)}`; // <<< URL handler cho EditModal
                window.open(downloadUrl, '_blank');
            } else {
                console.warn("Download template link is missing data-path attribute.");
            }
        });
    }

    // --- Dynamic Form Data Collection (Giống Create) ---
    function collectDynamicFormData() {
        var formData = {};
        form.find('.dynamic-form-instance').each(function() {
            var $container = $(this);
            var index = $container.data('index');
            var instanceId = $container.attr('id')?.replace('DynamicFormContainer_', '');
            if (!instanceId) {
                 console.warn(`Form container at index ${index} is missing a valid ID.`);
                 return;
             }
            var hiddenInputId = '#hiddenFormData_' + instanceId;
            var $hiddenInput = $(hiddenInputId);
            if ($hiddenInput.length === 0) {
                  console.warn(`Hidden input not found for form container with index ${index}. Selector: ${hiddenInputId}`);
                  return;
            }

            var currentFormFields = {};
            $container.find('[data-field-name]').each(function () {
                var $input = $(this);
                var fieldName = $input.data('field-name');
                if (!fieldName) return;

                var value;
                var inputType = $input.attr('type');
                var tagName = $input.prop('tagName').toLowerCase();

                if (tagName === 'textarea' || tagName === 'select') {
                    value = $input.val();
                } else if (inputType === 'checkbox') {
                    value = $input.is(':checked');
                } else if (inputType === 'radio') {
                    if ($input.is(':checked')) {
                        currentFormFields[fieldName] = $input.val();
                    }
                    return;
                } else {
                    value = $input.val();
                }
                if (inputType !== 'radio') {
                    currentFormFields[fieldName] = value;
                }
            });
            let jsonStringForm = JSON.stringify(currentFormFields);
            $hiddenInput.val(jsonStringForm);
            formData[instanceId] = currentFormFields;
        });
        let jsonString = JSON.stringify(formData);
        $("#BDocumentViewModel_HiddenDataFormInput").val(jsonString);
        return jsonString;
    }

    // --- Initialization on Modal Load ---
    $(function () {
        console.log("Edit BDocument Modal loaded.");

        // Khởi tạo bảng file đính kèm
        if (attachedDocsTableBody.length > 0) {
            initializeFileUploads(attachedDocsTableBody);
            initializeRemoveFileButtons(attachedDocsTableBody); // <<< Gọi ngay để gắn event cho file cũ
            initializeDownloadTemplateLinks(attachedDocsTableBody);
        } else {
            console.warn("Attached documents table body not found.");
        }

        // Gắn event cho form động
        if (declarationContainer.length > 0) {
             declarationContainer.on('change input', '[data-field-name]', function () {
                 console.log(`Field '${$(this).data('field-name')}' changed.`);
                 // collectDynamicFormData(); // Không cần gọi liên tục, chỉ gọi khi submit
             });
        } else {
             console.warn("Declaration form container not found, skipping change event binding.");
        }

        // Thu thập JSON lần cuối trước khi submit
        form.on('submit', function (e) {
            // Xóa các thông báo lỗi upload cũ trước khi submit lại
            form.find('.upload-error-message').remove();
            if (declarationContainer.length > 0) {
                console.log("Collecting final declaration JSON before submit...");
                collectDynamicFormData();
            } else {
                console.log("No declaration form to collect data from.");
            }
        });

        console.log("Edit BDocument Modal initialization complete.");
    });

})(jQuery); // Kết thúc IIFE