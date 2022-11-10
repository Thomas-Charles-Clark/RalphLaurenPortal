const pageViewModel = ((resources) => {
    let self = {
        resources: resources
    };

    self.resources.PleaseWait = self.resources.PleaseWait.replace(/\./, '');

    //#region Properties

    self.haveSearchedOnce = ko.observable(false);

    self.busy = ko.observable(false);

    self.busyMessage = ko.observable(self.resources.Searching);

    self.productTypeId = ko.observable('');

    self.collection = ko.observable();

    self.availableCollections = ko.observableArray(getCollections());

    self.searchTerm = ko.observable();

    self.searchResults = ko.observableArray([]);

    self.inspectingProduct = ko.observable();

    self.pageSize = ko.observable('50');

    //#endregion


    //#region Components

    self.orderStatusModel = orderStatusViewModel(resources, self);

    self.techInfoModel = techInfoViewModel(resources, self);

    self.stockModel = stockViewModel(resources, self);

    self.salesHistoryModel = salesHistoryViewModel(resources, self);

    //#endregion


    //#region Subscriptions

    self.productTypeId.subscribe(nv => {
        self.collection(undefined);
        fetch('/api/generalapi/FetchCollections', {
            method: 'POST',
            headers: window.fetchHeaders,
            body: JSON.stringify({
                productTypeId: nv
            })
        }).then(window.handleFetchResponse)
            .then(data => {
                self.availableCollections(data.Collections);
                self.pageSize.valueHasMutated();
            })
            .catch(window.catchFetchErrors);
    });

    //#endregion


    //#region Computeds

    self.searchResultCountDisplay = ko.pureComputed(() => {
        let count = self.searchResults().length;
        if (count == 0 || count > 1) {
            return `${count} ${self.resources.SearchResults}`;
        } else {
            return `${count} ${self.resources.SearchResult}`;
        }
    }, self);

    self.anySearchResults = ko.pureComputed(() => self.searchResults().length > 0, self);

    self.collectionName = ko.pureComputed(() => {
        return self.collection()?.Name;
    }, self);

    self.search = ko.computed(() => {
        self.busy(true);
        fetch('/api/generalapi/ProductSearch', {
            method: 'POST',
            headers: window.fetchHeaders,
            body: JSON.stringify({
                searchTerm: self.searchTerm.peek(),
                productTypeId: self.productTypeId.peek(),
                collectionId: self.collection()?.CollectionID,
                pageSize: self.pageSize()
            })
        })
            .then(window.handleFetchResponse)
            .then((data) => {
                self.haveSearchedOnce(true);
                self.busy(false);
                self.searchResults(data);
                setTimeout(() => {
                    document.querySelectorAll('.card-body .btn').forEach(function (element) {
                        new bootstrap.Ripple(element)
                    });
                });
            })
            .catch(window.catchFetchErrors)
    }, self).extend({
        rateLimit: {
            timeout: 100,
            method: 'notifyWhenChangesStop'
        }
    });

    //#endregion


    //#region Methods

    self.clearSearchTerm = () => {
        self.searchTerm(undefined);
        self.productTypeId('');
        self.collection(undefined);
    }

    self.s2ab = (s) => {
        var buf = new ArrayBuffer(s.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i != s.length; ++i)
            view[i] = s.charCodeAt(i) & 0xFF;
        return buf;
    }

    self.fetchProductFeed = () => {
        self.busy(true);
        self.busyMessage(self.resources.PleaseWait);
        fetch('/api/generalapi/FetchProductDownload', {
            method: 'POST',
            headers: {
                'Accept': 'text/plain',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                searchTerm: self.searchTerm(),
                productTypeId: self.productTypeId(),
                collectionId: self.collection()?.CollectionID
            })
        })
            .then(window.handleFetchResponse)
            .then(data => {

                let bin = atob(data.fileContents);
                let ab = self.s2ab(bin); // from example above
                let blob = new Blob([ab], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;' });

                var url = window.URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.href = url;
                a.download = `RalphLauren-Products.xlsx`;
                document.body.appendChild(a);
                a.click();
                a.remove();
                self.busy(false);
                self.busyMessage(self.resources.Searching);
            });
    }

    self.showProductModal = (product, tab) => {
        self.busy(true);

        self.inspectingProduct(product);

        self.stockModel.unit(product.stockMessage.unit);
        self.stockModel.stockStatus(product.statusMessage);

        var triggerElement = document.querySelector(`#nav-product-content button[data-bs-target="${tab}"]`);
        bootstrap.Tab.getOrCreateInstance(triggerElement).show();

        $('#product-modal').modal('show');

        Promise.allSettled([
            self.techInfoModel.fetchTechInfo(product.productId),
            self.stockModel.fetchStockInfo(product.productId)
        ]).then(() => {
            self.busy(false);
        });
    }

    //#endregion

    return self;
});

getResource([
    'Last12Months',
    'Last6Months',
    'ThisMonth',
    'Custom',
    'ShipmentDate',
    'InvoiceDate',
    'OrderDate',
    'StatusDate',
    'DespatchDate',
    'ExpiryDate',
    'DueDate',
    'FromMustBeDate',
    'PleaseProvideFromDate',
    'ToMustBeDate',
    'FromMustPreceedTo',
    'OrderNo.',
    'YourOrderReference',
    'Value',
    'Status',
    'Total',
    'Outstanding',
    'Released',
    'NoRecordsToDisplay',
    'Quantity',
    'Product',
    'Batch',
    'Description',
    'YourLineReference',
    'Tracking',
    'LineNo.',
    'BatchNo.',
    'Despatch',
    'Track',
    'kendoPagerDisplay',
    'kendoPagerEmpty',
    'kendoPagerPage',
    'kendoPagerOf',
    'kendoPagerItemsPerPage',
    'kendoPagerFirstPage',
    'kendoPagerLastPage',
    'kendoPagerNextPage',
    'kendoPagerPreviousPage',
    'kendoPagerRefresh',
    'kendoPagerMorePages',
    'StockistClickHere',
    'Searching',
    'PleaseWait',
    'SearchResult',
    'SearchResults',
    'AllCollections',
    'MoodboardProductType',
    'ppCollection',
    'Stock',
    'SoldQuantity',
    'ReservedQuantity',
    'WIPQuantity',
    'TotalQuantity'
]).then((arrayResult) => {

    let resources = {};

    Object.keys(arrayResult).forEach((r) => {
        resources[arrayResult[r].Name.replace(/\.$/, '')] = arrayResult[r].Value;
    })

    window.pageModel = pageViewModel(resources);
    window.pageModel.orderStatusModel.search();
    window.pageModel.orderStatusModel.ready(true);
    ko.applyBindings(window.pageModel, document.querySelector('#ko-wrapper'));

    var triggerTabList = [].slice.call(document.querySelectorAll('#order-status-tabs button'))
    triggerTabList.forEach(function (triggerEl) {
        var tabTrigger = bootstrap.Tab.getOrCreateInstance(triggerEl)

        var targetPane = document.querySelector(triggerEl.dataset.bsTarget);

        triggerEl.addEventListener('click', function (event) {
            event.preventDefault()

            let orderStatus = targetPane.dataset.refresh;
            window.pageModel.orderStatusModel.searchTerm(undefined);
            window.pageModel.orderStatusModel.viewingStatus(orderStatus);
            window.pageModel.orderStatusModel.refresh(orderStatus, true)

            tabTrigger.show()
        })
    })
});