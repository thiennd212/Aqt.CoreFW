var wfDesignerManager = (function () {
    return function () {
        //Load settings
        var _schemecode = '';
        var _processid = '';
        var _container = '';
        var _graphwidth = window.innerWidth;
        var graphminheight = 660;
        var graphheight = graphminheight;
        var wfdesigner;

        function Init(schemecode, processid, container, graphwidth = '') {
            _schemecode = schemecode;
            _processid = processid;
            _container = container;
            if (graphwidth != '') {
                _graphwidth = parseInt(graphwidth);
            }
            if (!container || container === '') {
                abp.message.error('Không có vị trí render workflow design');
            } else if (!schemecode || schemecode === '') {
                abp.message.error('Không có mã quy trình');
            } else if (_processid != '') {
                wfdesignerDrawWithReadOnly();
            } else {
                wfdesignerRedraw();
            }
        }

        //Recreate designer object
        function wfdesignerDrawWithReadOnly() {
            if (wfdesigner != undefined) {
                wfdesigner.destroy();
            }
            wfdesigner = new WorkflowDesigner({
                name: 'simpledesigner',
                apiurl: '/WorkflowDesigner/API',
                renderTo: _container,
                templatefolder: '/templates/',
                graphwidth: _graphwidth,
                graphheight: graphheight,
                notshowwindows: true
            });
            var isreadonly = false;
            if (_processid != undefined && _processid != '')
                isreadonly = true;
            var p = { schemecode: _schemecode, processid: _processid, readonly: isreadonly };
            if (wfdesigner.exists(p)) {
                wfdesigner.load(p);
            }
            else
                wfdesigner.create(_schemecode);
        }

        function wfdesignerRedraw() {
            var data;
            if (wfdesigner != undefined) {
                wfdesigner.destroy();
            }
            wfdesigner = new WorkflowDesigner({
                name: 'simpledesigner',
                apiurl: '/WorkflowDesigner/API',
                renderTo: _container,
                templatefolder: '/templates/',
                graphwidth: _graphwidth,
                graphheight: graphheight
            });
            if (data == undefined) {
                var isreadonly = false;
                if (_processid != undefined && _processid != '')
                    isreadonly = true;
                var p = { schemecode: _schemecode, processid: _processid, readonly: isreadonly };
                if (wfdesigner.exists(p)) {
                    wfdesigner.load(p);
                }
                else
                    wfdesigner.create(_schemecode);
            }
            else {
                wfdesigner.data = data;
                wfdesigner.render();
            }
        }

        //Adjusts the size of the designer window
        $(window).resize(function () {
            if (window.wfResizeTimer) {
                clearTimeout(window.wfResizeTimer);
                window.wfResizeTimer = undefined;
            }
            window.wfResizeTimer = setTimeout(function () {
                var w = $(window).width();
                var h = $(window).height();
                if (w > 300)
                    _graphwidth = w - 40;
                if (h > 300)
                    graphheight = h - 250;
                if (graphheight < graphminheight)
                    graphheight = graphminheight;
                wfdesigner.resize(_graphwidth, graphheight);
            }, 150);
        });

        //Add Control functions
        function DownloadScheme() {
            wfdesigner.downloadscheme();
        }
        function SelectScheme(type) {
            var file = $('#uploadFile');
            file.trigger('click');
        }
        function UploadScheme(form) {
            if (form.value == "")
                return;
            wfdesigner.uploadscheme($('#uploadform')[0], function () {
                abp.message.info('The file is uploaded!');
            });
        }
        function OnSave() {
            wfdesigner.schemecode = _schemecode;
            var err = wfdesigner.validate();
            if (err != undefined && err.length > 0) {
                return err;
            }
            else {
                wfdesigner.save(function () {
                });
                return 'OK';
            }
        }

        function OnNew() {
            wfdesigner.create();
        }

        return {
            Init: Init,
            OnSave: OnSave,
            OnNew: OnNew,
            DownloadScheme: DownloadScheme,
            SelectScheme: SelectScheme,
            UploadScheme: UploadScheme,
        };
    };
})();    