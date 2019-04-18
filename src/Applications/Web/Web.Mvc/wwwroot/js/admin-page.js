var page = (function () {
    return {
        initialiseUi: function (action, masterPage, page, elementTypes, elementDefinitionsByTypeId) {
            new Vue({
                el: '#adminPage',
                data: {
                    page,
                    masterPage,
                    elementTypes,
                    elementDefinitionsByTypeId,
                    form: {
                        action: action,
                        valid: true,
                        readOnly: action === 'read' || action === 'delete',
                        customErrorMessages: forms.createEmptyCustomErrorMessages()
                    }
                },
                computed: {
                    zones() {
                        return page.pageZones.map(pz => {
                            return {
                                name: masterPage.masterPageZones.filter(mpz => mpz.masterPageZoneId === pz.masterPageZoneId)[0].name,
                                elements: pz.pageZoneElements.map(pze => { return { elementTypeId: pze.elementTypeId, elementTypeName: elementTypes.filter(et => et.elementTypeId === pze.elementTypeId)[0].name, elementId: pze.elementId, elementName: elementDefinitionsByTypeId[pze.elementTypeId].filter(ed => ed.elementId === pze.elementId)[0].name }; })
                            };
                        });
                    }
                },
                methods: {
                    validateForm: function (validationData) {
                        this.$data.form.valid = forms.validateForm(validationData);
                    },
                    getCustomErrorMessages(key) {
                        return forms.getFilteredCustomErrorMessages(this.$data.form.customErrorMessages, key);
                    },
                    processErrors(error) {
                        if (error.response && error.response.data && error.response.data.errors)
                            this.$data.form.customErrorMessages = forms.createCustomErrorMessages(error.response.data.errors);
                        else
                            this.$data.form.customErrorMessages = forms.createCustomErrorMessages([ { key: '', message: 'An unexpected error occurred' }]);
                    },
                    update() {
                        axios
                            .put(conf.pagesApiPathname() + '/' + this.$data.page.pageId, this.$data.page, { headers: auth.getHeaders() })
                            .then(() => { window.location.href = conf.pagesPathname() + '/' + this.$data.page.pageId; })
                            .catch((error) => { auth.checkAuthorised(error); this.processErrors(error); });
                    },
                    submitClicked() {
                        switch (this.$data.form.action)
                        {
                            case 'update':
                                this.update();  
                                break;
                        }
                    }
                }
            });
        },
        getElementsByTypeId: function (page) {
            let elementsByTypeId = {};
            page.pageZones.forEach(function (pageZone) {
                pageZone.pageZoneElements.forEach(function (pageZoneElement) {
                    if (elementsByTypeId[pageZoneElement.elementTypeId] === undefined)
                        elementsByTypeId[pageZoneElement.elementTypeId] = [];
                    elementsByTypeId[pageZoneElement.elementTypeId].push(pageZoneElement.elementId);
                });
            });
            Object.keys(elementsByTypeId).forEach(function (elementTypeId) {
                elementsByTypeId[elementTypeId] = Array.from(new Set(elementsByTypeId[elementTypeId]));
            });
            return elementsByTypeId;
        },
        initialiseWithPage: function (action, page) {
            let urls = [];
            let elementsByTypeId = this.getElementsByTypeId(page);
            urls.push(conf.masterPagesApiPathname() + '/' + page.masterPageId);
            if (Object.keys(elementsByTypeId).length > 0) {
                urls.push(conf.coreElementsApiPathname() + '?elementtypeids=' + Object.keys(elementsByTypeId).join(','));
                Object.keys(elementsByTypeId).forEach(function(elementTypeId) {
                    urls.push(conf.coreElementsApiPathname() + '/' + elementTypeId + '/elements?elementids=' + elementsByTypeId[elementTypeId].join(','));
                });
            }
            let axiosPromises = urls.map(url => axios.get(url, { headers: auth.getHeaders() }));
            axios.all(axiosPromises).then((responses) => { 
                let items = responses.map(r => r.data);
                let masterPage = items[0];
                let elementTypes = items.length > 1 ? items[1] : [];
                let elementDefinitionsByTypeId = {};
                Object.keys(elementsByTypeId).forEach(function(elementTypeId, index) {
                    elementDefinitionsByTypeId[elementTypeId] = items[index + 2];
                });
                this.initialiseUi(action, masterPage, page, elementTypes, elementDefinitionsByTypeId);
            });
        },
        initialise: function (action, pageId) {
            axios
                .get(conf.pagesApiPathname() + '/' + pageId, { headers: auth.getHeaders() })
                .then((pageResponse) => { this.initialiseWithPage(action, pageResponse.data); })
                .catch((error) => { auth.checkAuthorised(error); });
        }
    };
})();