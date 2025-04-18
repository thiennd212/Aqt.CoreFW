$(function () {
    var _wfDesignerManager = new wfDesignerManager();
    function init(schemecode, processid, container, graphwidth = '') {
        _wfDesignerManager.Init(schemecode, processid, container, graphwidth);
    }

    init('FWTEST', '', 'wfdesigner');

    $('#btnSave').click(function () {
        var data = _wfDesignerManager.OnSave();
        console.log(data);
        abp.message.success('Đã lưu thành công');
    });
});
