var account = (function () {
    return {
        initialiseLogin: function () {
            new Vue({
                el: '#adminPage',
                data: {
                    model: {
                        email: '',
                        password: ''
                    },
                    form: {
                        valid: true,
                        customErrorMessages: forms.createEmptyCustomErrorMessages()
                    }
                },
                methods: {
                    getCustomErrorMessages(key) {
                        return forms.getFilteredCustomErrorMessages(this.$data.form.customErrorMessages, key);
                    },
                    validateForm: function (validationData) {
                        this.$data.form.valid = forms.validateForm(validationData);
                    },
                    processErrors(error) {
                        if (error.response && error.response.data && error.response.data.errors)
                            this.$data.form.customErrorMessages = forms.createCustomErrorMessages(error.response.data.errors);
                        else
                            this.$data.form.customErrorMessages = forms.createCustomErrorMessages([ { key: '', message: 'An unexpected error occurred' }]);
                    },
                    submitClicked() {
                        axios
                            .post(conf.accountApiPathname() + '/authenticate', this.$data.model)
                            .then((response) => { auth.login(response.data); window.location.href = conf.homePathname(); })
                            .catch((error) => { this.processErrors(error); });
                    }
                }
            });
        }
    };
})();