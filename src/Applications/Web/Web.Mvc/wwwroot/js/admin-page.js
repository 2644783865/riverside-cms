var page = (function () {
    return {
        initialiseUi: function (action, model) {
            new Vue({
                el: '#adminPage',
                data: {
                    model,
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
                            .put(conf.baseUrl() + 'api/v1/core/pages/' + this.$data.model.pageId, this.$data.model, { headers: auth.getHeaders() })
                            .then(() => { window.location.href = conf.baseUrl() + 'admin/pages/' + this.$data.model.pageId; })
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
        initialise: function (action, pageId) {
            let axiosPromises = [];
            axiosPromises.push(axios.get(conf.baseUrl() + 'api/v1/core/pages/' + pageId, { headers: auth.getHeaders() }));
            axios.all(axiosPromises).then(axios.spread((pageResponse) => {
                let page = pageResponse !== undefined ? pageResponse.data : null;
                this.initialiseUi(action, page);
            })).catch((error) => { auth.checkAuthorised(error); });
        }
    };
})();