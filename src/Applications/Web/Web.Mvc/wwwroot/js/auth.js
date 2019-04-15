var auth = (function () {
    return {
        getHeaders() {
            let userSession = JSON.parse(localStorage.getItem('userSession'));
            if (userSession === null)
                return { 'Content-Type': 'application/json' };
            let token = userSession.security.token;
            const authHeader = token ? { 'Authorization': 'Bearer ' + token } : {};
            let headers = {
                ...authHeader,
                'Content-Type': 'application/json'
            };
            return headers;
        },
        checkAuthorised(error) {
            console.log(error);
            if (error.response.status === 401) {
                window.location.href = conf.baseUrl() + 'account/login';
            }
        }
    };
})();