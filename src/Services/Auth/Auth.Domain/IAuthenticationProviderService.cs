using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Auth.Domain
{
    /// <summary>
    /// Authentication providers should implement this provider service.
    /// </summary>
    public interface IAuthenticationProviderService
    {
        /// <summary>
        /// Log user on using underlying authentication provider.
        /// </summary>
        /// <param name="session">Contains information about session and user that is being logged on.</param>
        void Logon(AuthenticationSession session);

        /// <summary>
        /// Gets authentication session for logged on user or null if no user logged on.
        /// </summary>
        /// <returns>Authenticated session and user details.</returns>
        AuthenticationSession GetLoggedOnSession();

        /// <summary>
        /// Logs off active authentication session.
        /// </summary>
        void Logoff();
    }
}
