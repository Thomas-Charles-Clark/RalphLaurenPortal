const tradeAccountSelectorViewModel = (() => {
    let self = {};

    self.tradeAccount = ko.observable();

    self.tradeAccount.skipSubscription = true;

    self.availableTradeAccounts = ko.observableArray([]);

    self.createTradeAccount = () => tradeAccountViewModel(self);

    self.createInitialTradeAccount = (innerText) => tradeAccountViewModel(self, JSON.parse(innerText));

    self.tradeAccountLabel = ko.pureComputed(() => {
        return self.tradeAccount()?.Label;
    }, self)

    self.tradeAccountId = ko.pureComputed(() => {
        return self.tradeAccount()?.tradeAccountId();
    }, self);

    self.tradeAccountId.subscribe(nv => {
        if (self.tradeAccount.skipSubscription) {
            self.tradeAccount.skipSubscription = false;
        } else {
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
                        if (window.hasOwnProperty('pageModel')) {
                            window.pageModel.collection.valueHasMutated();
                            if (window.pageModel.hasOwnProperty('orderStatusModel')) {
                                window.pageModel.orderStatusModel.search();
                            }
                        }
                    } else {
                        showError()
                    }
                })
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

window.tradeAccountSelector = tradeAccountSelectorViewModel();
ko.applyBindings(window.tradeAccountSelector, document.querySelector('#trade-account-dropdown-wrapper'))