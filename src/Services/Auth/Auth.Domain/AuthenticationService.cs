using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationProviderService _authenticationProviderService;
        private readonly IAuthenticationValidator _authenticationValidator;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IAuthenticationProviderService authenticationProviderService, IAuthenticationValidator authenticationValidator, IUserRepository userRepository)
        {
            _authenticationProviderService = authenticationProviderService;
            _authenticationValidator = authenticationValidator;
            _userRepository = userRepository;
        }

        public void Logoff()
        {
            _authenticationProviderService.Logoff();
        }

        public async Task LogonAsync(long tenantId, LogonModel model)
        {
            try
            {
                // Validate supplier logon user credentials
                await _authenticationValidator.ValidateLogonAsync(tenantId, model);

                // Get authenticated user details
                UserIdentity identity = await _userRepository.ReadUserIdentityAsync(tenantId, model.Email.Trim().ToLower());

                // Logon user using authentication provider
                AuthenticationSession session = new AuthenticationSession
                {
                    Persist = model.RememberMe,
                    Identity = identity
                };
                _authenticationProviderService.Logon(session);
            }
            catch (UserLockedOutException)
            {
                Logoff();
                throw;
            }
        }
    }
}
