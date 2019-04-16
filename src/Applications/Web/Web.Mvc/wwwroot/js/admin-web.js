var web = (function () {
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
                            .put(conf.webApiPathname(), this.$data.model, { headers: auth.getHeaders() })
                            .then(() => { window.location.href = conf.webPathname(); })
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
        initialise: function (action) {
            let axiosPromises = [];
            axiosPromises.push(axios.get(conf.webApiPathname(), { headers: auth.getHeaders() }));
            axios.all(axiosPromises).then(axios.spread((webResponse) => {
                let web = webResponse !== undefined ? webResponse.data : null;
                this.initialiseUi(action, web);
            })).catch((error) => { auth.checkAuthorised(error); });
        }
    };
})();