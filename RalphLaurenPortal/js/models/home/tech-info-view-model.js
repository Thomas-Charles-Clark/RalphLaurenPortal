const techInfoViewModel = ((resources, parent) => {
    let self = {
        parent: parent
    };

    self.data = ko.observable();

    self.fetchTechInfo = (productId) => {
        self.data(undefined);
        return fetch('/api/generalapi/FetchTechInfo', {
            method: 'POST',
            headers: window.fetchHeaders,
            body: JSON.stringify({
                productId: productId
            })
        })
        .then(window.handleFetchResponse)
        .then((data) => {
            console.trace(data);
            if (data != null && Array.isArray(data) && data.length > 0)
                self.data(data[0]);
        });
    }

    return self;
});