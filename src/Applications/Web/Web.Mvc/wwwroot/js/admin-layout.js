var layout = (function () {
    return {
        initialise: function () {
            new Vue({
                el: '#adminBar',
                computed: {
                    loggedIn() {
                        return auth.userSession() !== null;
                    }
                },
                methods: {
                    logout() {
                        auth.logout();
                        window.location.href = conf.loginPathname();
                    }
                },
                created() {
                    if (!this.loggedIn && window.location.pathname !== conf.loginPathname())
                        window.location.href = conf.loginPathname();
                }
            });
        }
    };
})();