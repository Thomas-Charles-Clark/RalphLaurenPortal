const stockViewModel = ((resources, parent) => {

    let self = {
        parent: parent
    };

    self.totalStock = ko.observable();

    self.totalPurchaseOrderStock = ko.observable();

    self.unit = ko.observable();

    self.stockStatus = ko.observable();

    self.inStockData = ko.observable();

    self.dueInData = ko.observable();

    self.supplierData = ko.observable();

    self.supplierStockAvailabilityMessage = ko.observable();

    self.totalInStock = ko.pureComputed(function () {
        if (self.inStockData()) {
            return Number(Math.round((Enumerable.From(self.inStockData()).SelectMany(function (x) {
                return x.Pieces
            }).Select(function (x) {
                return +x.Length
            }).Sum()) * 10) / 10).toFixed(2) + self.unit()
        } else {
            return 0
        }
    }, self);

    self.totalDueIn = ko.pureComputed(function () {
        if (self.dueInData()) {
            return Number(Math.round((Enumerable.From(self.dueInData()).Select(function (x) {
                return +x.Length
            }).Sum()) * 10) / 10).toFixed(2) + self.unit()
        } else {
            return 0
        }
    }, self);

    self.totalSupplier = ko.pureComputed(function () {
        if (self.supplierData()) {
            return Number(Math.round((Enumerable.From(self.supplierData()).SelectMany(function (x) {
                return x.Pieces
            }).Select(function (x) {
                return +x.Length
            }).Sum()) * 10) / 10).toFixed(2) + self.unit()
        } else {
            return 0
        }
    }, self);

    self.supplierHasStock = ko.pureComputed(function () {
        return self.totalSupplier() != '0.00m' && self.totalSupplier() != 0
    }, self);

    self.sufficientSinglePieceAvailable = ko.pureComputed(function () {
        return Enumerable.From(self.inStockData()).SelectMany(function (x) {
            return x.Pieces
        }).Where(function (x) {
            return +x.Length <= +self.Quantity
        }).SingleOrDefault();
    }, self);

    self.fetchStockInfo = (productId) => {
        return fetch('/api/generalapi/FetchStockInfo', {
            method: 'POST',
            headers: window.fetchHeaders,
            body: JSON.stringify({
                productId: productId,
                countryId: getCountryId(),
                languageId: getLanguageId(),
                contentDate: getDate()
            })
        })
        .then(window.handleFetchResponse)
        .then((data) => {

            console.trace(data);

            if (data != null) {

                self.totalStock(data.TotalStock);

                ko.utils.arrayForEach(data.Batches, function (b) {
                    b.totalStock = Number(Math.round(Enumerable.From(b.Pieces).Select(function (p) {
                        return Math.round((Number(p.Length) * Number(p.Quantity)) * 10) / 10;
                    }).Sum() * 10) / 10).toFixed(2);
                })

                self.inStockData(data.Batches);

                self.supplierData(data.SupplierStock);

                self.dueInData(data.PurchaseOrders);

                self.totalPurchaseOrderStock(data.TotalPurchaseOrderLength);

                self.supplierStockAvailabilityMessage(data.SupplierAvailabilityMessage);

                $('#product-modal').modal('handleUpdate');
            }
        });
    }
    return self;
});