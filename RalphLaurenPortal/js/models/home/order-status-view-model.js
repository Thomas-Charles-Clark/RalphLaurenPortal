const orderStatusViewModel = ((resources, parent) => {

    let self = {
        parent: parent,
        resources: resources
    };

    let kendoGridPagerMessages = {
        display: self.resources[''],
        empty: self.resources[''],
        page: self.resources[''],
        of: self.resources[''],
        itemsPerPage: self.resources[''],
        first: self.resources[''],
        last: self.resources[''],
        next: self.resources[''],
        previous: self.resources[''],
        refresh: self.resources[''],
        morePages: self.resources['']
    };

    //#region Properties

    self.ready = ko.observable(false);

    self.outstandingOrdersBusy = ko.observable(false);
    self.backOrdersBusy = ko.observable(false);
    self.reserveOrdersBusy = ko.observable(false);
    self.despatchedOrdersBusy = ko.observable(false);
    self.ordersInProgressBusy = ko.observable(false);
    self.heldOrdersBusy = ko.observable(false);

    self.showLongSearch = ko.observable(false);
    self.exportingBusy = ko.observable(false);
    self.exportingProgress = ko.observable(0);

    self.outstandingOrders = ko.observableArray([]);
    self.backOrders = ko.observableArray([]);
    self.reserveOrders = ko.observable([]);
    self.despatchedOrders = ko.observableArray([]);
    self.ordersInProgress = ko.observableArray([]);
    self.heldOrders = ko.observableArray([]);

    self.trackingOrderNumber = ko.observable();
    self.trackingResults = ko.observableArray([]);

    self.orderDetailsOrderNumber = ko.observable();
    self.orderDetailsOrderDate = ko.observable();
    self.orderDetailsDeliveryInfo = ko.observable();
    self.orderDetailsOrderLines = ko.observableArray([]);

    self.viewingStatus = ko.observable('outstanding');

    self.searchTerm = ko.observable();

    self.sampleOrders = ko.observable(false);

    self.validateSearchNow = ko.observable(false);

    self.dateRange = ko.observable(6);

    self.dateRangeType = ko.observable(1);

    self.dateRanges = [{
        dateRangeId: 12,
        name: self.resources.Last12Months
    }, {
        dateRangeId: 6,
        name: self.resources.Last6Months
    }, {
        dateRangeId: 1,
        name: self.resources.ThisMonth
    }, {
        dateRangeId: -1,
        name: self.resources.Custom
    }];

    self.allDateRangeTypes = [{
        dateRangeTypeId: 1,
        name: self.resources.OrderDate
    }, {
        dateRangeTypeId: 2,
        name: self.resources.StatusDate
    }, {
        dateRangeTypeId: 3,
        name: self.resources.DueDate
    }];

    self.dateRangeTypes = ko.observableArray();

    self.customFromDate = ko.observable();

    self.customToDate = ko.observable();

    self.customFromDate.extend({
        date: {
            params: true,
            message: self.resources.FromMustBeDate
        },
        required: {
            params: true,
            onlyIf: function () {
                return self.validateSearchNow() && self.dateRange() == -1 && self.customToDate() != null
            },
            message: self.resources.PleaseProvideFromDate
        }
    });

    self.customToDate.extend({
        date: {
            params: true,
            message: self.resources.ToMustBeDate
        }
    });

    self.searchErrors = ko.validation.group([
        self.customFromDate,
        self.customToDate,
        self.fromPreceedsTo
    ]);

    self.customFromDatePicker = {
        widget: ko.observable(),
        value: self.customFromDate,
        format: 'dd/MM/yyyy',
        clear: function () {
            self.customFromDate(undefined);
            self.customFromDatePicker.widget().value(null);
            self.customFromDatePicker.widget().trigger('change');
        }
    };

    self.customToDatePicker = {
        widget: ko.observable(),
        value: self.customToDate,
        format: 'dd/MM/yyyy',
        clear: function () {
            self.customToDate(undefined);
            self.customToDatePicker.widget().value(null);
            self.customToDatePicker.widget().trigger('change');
        }
    };

    self.trackingOrderNumber = ko.observable();

    self.trackingResults = ko.observableArray([]);

    self.schema = {
        model: {
            id: 'OrderNumber',
            fields: {
                OrderNumber: {},
                LineNumber: {},
                OrderDate: { type: 'date' },
                OrderDateDisplay: {},
                OrderQuantity: {},
                Product: {},
                Batch: {},
                Description: {},
                CustomerOrderNumber: {},
                CustomerOrderNumberDisplay: {},
                CustomerOrderReference: {},
                Status: {},
                Tracking: {},
                BaseProduct: {},
                DespatchStatusDate: { type: 'date' },
                DespatchStatusDateDisplay: {},
                DespatchStatusDateMessage: { type: 'date' },
                ReservedUntil: {},
                ReservedUntilDisplay: {},
                ReservedUntilMessage: {},
                ReserveAction: {}
            }
        }
    };

    self.outstandingOrdersGrid = {
        busy: self.outstandingOrdersBusy,
        widget: ko.observable(),
        data: self.outstandingOrders,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: [{
                field: 'OrderDate',
                dir: 'desc'
            }, {
                field: 'OrderNumber',
                dir: 'desc'
            }],
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'OrderNumber',
            title: self.resources.OrderNo,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: OrderNumber #</span>'
        }, {
            field: 'OrderDate',
            title: self.resources.OrderDate,
            width: 110,
            template: "#: OrderDate ? OrderDateDisplay : '' #"
        }, {
            field: 'OrderQuantity',
            title: self.resources.Quantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'Product',
            title: self.resources.Product,
            width: 120
        }, {
            field: 'Description',
            title: self.resources.Description,
            width: 140
        }, {
            field: 'Batch',
            title: self.resources.Batch,
            width: 120,
            template: (dataItem) => {
                let batches = new Set(dataItem.Batch.split('|'));
                return ('\n', [...batches]).join('\n');
            }
        }, {
            field: 'CustomerOrderNumberDisplay',
            title: self.resources.YourOrderReference,
            width: 140
        }, {
            field: 'CustomerOrderReference',
            title: self.resources.YourLineReference,
            width: 140
        }, {
            field: 'Status',
            title: self.resources.Status,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: Status #</span>'
        }, {
            field: 'Tracking',
            title: self.resources.Tracking,
            width: 100,
            template: function (dataItem) {
                if (dataItem) {
                    if (dataItem.Tracking) {
                        if (dataItem.Tracking.indexOf('http') == 0)
                            return '<a href="' + dataItem.Tracking + '" target="_blank">' + self.resources.Track + '</a>'
                        else
                            return '<a href="javascript:;" onclick="pageModel.orderStatusModel.checkParcel(\'' + dataItem.Tracking + '\')">' + self.resources.Track + '</a>'
                    } else {
                        return ''
                    }
                }
            }
        }],
        sortable: true,
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            if (dataItem)
                self.fetchOrderDetails(dataItem.OrderNumber, dataItem.OrderDate);
        },
        dataBound: function (e) {
            let grid = e.sender;
            grid.autoFitColumn(grid.columns);
        },
        excelExport: self.exportToExcel
    };

    self.backOrdersGrid = {
        busy: self.backOrdersBusy,
        widget: ko.observable(),
        data: self.backOrders,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: {
                field: 'OrderDate',
                dir: 'desc'
            },
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'OrderNumber',
            title: self.resources.OrderNo,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: OrderNumber #</span>'
        }, {
            field: 'OrderDate',
            title: self.resources.OrderDate,
            width: 110,
            template: "#: OrderDate ? OrderDateDisplay : '' #"
        }, {
            field: 'OrderQuantity',
            title: self.resources.Quantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'Product',
            title: self.resources.Product,
            width: 120
        }, {
            field: 'Description',
            title: self.resources.Description,
            width: 140
        }, {
            field: 'CustomerOrderNumberDisplay',
            title: self.resources.YourOrderReference,
            width: 140
        }, {
            field: 'CustomerOrderReference',
            title: self.resources.YourLineReference,
            width: 200
        }, {
            field: 'DespatchStatusDate',
            title: self.resources.DueDate,
            width: 100,
            template: "#: DespatchStatusDateDisplay ? DespatchStatusDateDisplay : DespatchStatusDateMessage #"
        }],
        sortable: true,
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            if (dataItem)
                self.fetchOrderDetails(dataItem.OrderNumber, dataItem.OrderDate);
        },
        dataBound: function (e) {
            let grid = e.sender;
            grid.autoFitColumn(grid.columns);
        },
        excelExport: self.exportToExcel
    };

    self.reserveOrdersGrid = {
        busy: self.reserveOrdersBusy,
        widget: ko.observable(),
        data: self.reserveOrders,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: {
                field: 'OrderDate',
                dir: 'desc'
            },
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'OrderNumber',
            title: self.resources.OrderNo,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: OrderNumber #</span>'
        }, {
            field: 'OrderDate',
            title: self.resources.OrderDate,
            width: 110,
            template: "#: OrderDate ? OrderDateDisplay : '' #"
        }, {
            field: 'OrderQuantity',
            title: self.resources.Quantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'Product',
            title: self.resources.Product,
            width: 120
        }, {
            field: 'Description',
            title: self.resources.Description,
            width: 140
        }, {
            field: 'Batch',
            title: self.resources.Batch,
            width: 120
        }, {
            field: 'CustomerOrderNumberDisplay',
            title: self.resources.YourOrderReference,
            width: 140
        }, {
            field: 'CustomerOrderReference',
            title: self.resources.YourLineReference,
            width: 200
        }, {
            field: 'ReservedUntil',
            title: self.resources.ExpiryDate,
            width: 100,
            template: "#: ReservedUntil ? ReservedUntilDisplay : ReservedUntil #",
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }],
        //, {
        //    field: 'ReserveAction',
        //    title: 'Action'
        //}],
        sortable: true,
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            if (dataItem)
                self.fetchOrderDetails(dataItem.OrderNumber, dataItem.OrderDate);
        },
        dataBound: function (e) {
            let grid = e.sender;
            grid.autoFitColumn(grid.columns);
        },
        excelExport: self.exportToExcel
    };

    self.despatchedOrdersGrid = {
        busy: self.despatchedOrdersBusy,
        widget: ko.observable(),
        data: self.despatchedOrders,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: {
                field: 'OrderDate',
                dir: 'desc'
            },
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'OrderNumber',
            title: self.resources.OrderNo,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: OrderNumber #</span>'
        }, {
            field: 'OrderDate',
            title: self.resources.OrderDate,
            width: 110,
            template: "#: OrderDate ? OrderDateDisplay : '' #"
        }, {
            field: 'OrderQuantity',
            title: self.resources.Quantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'Product',
            title: self.resources.Product,
            width: 120
        }, {
            field: 'Description',
            title: self.resources.Description,
            width: 140
        }, {
            field: 'Batch',
            title: self.resources.Batch,
            width: 120
        }, {
            field: 'CustomerOrderNumberDisplay',
            title: self.resources.YourOrderReference,
            width: 140
        }, {
            field: 'CustomerOrderReference',
            title: self.resources.YourLineReference,
            width: 200
        }, {
            field: 'DespatchStatusDateMessage',
            title: self.resources.DespatchDate,
            width: 115,
            template: "#: DespatchStatusDateDisplay ? DespatchStatusDateDisplay : DespatchStatusDateMessage #"
        }, {
            field: 'Tracking',
            title: self.resources.Tracking,
            width: 100,
            template: function (dataItem) {
                if (dataItem) {
                    if (dataItem.Tracking) {
                        if (dataItem.Tracking.indexOf('http') == 0)
                            return '<a href="' + dataItem.Tracking + '" target="_blank">' + self.resources.Track + '</a>'
                        else
                            return '<a href="javascript:;" onclick="pageModel.orderStatusModel.checkParcel(\'' + dataItem.Tracking + '\')">' + self.resources.Track + '</a>'
                    } else {
                        return ''
                    }
                }
            }
        }],
        sortable: true,
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            if (dataItem)
                self.fetchOrderDetails(dataItem.OrderNumber, dataItem.OrderDate);
        },
        dataBound: function (e) {
            let grid = e.sender;
            grid.autoFitColumn(grid.columns);
        },
        excelExport: self.exportToExcel
    };

    self.ordersInProgressGrid = {
        busy: self.ordersInProgressBusy,
        widget: ko.observable(),
        data: self.ordersInProgress,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: {
                field: 'OrderDate',
                dir: 'desc'
            },
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'OrderNumber',
            title: self.resources.OrderNo,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: OrderNumber #</span>'
        }, {
            field: 'OrderDate',
            title: self.resources.OrderDate,
            width: 110,
            template: "#: OrderDate ? OrderDateDisplay : '' #"
        }, {
            field: 'OrderQuantity',
            title: self.resources.Quantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'Product',
            title: self.resources.Product,
            width: 120
        }, {
            field: 'Description',
            title: self.resources.Description
        }, {
            field: 'Batch',
            title: self.resources.Batch,
            width: 120
        }, {
            field: 'CustomerOrderNumberDisplay',
            title: self.resources.YourOrderReference,
            width: 140
        }, {
            field: 'CustomerOrderReference',
            title: self.resources.YourLineReference,
            width: 200
        }, {
            field: 'DespatchStatusDateMessage',
            title: self.resources.StatusDate,
            width: 115,
            template: "#: DespatchStatusDateDisplay ? DespatchStatusDateDisplay : DespatchStatusDateMessage #"
        }, {
            field: 'Status',
            title: self.resources.Status,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: Status #</span>'
        }],
        sortable: true,
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            if (dataItem)
                self.fetchOrderDetails(dataItem.OrderNumber, dataItem.OrderDate);
        },
        dataBound: function (e) {
            let grid = e.sender;
            grid.autoFitColumn(grid.columns);
        },
        excelExport: self.exportToExcel
    };

    self.heldOrdersGrid = {
        busy: self.heldOrdersBusy,
        widget: ko.observable(),
        data: self.heldOrders,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: [{
                field: 'OrderDate',
                dir: 'desc'
            }, {
                field: 'OrderNumber',
                dir: 'desc'
            }],
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'OrderNumber',
            title: self.resources.OrderNo,
            width: 100,
            template: '<span class="grid-msg" title="' + self.resources.StockistClickHere + '">#: OrderNumber #</span>'
        }, {
            field: 'OrderDate',
            title: self.resources.OrderDate,
            width: 110,
            template: "#: OrderDate ? OrderDateDisplay : '' #"
        }, {
            field: 'OrderQuantity',
            title: self.resources.Quantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'Product',
            title: self.resources.Product,
            width: 120
        }, {
            field: 'Description',
            title: self.resources.Description
        }, {
            field: 'Batch',
            title: self.resources.Batch,
            width: 120
        }, {
            field: 'CustomerOrderNumberDisplay',
            title: self.resources.YourOrderReference,
            width: 140
        }, {
            field: 'CustomerOrderReference',
            title: self.resources.YourLineReference,
            width: 200
        }, {
            field: 'Status',
            title: self.resources.Status,
            width: 100
        }],
        sortable: true,
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            if (dataItem)
                self.fetchOrderDetails(dataItem.OrderNumber, dataItem.OrderDate);
        },
        dataBound: function (e) {
            let grid = e.sender;
            grid.autoFitColumn(grid.columns);
        },
        excelExport: self.exportToExcel
    };

    self.ordersDetailsGrid = {
        widget: ko.observable(),
        data: self.orderDetailsOrderLines,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: {
                field: 'LineNumber',
                dir: 'asc'
            },
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'LineNumber',
            title: self.resources.LineNo,
            hidden: true
        }, {
            field: 'Product',
            title: self.resources.Product,
            width: 120
        }, {
            field: 'Description',
            title: self.resources.Description,
            width: 140
        }, {
            field: 'Batch',
            title: self.resources.Batch,
            width: 120
        }, {
            field: 'Quantity',
            title: self.resources.Quantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'BatchNumber',
            title: self.resources.BatchNo,
            width: 100,
            template: (dataItem) => {
                let batches = new Set(dataItem.BatchNumber.split('|'));
                return ('\n', [...batches]).join('\n');
            }
        }, {
            field: 'CustomerOrderNumberDisplay',
            title: self.resources.YourOrderReference,
            width: 140
        }, {
            field: 'CustomerOrderReference',
            title: self.resources.YourLineReference,
            width: 115
        }, {
            field: 'OrderStatus',
            title: self.resources.Status,
            width: 90,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            },
            template: '#: OrderStatus #'
        }, {
            field: 'DespatchStatus',
            title: self.resources.Despatch,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            }
        }, {
            field: 'Tracking',
            title: self.resources.Track,
            width: 100,
            template: function (dataItem) {
                if (dataItem) {
                    if (dataItem.Tracking) {
                        if (dataItem.Tracking.indexOf('http') == 0)
                            return '<a href="' + dataItem.Tracking + '" target="_blank">' + self.resources.Track + '</a>'
                        else
                            return '<a href="javascript:;" onclick="pageModel.orderStatusModel.checkParcel(\'' + dataItem.Tracking + '\')">' + self.resources.Track + '</a>'
                    } else {
                        return ''
                    }
                }
            }
        }],
        sortable: true,
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            //window.open('/downloads/trade-document?docref=' + encodeURIComponent(dataItem.DocumentNumber) + '&c=' + getCountryID() + '&l=' + getLanguageID(), '_blank');
        },
        dataBound: function (e) {
            let grid = e.sender;
            grid.autoFitColumn(grid.columns);
        },
        excelExport: self.exportToExcel
    };

    //#endregion


    //#region Components



    //#endregion


    //#region Subscriptions

    self.dateRange.subscribe(function (newValue) {
        if (newValue != -1) {
            self.refresh(self.viewingStatus(), true);
        } else {
            var from = new Date();
            from.setMonth(from.getMonth() - 1);
            self.customFromDate(from);
            self.refresh(self.viewingStatus(), true);
        }
    });

    self.dateRangeType.subscribe(function (newValue) {
        if (self.dateRange() != -1) {
            self.refresh(self.viewingStatus(), true);
        } else {
            var from = new Date();
            from.setMonth(from.getMonth() - 1);
            self.customFromDate(from);
            self.refresh(self.viewingStatus(), true);
        }
    });

    self.sampleOrders.subscribe(function (newValue) {
        self.search();
    });

    //#endregion


    //#region Computeds

    self.showSampleControl = ko.pureComputed(function () {
        switch (self.viewingStatus()) {
            case 'backOrders':
            case 'despatchedOrders':
            case 'ordersInProgress':
                return true;
            default:
                return false;
        }
    }, self);

    self.availableDateRangeTypes = ko.pureComputed(function () {
        switch (self.viewingStatus()) {
            case 'outstanding':
                return [{
                    dateRangeTypeId: 1,
                    name: self.resources.OrderDate
                }]
                break;
            case 'backOrders':
                return [{
                    dateRangeTypeId: 1,
                    name: self.resources.OrderDate
                }, {
                    dateRangeTypeId: 5,
                    name: self.resources.DueDate
                }]
                break;
            case 'reserveOrders':
                return [{
                    dateRangeTypeId: 1,
                    name: self.resources.OrderDate
                }, {
                    dateRangeTypeId: 4,
                    name: self.resources.ExpiryDate
                }]
                break;
            case 'despatchedOrders':
                return [{
                    dateRangeTypeId: 1,
                    name: self.resources.OrderDate
                }, {
                    dateRangeTypeId: 3,
                    name: self.resources.DespatchDate
                }]
                break;
            case 'ordersInProgress':
                return [{
                    dateRangeTypeId: 1,
                    name: self.resources.OrderDate
                }, {
                    dateRangeTypeId: 2,
                    name: self.resources.StatusDate
                }]
            case 'held':
                return [{
                    dateRangeTypeId: 1,
                    name: self.resources.OrderDate
                }]
                break;
                break;
        }
    }, self);

    self.fromPreceedsTo = ko.pureComputed(function () {
        if (self.customFromDate() && self.customToDate()) {
            return self.customFromDate().getTime() <= self.customToDate().getTime();
        }
    }, self).extend({
        equal: {
            params: true,
            onlyIf: function () {
                return self.customFromDate() && self.customToDate()
            },
            message: self.resources.FromMustPreceedTo
        }
    });

    //#endregion


    //#region Methods

    self.search = function () {
        self.refresh(self.viewingStatus(), true);
    }

    self.refresh = function (orderStatus, force, clean) {
        if (clean) {
            //self.dateRange(12);
            //self.dateRangeType(?);
            self.searchTerm(undefined);
        }

        self.validateSearchNow(true);

        self.searchErrors.showAllMessages(false);

        window.setTimeout(function () {
            if (self.searchErrors().length) {
                self.searchErrors.showAllMessages(true);
                self.searchErrors.filter(function (x) {
                    x.notifySubscribers(x.isValid())
                });
            } else {

                var data = {
                    orderStatus: orderStatus,
                    dateRange: self.dateRange(),
                    dateRangeType: self.dateRangeType(),
                    searchTerm: self.searchTerm()
                };

                if (self.dateRange() == -1) {
                    data.fromDate = self.customFromDate() ? self.customFromDate().toISOString() : null;
                    data.toDate = self.customToDate() ? self.customToDate().toISOString() : null;
                }

                var affectingGrid = null;
                var affectingArray = null;
                var affectingBusy = null;

                switch (orderStatus) {
                    case 'outstanding':
                        affectingGrid = self.outstandingOrdersGrid.widget;
                        affectingArray = self.outstandingOrders;
                        affectingBusy = self.outstandingOrdersBusy;
                        break;
                    case 'backOrders':
                        affectingGrid = self.backOrdersGrid.widget;
                        affectingArray = self.backOrders;
                        affectingBusy = self.backOrdersBusy;
                        data.sampleOrders = self.sampleOrders();
                        break;
                    case 'reserveOrders':
                        affectingGrid = self.reserveOrdersGrid.widget;
                        affectingArray = self.reserveOrders;
                        affectingBusy = self.reserveOrdersBusy;
                        break;
                    case 'despatchedOrders':
                        affectingGrid = self.despatchedOrdersGrid.widget;
                        affectingArray = self.despatchedOrders;
                        affectingBusy = self.despatchedOrdersBusy;
                        data.sampleOrders = self.sampleOrders();
                        break;
                    case 'ordersInProgress':
                        affectingGrid = self.ordersInProgressGrid.widget;
                        affectingArray = self.ordersInProgress;
                        affectingBusy = self.ordersInProgressBusy;
                        data.sampleOrders = self.sampleOrders();
                        break;
                    case 'held':
                        affectingGrid = self.heldOrdersGrid.widget;
                        affectingArray = self.heldOrders;
                        affectingBusy = self.heldOrdersBusy;
                        data.sampleOrders = self.sampleOrders();
                        break;
                }

                if ((affectingArray().length == 0) || force) {
                    affectingArray([]);
                    affectingBusy(true);

                    self.longSearchTimeout = window.setTimeout(function () {
                        self.showLongSearch(true);
                    }, 30000);

                    fetch('/api/generalapi/OutstandingOrderSearch', {
                        method: 'POST',
                        headers: window.fetchHeaders,
                        body: JSON.stringify(data)
                    })
                        .then(window.handleFetchResponse)
                        .then(data => {
                            window.clearTimeout(self.longSearchTimeout);
                            self.showLongSearch(false);
                            affectingArray(data);
                            affectingBusy(false);
                            self.validateSearchNow(false);
                            affectingGrid().dataSource.query({
                                page: 1,
                                pageSize: affectingGrid().dataSource.pageSize(),
                                sort: affectingGrid().dataSource.sort()
                            });
                        })
                        .catch(err => {
                            affectingBusy(false);
                            window.catchFetchErrors(err);
                        })

                    //$.ajax({
                    //    url: '/api/generalapi/OutstandingOrderSearch',
                    //    type: 'POST',
                    //    dataType: 'json',
                    //    data: data,
                    //    success: function (data) {
                    //        window.clearTimeout(self.longSearchTimeout);
                    //        self.showLongSearch(false);
                    //        affectingArray(data);
                    //        affectingBusy(false);
                    //        self.validateSearchNow(false);
                    //        affectingGrid().dataSource.query({
                    //            page: 1,
                    //            pageSize: affectingGrid().dataSource.pageSize(),
                    //            sort: affectingGrid().dataSource.sort()
                    //        });
                    //    },
                    //    error: function (xhr, s, a) {
                    //        showError();
                    //        affectingBusy(false);
                    //    },
                    //    timeout: 120000
                    //});
                }
            }
        }, 0);
    }

    self.checkParcel = function (text) {

        // this because the way we pull tracking data changed - it can now potentially
        // directly return the sole tracking URL, but it can still also potentially
        // return an indication there were multiple parcels.
        if (text.indexOf('http') == 0) {
            window.open(text, '_blank');
            return;
        }

        var affectingBusy = null;

        switch (self.viewingStatus()) {
            case 'outstanding':
                affectingBusy = self.outstandingOrdersBusy;
                break;
            case 'backOrders':
                affectingBusy = self.backOrdersBusy;
                break;
            case 'reserveOrders':
                affectingBusy = self.reserveOrdersBusy;
                break;
            case 'despatchedOrders':
                affectingBusy = self.despatchedOrdersBusy;
                break;
            case 'ordersInProgress':
                affectingBusy = self.ordersInProgressBusy;
                break;
            case 'held':
                affectingBusy = self.heldOrdersBusy;
                break;
        }

        affectingBusy(true);

        fetch('/api/generalapi/GetParcelCount', {
            method: 'POST',
            headers: window.fetchHeaders,
            cache: 'no-cache',
            credentials: 'same-origin',
            body: JSON.stringify({
                documentNumber: text
            })
                .then(window.handleFetchResponse)
                .then(data => {
                    affectingBusy(false);
                    if (data.Results.length == 1) {
                        window.open(data.Results[0].TrackingURL, '_blank');
                    } else {
                        self.trackingOrderNumber(text.split(',')[1]);
                        self.trackingResults(data.Results);
                        self.showTrackingModal();
                    }
                })
                .catch(err => {
                    affectingBusy(false);
                    window.catchFetchErrors(err);
                })
        });
    }

    self.showTrackingModal = function () {
        $('#tracking-modal').modal('show');
    }

    self.hideTrackingModal = function () {
        self.trackingResults([]);
        $('#tracking-modal').modal('hide');
    }

    self.showOrderDetailModal = function () {
        $('#order-detail-modal').modal('show');
    }

    self.hideOrderDetailModal = function () {
        $('#order-detail-modal').modal('hide');
    }

    self.fetchOrderDetails = function (orderNumber, orderDate) {

        var affectingBusy = null;

        switch (self.viewingStatus()) {
            case 'outstanding':
                affectingBusy = self.outstandingOrdersBusy;
                break;
            case 'backOrders':
                affectingBusy = self.backOrdersBusy;
                break;
            case 'reserveOrders':
                affectingBusy = self.reserveOrdersBusy;
                break;
            case 'despatchedOrders':
                affectingBusy = self.despatchedOrdersBusy;
                break;
            case 'ordersInProgress':
                affectingBusy = self.ordersInProgressBusy;
                break;
            case 'held':
                affectingBusy = self.heldOrdersBusy;
                break;
        }

        affectingBusy(true);

        //$.ajax({
        //    url: '/api/generalapi/GetOrderDeliveryInfo',
        //    type: 'POST',
        //    dataType: 'json',
        //    data: {
        //        orderNumber: orderNumber
        //    },
        //    success: function (data) {
        //        console.log('GetOrderDeliveryInfo returned', data);

        //        self.orderDetailsOrderDate(kendo.toString(kendo.parseDate(orderDate, 'yyyy-MM-ddTHH:mm:ss'), 'MM/dd/yyyy'));
        //        self.orderDetailsOrderNumber(orderNumber);
        //        self.orderDetailsDeliveryInfo(data.DeliveryInfo);
        //        self.orderDetailsOrderLines(data.OrderLines);

        //        affectingBusy(false);

        //        self.showOrderDetailModal();
        //    },
        //    error: function (xhr, s, a) {
        //        affectingBusy(false);
        //        window.catchFetchError(err);
        //    }
        //});

        fetch('/api/generalapi/GetOrderDeliveryInfo', {
            method: 'POST',
            headers: window.fetchHeaders,
            cache: 'no-cache',
            credentials: 'same-origin',
            body: JSON.stringify({
                orderNumber: orderNumber
            })
                .then(window.handleFetchResponse)
                .then(data => {

                    console.log('GetOrderDeliveryInfo returned', data);

                    self.orderDetailsOrderDate(kendo.toString(kendo.parseDate(orderDate, 'yyyy-MM-ddTHH:mm:ss'), 'MM/dd/yyyy'));
                    self.orderDetailsOrderNumber(orderNumber);
                    self.orderDetailsDeliveryInfo(data.DeliveryInfo);
                    self.orderDetailsOrderLines(data.OrderLines);

                    affectingBusy(false);

                    self.showOrderDetailModal();
                })
                .catch(err => {
                    affectingBusy(false);
                    window.catchFetchErrors(err);
                })
        })
    }

    self.saveAsExcel = function (grid) {
        self.fetchAllTracking(grid);
    }

    self.fetchAllTracking = async function (grid) {

        if (Enumerable.From(grid.widget().columns).Where(function (x) {
            return x.field == 'Tracking'
        }).Any()) {

            grid.busy(true);
            self.exportingBusy(true);

            function updateTrackingCell(grid, i, text, increment) {
                return new Promise(function (resolve, reject) {
                    //$.ajax({
                    //    url: '/api/generalapi/GetParcelCount',
                    //    type: 'POST',
                    //    dataType: 'json',
                    //    data: {
                    //        documentNumber: text
                    //    },
                    //    success: function (data) {
                    //        grid.widget().dataSource.data()[i].Tracking = data.Results.map(function (x) { return x.TrackingURL }).join("\r\n");
                    //        //console.log('resolved promise', i);
                    //        self.exportingProgress(self.exportingProgress() + increment);
                    //        resolve();
                    //    },
                    //    error: function (xhr, s, a) {
                    //        reject();
                    //    }
                    //});

                    fetch('/api/generalapi/GetParcelCount', {
                        method: 'POST',
                        headers: window.fetchHeaders,
                        cache: 'no-cache',
                        credentials: 'same-origin',
                        body: JSON.stringify({
                            documentNumber: text
                        })
                            .then(window.handleFetchResponse)
                            .then(data => {
                                grid.widget().dataSource.data()[i].Tracking = data.Results.map(function (x) { return x.TrackingURL }).join("\r\n");
                                //console.log('resolved promise', i);
                                self.exportingProgress(self.exportingProgress() + increment);
                                resolve();
                            })
                            .catch(err => {
                                window.catchFetchErrors(err);
                                reject();
                            })
                    });
                });
            }

            var allPromises = [];

            var gridData = grid.widget().dataSource.data();

            var increment = 100 / Enumerable.From(gridData).Where(function (x) {
                return x.Tracking
            }).Count();

            for (var i = 0; i < gridData.length; i++) {
                if (gridData[i].Tracking) {
                    if (gridData[i].Tracking.indexOf('http') == -1) {
                        allPromises.push(updateTrackingCell(grid, i, gridData[i].Tracking, increment));
                    }
                }
            }

            console.log('allPromises count', increment);

            await Promise.all(allPromises).then(function () {
                self.exportingProgress(100);
                window.setTimeout(function () {
                    grid.busy(false);
                    self.exportingBusy(false);
                    self.exportingProgress(0);
                }, 500);
                grid.widget().saveAsExcel();
            });
        } else {
            grid.widget().saveAsExcel();
        }
    }

    self.exportToExcel = async function (e) {
        var sheet = e.workbook.sheets[0];
        var grid = e.sender;
        var data = grid.dataSource.data();
        var gridColumns = grid.columns;
        var columns = gridColumns.map(function (col) {
            return {
                value: col.title ? col.title : col.field,
                autoWidth: true,
                background: "#7a7a7a",
                color: "#fff"
            };
        });

        var rows = [{ cells: columns, type: "header" }];

        for (var i = 0; i < data.length; i++) {
            var rowCells = [];
            for (var j = 0; j < gridColumns.length; j++) {
                var cellValue = data[i][gridColumns[j].field];
                if (typeof cellValue === 'string') {
                    cellValue = cellValue.replace(/T00:00:00/g, '');
                    cellValue = cellValue
                }

                rowCells.push({ value: cellValue });
            }
            rows.push({ cells: rowCells, type: "data" });
        }

        sheet.rows = rows;



        // Get a reference to the Excel sheet we are working on
        var sheet = e.workbook.sheets[0];
        var hiddenColsShift = 0;
        var hasHeader = false;

        // loop through all cols to check if there is a template in columns
        for (var i = 0; i < this.columns.length; i++) {

            // if a column is hidden, it appears in the columns array but not in the Excel columns collection
            // we must shift -1 each time we find a hidden column in the columns array
            // befor to the set the hyperlink formula in the column next to the hidden one
            if (this.columns[i].hidden === true) {
                hiddenColsShift++;
            }
            else {

                if (this.columns[i].template) {
                    //if (typeof this.columns[i].template === 'function') {

                    //this.columns[i].template.indexOf('http') > -1) {

                    // if the column has a template, it can be a hyperlink
                    var template = kendo.template(this.columns[i].template);

                    // loop through all the rows in the Excel sheet
                    for (var j = 0; j < sheet.rows.length; j++) {

                        // And check that the row is not the header row
                        if (sheet.rows[j].type !== "header") {

                            // get the value of the cell
                            var value = sheet.rows[j].cells[i].value;

                            var templateResult = null;
                            // get the data of the row and execute the template
                            if (hasHeader === true) {
                                templateResult = template(e.sender.dataSource.data()[j - 1]);
                            }
                            else {
                                templateResult = template(e.sender.dataSource.data()[j]);
                            }

                            try {
                                // check if there is an url defined in the template
                                var url = $(templateResult).attr("href");

                                if (url !== null && typeof url !== 'undefined') {
                                    if (url.indexOf(String.fromCodePoint(253)) > -1) {
                                        let a = url.split(String.fromCodePoint(253));
                                        for (var ctr = 0; ctr < a.length; ctr++) {
                                            let url = a[ctr];

                                            if (ctr > 0) {
                                                sheet.rows[j].cells.push({ value: url });
                                            }

                                            // we create a formula hyperlink
                                            sheet.rows[j].cells[i + ctr].formula = '=HYPERLINK("' + url + '","' + self.resources.Track + '")';

                                            // We set the color property as blue, underlined and right aligned to make it look like a link
                                            sheet.rows[j].cells[i + ctr - hiddenColsShift].format = '[Blue]';
                                            sheet.rows[j].cells[i + ctr - hiddenColsShift].underline = true;
                                            sheet.rows[j].cells[i + ctr - hiddenColsShift].textAlign = 'left';
                                        }
                                    } else {
                                        // we create a formula hyperlink
                                        sheet.rows[j].cells[i].formula = '=HYPERLINK("' + url + '","' + self.resources.Track + '")';

                                        // We set the color property as blue, underlined and right aligned to make it look like a link
                                        sheet.rows[j].cells[i - hiddenColsShift].format = '[Blue]';
                                        sheet.rows[j].cells[i - hiddenColsShift].underline = true;
                                        sheet.rows[j].cells[i - hiddenColsShift].textAlign = 'left';
                                    }
                                }
                            }
                            catch {
                                sheet.rows[j].cells[i].value = templateResult;
                            }
                        }
                        else {
                            hasHeader = true;
                        }
                    }
                    //}
                }
            }
        }
    }

    //#endregion

    return self;
});