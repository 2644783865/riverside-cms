var page = (function () {
    return {
        initialise: function () {
            new Vue({
                el: '#rcms-form',
                data: {
                    model: {
                        email: 'example@example.com',
                        password: 'blahblah'
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
                        console.log(error.response.data.errors);
                        if (error.response && error.response.data && error.response.data.errors)
                            this.$data.form.customErrorMessages = forms.createCustomErrorMessages(error.response.data.errors);
                        else
                            this.$data.form.customErrorMessages = forms.createCustomErrorMessages([ { key: '', message: 'An unexpected error occurred' }]);
                    },
                    submitClicked() {
                        axios
                            .post('/api/v1/authentication/authenticate', this.$data.model)
                            .then((response) => { localStorage.setItem('userSession', JSON.stringify(response.data)); })
                            .catch((error) => { this.processErrors(error); });
                    },
                    testClicked() {
                        let token = JSON.parse(localStorage.getItem('userSession')).security.token;
                        const authHeader = token ? { 'Authorization': 'Bearer ' + token } : {};
                        let headers = {
                            ...authHeader,
                            'Content-Type': 'application/json'
                        };
                        axios
                            .get('/api/v1/authentication/test', { headers: headers })
                            .then((response) => { console.log(response.data); })
                            .catch((error) => { });
                    }
                }
            });
        }
    };
})();