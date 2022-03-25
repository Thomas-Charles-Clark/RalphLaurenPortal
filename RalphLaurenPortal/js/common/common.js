const pageViewModel = (() => {
    let self = {};

    self.search = () => {
        fetch('/api/generalapi/ProductSearch', {
            method: 'POST',
            headers: window.fetchHeaders,
            body: JSON.stringify({
                searchTerm: 'Brera',
                productTypeId: null
            })
        })
        .then(window.handleFetchResponse)
        .then((data) => {
            console.trace(data);
        })
        .catch(window.catchFetchErrors)
    }

    return self;
});

var pageModel = pageViewModel();

ko.applyBindings(pageModel, document.querySelector('#ko-wrapper'));