var page = (function () {
    return {
        initialiseUi: function (action, page, element) {
            new Vue({
                el: '#adminPage',
                data: {
                    page,
                    element,
                    form: {
                        action: action,
                        valid: true,
                        readOnly: action === 'read' || action === 'delete',
                        customErrorMessages: forms.createEmptyCustomErrorMessages()
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
                            .put(conf.elementsApiPathname() + '/' + this.$data.element.elementTypeId + '/elements/' + this.$data.element.elementId, this.$data.element, { headers: auth.getHeaders() })
                            .then(() => { window.location.href = conf.pagesPathname() + '/' + this.$data.page.pageId + '/elementtypes/' + this.$data.element.elementTypeId + '/elements/' + this.$data.element.elementId; })
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
        initialise: function (action, pageId, elementTypeId, elementId) {
            let urls = [];
            urls.push(conf.pagesApiPathname() + '/' + pageId);
            urls.push(conf.elementsApiPathname() + '/' + elementTypeId + '/elements/' + elementId);
            let axiosPromises = urls.map(url => axios.get(url, { headers: auth.getHeaders() }));
            axios.all(axiosPromises).then((responses) => { 
                let items = responses.map(r => r.data);
                this.initialiseUi(action, items[0], items[1]);
            }).catch((error) => { auth.checkAuthorised(error); });
        }
    };
})();