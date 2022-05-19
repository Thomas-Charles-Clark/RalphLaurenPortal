const salesHistoryViewModel = ((resources, parent) => {
    let self = {
        parent: parent,
        resources: resources
    };

    //#region Properties

    self.busy = ko.observable(false);

    self.busyMessage = ko.observable(resources.Searching);

    self.searchResults = ko.observableArray([]);

    self.haveSearchedOnce = ko.observable(false);

    self.searchTerm = ko.observable();

    self.dateRange = ko.observable(6);

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
                return self.dateRange() == -1 && self.customToDate() != null
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

    self.schema = {
        model: {
            id: 'ProductID',
            fields: {
                ProductID: {},
                productId: {},
                ImageID: {},
                imageId: {},
                Code: {},
                code: {},
                ProductTypeName: {},
                CollectionName: {},
                Name: {},
                name: {},
                Status: {},
                statusMessage: {},
                StockMessage: {},
                stockMessage: {},
                SoldQuantity: {},
                ReservedQuantity: {},
                WIPQuantity: {},
                TotalQuantity: {}
            }
        }
    };

    self.grid = {
        busy: self.busy,
        widget: ko.observable(),
        data: self.searchResults,
        dataSource: new kendo.data.DataSource({
            schema: self.schema,
            serverFiltering: false,
            serverSorting: false,
            serverGrouping: false,
            serverPaging: false,
            pageSize: 20,
            sort: [{
                field: 'CollectionName',
                dir: 'desc'
            }, {
                field: 'Code',
                dir: 'desc'
            }],
        }),
        pageable: {
            pageSize: 20,
            pageSizes: [20, 50, 100, 500],
            messages: kendoGridPagerMessages
        },
        columns: [{
            field: 'ImageID',
            title: ' ',
            width: 100,
            template: '<img width="156" height="156" src="/image/239/#: ImageID #" />',
            filterable: false
        }, {
            field: 'ProductTypeName',
            title: self.resources.MoodboardProductType,
            width: 140,
            filterable: true
        }, {
            field: 'CollectionName',
            title: self.resources.ppCollection,
            width: 140,
            filterable: true
        }, {
            field: 'Code',
            title: self.resources.Code,
            width: 120,
            filterableable: true
        }, {
            field: 'Name',
            title: self.resources.Name,
            width: 120,
            filterableable: true
        }, {
            field: 'Status',
            title: self.resources.Status,
            width: 140,
            filterable: false
        }, {
            field: 'StockMessage',
            title: self.resources.Stock,
            width: 120,
            filterable: false
        }, {
            field: 'SoldQuantity',
            title: self.resources.SoldQuantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            },
            filterable: false
        }, {
            field: 'ReservedQuantity',
            title: self.resources.ReservedQuantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            },
            filterable: false
        }, {
            field: 'WIPQuantity',
            title: self.resources.WIPQuantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            },
            filterable: false
        }, {
            field: 'TotalQuantity',
            title: self.resources.TotalQuantity,
            width: 100,
            attributes: {
                'class': 'text-right'
            },
            headerAttributes: {
                'class': 'text-right'
            },
            filterable: false
        }],
        sortable: true,
        filterable: {
            extra: false,
            model: 'contains',
            operators: {
                string: {
                    contains: 'Contains',
                    startsWith: 'Starts with',
                    endsWith: 'Ends with',
                    eq: 'Equals',
                    neq: 'Does not equal'
                }
            }
        },
        noRecords: {
            template: self.resources.NoRecordsToDisplay
        },
        selectable: 'row',
        change: function (e) {
            var dataItem = this.dataItem(this.select());
            self.parent.showProductModal(dataItem, '#nav-tech')
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

    self.searchResults.subscribe(nvs => {
        self.haveSearchedOnce(true);
    });

    self.dateRange.subscribe(nv => {
        if (nv === -1 && self.customFromDate() == null) {
            var from = new Date();
            from.setMonth(from.getMonth() - 1);
            self.customFromDate(from)
        }
    });

    //#endregion


    //#region Computeds

    self.resultCountDisplay = ko.pureComputed(() => {
        let count = self.searchResults().length;
        if (count == 0 || count > 1) {
            return `${count} ${self.resources.SearchResults}`;
        } else {
            return `${count} ${self.resources.SearchResult}`;
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

    self.search = ko.computed(() => {
        self.busy(true);
        fetch('/api/generalapi/SalesHistorySearch', {
            method: 'POST',
            headers: window.fetchHeaders,
            body: JSON.stringify({
                dateRange: self.dateRange(),
                fromDate: self.customFromDate(),
                toDate: self.customToDate()
            })
        })
        .then(window.handleFetchResponse)
        .then(data => {
            self.searchResults(data);
            setTimeout(() => {
                self.busy(false);
            }, 0);
        })
        .catch(err => {
            self.busy(false);
            window.catchFetchErrors(err);
        })
    }, self);

    //#endregion


    //#region Methods

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
                if (typeof cellValue === 'string')
                    cellValue = cellValue.replace(/T00:00:00/g, '');

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
})