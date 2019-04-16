var auth = (function () {
    return {
        login(userSession) {
            localStorage.setItem('userSession', JSON.stringify(userSession));
        },
        logout() {
            localStorage.setItem('userSession', null);
        },
        userSession() {
            let userSessionString = localStorage.getItem('userSession');
            if (userSessionString === null || userSessionString === undefined)
                return null;
            let userSession = JSON.parse(userSessionString);
            if (userSession === null || userSession === undefined)
                return null;
            return userSession;
        },
        getHeaders() {
            let userSession = this.userSession();
            if (userSession === null)
                return { 'Content-Type': 'application/json' };
            else
                return { 'Authorization': 'Bearer ' + userSession.security.token, 'Content-Type': 'application/json' };
        },
        checkAuthorised(error) {
            if (error.response.status === 401) {
                window.location.href = conf.loginPathname();
            }
        }
    };
})();