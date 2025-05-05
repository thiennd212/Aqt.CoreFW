// module easyAbpFileManagement

(function(){

  // controller easyAbp.fileManagement.files.file

  (function(){

    abp.utils.createNamespace(window, 'easyAbp.fileManagement.files.file');

    easyAbp.fileManagement.files.file.get = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.getList = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file' + abp.utils.buildQueryString([{ name: 'parentId', value: input.parentId }, { name: 'fileContainerName', value: input.fileContainerName }, { name: 'ownerUserId', value: input.ownerUserId }, { name: 'directoryOnly', value: input.directoryOnly }, { name: 'filter', value: input.filter }, { name: 'sorting', value: input.sorting }, { name: 'skipCount', value: input.skipCount }, { name: 'maxResultCount', value: input.maxResultCount }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.create = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/with-bytes',
        type: 'POST',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.createWithStream = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/with-stream' + abp.utils.buildQueryString([{ name: 'generateUniqueFileName', value: input.generateUniqueFileName }, { name: 'fileContainerName', value: input.fileContainerName }, { name: 'parentId', value: input.parentId }, { name: 'ownerUserId', value: input.ownerUserId }, { name: 'extraProperties', value: input.extraProperties }]) + '',
        type: 'POST'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.actionCreate = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file',
        type: 'POST',
        data: 'input.FileContainerName=' + input.fileContainerName + '&' + 'input.FileType=' + input.fileType + '&' + 'input.ParentId=' + input.parentId + '&' + 'input.OwnerUserId=' + input.ownerUserId + '&' + 'input.GenerateUniqueFileName=' + input.generateUniqueFileName + '&' + 'input.ExtraProperties=' + input.extraProperties
      }, $.extend(true, {}, { contentType: 'application/x-www-form-urlencoded; charset=UTF-8' }, ajaxParams)));
    };

    easyAbp.fileManagement.files.file.createMany = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/many/with-bytes',
        type: 'POST',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.createManyWithStream = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/many/with-stream' + abp.utils.buildQueryString([{ name: 'generateUniqueFileName', value: input.generateUniqueFileName }, { name: 'fileContainerName', value: input.fileContainerName }, { name: 'parentId', value: input.parentId }, { name: 'ownerUserId', value: input.ownerUserId }, { name: 'extraProperties', value: input.extraProperties }]) + '',
        type: 'POST'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.actionCreateMany = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/many',
        type: 'POST',
        data: 'input.FileContainerName=' + input.fileContainerName + '&' + 'input.FileType=' + input.fileType + '&' + 'input.ParentId=' + input.parentId + '&' + 'input.OwnerUserId=' + input.ownerUserId + '&' + 'input.GenerateUniqueFileName=' + input.generateUniqueFileName + '&' + 'input.ExtraProperties=' + input.extraProperties
      }, $.extend(true, {}, { contentType: 'application/x-www-form-urlencoded; charset=UTF-8' }, ajaxParams)));
    };

    easyAbp.fileManagement.files.file['delete'] = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '',
        type: 'DELETE',
        dataType: null
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.move = function(id, input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '/move',
        type: 'PUT',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.getDownloadInfo = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '/download-info',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.updateInfo = function(id, input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '/info',
        type: 'PUT',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.download = function(id, token, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '/download/with-bytes' + abp.utils.buildQueryString([{ name: 'token', value: token }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.downloadWithStream = function(id, token, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '/download/with-stream' + abp.utils.buildQueryString([{ name: 'token', value: token }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.actionDownload = function(id, token, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/' + id + '/download' + abp.utils.buildQueryString([{ name: 'token', value: token }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.getConfiguration = function(fileContainerName, ownerUserId, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/configuration' + abp.utils.buildQueryString([{ name: 'fileContainerName', value: fileContainerName }, { name: 'ownerUserId', value: ownerUserId }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.getLocation = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/location' + abp.utils.buildQueryString([{ name: 'id', value: id }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.fileManagement.files.file.getByPath = function(path, fileContainerName, ownerUserId, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/file-management/file/by-path' + abp.utils.buildQueryString([{ name: 'path', value: path }, { name: 'fileContainerName', value: fileContainerName }, { name: 'ownerUserId', value: ownerUserId }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

  })();

})();