const tradeAccountSelectorViewModel = ((resources) => {
    let self = {
        resources: resources
    };

    self.tradeAccount = ko.observable();

    self.tradeAccount.skipSubscription = true;

    self.availableTradeAccounts = ko.observableArray([]);

    self.createTradeAccount = () => tradeAccountViewModel(self);

    self.createInitialTradeAccount = (innerText) => tradeAccountViewModel(self, JSON.parse(innerText));

    self.tradeAccountLabel = ko.pureComputed(() => {
        return self.tradeAccount()?.Label ?? self.resources.All;
    }, self)

    self.tradeAccountId = ko.pureComputed(() => {
        return self.tradeAccount()?.tradeAccountId();
    }, self);

    self.tradeAccountId.subscribe(nv => {
        if (self.tradeAccount.skipSubscription) {
            self.tradeAccount.skipSubscription = false;
        } else {
            window.pageModel.busy(true);

            var affectingBusy = null;

            switch (window.pageModel.orderStatusModel.viewingStatus()) {
                case 'outstanding':
                    affectingBusy = window.pageModel.orderStatusModel.outstandingOrdersBusy;
                    break;
                case 'backOrders':
                    affectingBusy = window.pageModel.orderStatusModel.backOrdersBusy;
                    break;
                case 'reserveOrders':
                    affectingBusy = window.pageModel.orderStatusModel.reserveOrdersBusy;
                    break;
                case 'despatchedOrders':
                    affectingBusy = window.pageModel.orderStatusModel.despatchedOrdersBusy;
                    break;
                case 'ordersInProgress':
                    affectingBusy = window.pageModel.orderStatusModel.ordersInProgressBusy;
                    break;
                case 'held':
                    affectingBusy = window.pageModel.orderStatusModel.heldOrdersBusy;
                    break;
            }

            affectingBusy(true);

            window.pageModel.salesHistoryModel.busy(true);

            fetch('/api/generalapi/UpdateTradeAccountID', {
                method: 'POST',
                headers: window.fetchHeaders,
                body: JSON.stringify({
                    tradeAccountId: nv
                })
            })
                .then(window.handleFetchResponse)
                .then(data => {
                    if (data.success) {
                        window.pageModel.collection.valueHasMutated();
                        window.pageModel.orderStatusModel.search();
                        window.pageModel.salesHistoryModel.dateRange.valueHasMutated();
                    } else {
                        showError()
                    }
                })
                .catch(err => {
                    affectingBusy(false);
                    window.catchFetchErrors(err);
                });
        }
    })

    return self;
});

const tradeAccountViewModel = ((parent, data) => {
    let self = {
        parent: parent
    }

    self.tradeAccountId = ko.observable();

    self.label = ko.observable();

    self.data = ko.observable();

    self.dataSub = self.data.subscribe(nv => {
        Object.assign(self, nv);
        self.dataSub.dispose();
        self.tradeAccountId(self.TradeAccountID);
        self.label(self.Label);
    });

    if (data) {
        self.data(data);
    }

    return self;
});

getResource(['All'])
.then((arrayResult) => {

    let resources = {};

    Object.keys(arrayResult).forEach((r) => {
        resources[arrayResult[r].Name.replace(/\.$/, '')] = arrayResult[r].Value;
    })

    window.tradeAccountSelector = tradeAccountSelectorViewModel(resources);
    ko.applyBindings(window.tradeAccountSelector, document.querySelector('#trade-account-dropdown-wrapper'))
});