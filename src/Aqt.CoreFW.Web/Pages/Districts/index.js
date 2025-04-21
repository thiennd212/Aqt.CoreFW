                    ajax: abp.libs.datatables.createAjax(districtAppService.getList, function() {
                        return {
                            filter: $('#SearchFilter').val(),
                            status: $('#StatusFilter').val() === "" ? null : parseInt($('#StatusFilter').val()),
                            provinceId: $('#ProvinceFilter').val() === "" ? null : $('#ProvinceFilter').val()
                        };
                    }),
                    columnDefs: [
                        {
                            title: l('Actions'),
                            rowAction: { /* ... row actions ... */ }
                        },
                        { title: l('DisplayName:District.Code'), data: "code", orderable: true },
                        { title: l('DisplayName:District.Name'), data: "name", orderable: true },
                        { title: l('DisplayName:District.ProvinceName'), data: "provinceName", orderable: false },
                        { title: l('DisplayName:District.Order'), data: "order", orderable: true },
                        {
                            title: l('DisplayName:District.Status'), data: "status", orderable: true,
                            render: (data) => l('Enum:DistrictStatus.' + data)
                        },
                        { title: l('DisplayName:District.Description'), data: "description", orderable: false },
                    ]
                })
            );
        }
    }
}
