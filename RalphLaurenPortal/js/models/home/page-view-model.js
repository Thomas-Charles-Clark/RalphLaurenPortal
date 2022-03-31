const pageViewModel = ((resources) => {
    let self = {};

    self.busy = ko.observable(false);

    self.productTypeId = ko.observable();

    self.searchTerm = ko.observable();

    self.searchResults = ko.observableArray([]);

    self.orderStatusModel = orderStatusViewModel(resources, self);

    self.inspectingProduct = ko.observable();

    self.techInfoModel = techInfoViewModel(resources, self);

    self.stockModel = stockViewModel(resources, self);

    self.clearSearchTerm = () => {
        self.searchTerm(undefined);
        self.search();
    }

    self.productTypeId.subscribe((newValue) => {
        if (self.searchTerm()) {
            self.search();
        }
    })

    self.search = () => {
        self.busy(true);
        fetch('/api/generalapi/ProductSearch', {
            method: 'POST',
            headers: window.fetchHeaders,
            body: JSON.stringify({
                searchTerm: self.searchTerm(),
                productTypeId: self.productTypeId()
            })
        })
        .then(window.handleFetchResponse)
        .then((data) => {
            console.trace(data);
            self.searchResults(data);
            setTimeout(() => {
                self.busy(false);
                document.querySelectorAll('.card-body .btn').forEach(function (element) {
                    new bootstrap.Ripple(element)
                });
            });
        })
        .catch(window.catchFetchErrors)
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

    self.search();

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
    'YourReference',
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
    'kendoPagerMorePages'
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
            window.pageModel.orderStatusModel.refresh(orderStatus, false)

            tabTrigger.show()
        })
    })
});