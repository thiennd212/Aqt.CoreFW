$(function () {
    const l = abp.localization.getResource('CoreFW');
    const ouAppService = aqt.coreFW.application.organizationUnits.organizationUnit; // Namespace JS Proxy

    // Modal Managers trỏ đến các trang modal riêng biệt
    const createModal = new abp.ModalManager(abp.appPath + 'OrganizationUnits/CreateModal');
    const editModal = new abp.ModalManager(abp.appPath + 'OrganizationUnits/EditModal');

    const $tree = $('#OrganizationUnitTreeContainer');

    function initializeTree() {
        ouAppService.getTree()
            .then(function (result) {
                const jstreeData = result.items.map(item => ({
                    id: item.id,
                    parent: item.parent || '#',
                    text: item.displayName || item.text,
                    state: item.state,
                    icon: item.icon,
                    data: item // Gắn dữ liệu gốc vào node
                }));

                $tree.jstree('destroy'); // Xóa cây cũ nếu có
                $tree.jstree({
                    core: {
                        check_callback: function (operation, node, node_parent, node_position, more) {
                            return operation === "move_node" ? window.ouPermissions?.canMove : true;
                        },
                        themes: { name: 'default' },
                        data: jstreeData
                    },
                    plugins: ['contextmenu', 'dnd', 'state', 'types', 'wholerow'],
                    //types: {
                    //    default: { icon: "fa fa-sitemap text-primary" }
                    //},
                    contextmenu: {
                        items: function (node) {
                            let items = {};
                            if (ouPermissions.canCreate) {
                                items.create = {
                                    label: l('AddSubUnit'),
                                    icon: 'fas fa-plus',
                                    action: () => createModal.open({ parentId: node.id })
                                };
                            }
                            if (ouPermissions.canUpdate) {
                                items.edit = {
                                    label: l('Edit'),
                                    icon: 'fas fa-pencil-alt',
                                    action: () => editModal.open({ id: node.id })
                                };
                            }
                            if (ouPermissions.canDelete) {
                                items.remove = {
                                    label: l('Delete'),
                                    icon: 'fas fa-trash',
                                    action: () => {
                                        abp.message.confirm(
                                            l('AreYouSureToDeleteOrganizationUnit', node.text),
                                            l('AreYouSure'),
                                            isConfirmed => {
                                                if (isConfirmed) {
                                                    ouAppService.delete(node.id).then(() => {
                                                        abp.notify.success(l('SuccessfullyDeleted'));
                                                        $tree.jstree(true).delete_node(node);
                                                    });
                                                }
                                            }
                                        );
                                    }
                                };
                            }
                            return items;
                        }
                    },
                    dnd: {
                        is_draggable: () => ouPermissions.canMove
                    }
                });

                // Mở toàn bộ cây sau khi tải xong
                //$tree.on('loaded.jstree', function () {
                //    $tree.jstree(true).open_all();
                //});

                // Xử lý sự kiện kéo thả
                $tree.on('move_node.jstree', function (e, data) {
                    if (!ouPermissions.canMove) return;
                    let moveInput = {
                        id: data.node.id,
                        newParentId: data.parent === '#' ? null : data.parent
                    };
                    ouAppService.move(moveInput).then(() => {
                        abp.notify.success(l('SuccessfullyMoved'));
                    }).catch(error => {
                        abp.notify.error(error.message || l('ErrorOccurred'));
                        $.jstree.rollback(data.rlbk);
                    });
                });
            })
            .catch(function (error) {
                console.error("Error loading tree data:", error);
                abp.notify.error(l('ErrorOccurredWhileLoadingOrganizationUnits') || 'Error loading data.');
            });
    }


    initializeTree();

    // Xử lý nút "Thêm đơn vị gốc"
    $('#AddRootUnitButton').on('click', (e) => {
        e.preventDefault(); 
        if (!ouPermissions.canCreate) return; // Kiểm tra quyền
        // Mở modal tạo, parentId là null
        createModal.open({ parentId: null }); 
    });

    // Callback sau khi modal Create/Edit đóng thành công
    createModal.onResult(() => { 
        abp.notify.success(l('SuccessfullySaved')); 
        $tree.jstree(true).refresh(); // Refresh lại cây
    });
    editModal.onResult(() => { 
        abp.notify.success(l('SuccessfullySaved')); 
        $tree.jstree(true).refresh(); // Refresh lại cây
    });
}); 