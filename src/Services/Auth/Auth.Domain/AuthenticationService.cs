using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationValidator _authenticationValidator;
        private readonly ISecurityTokenService _securityTokenService;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IAuthenticationValidator authenticationValidator, ISecurityTokenService securityTokenService, IUserRepository userRepository)
        {
            _authenticationValidator = authenticationValidator;
            _securityTokenService = securityTokenService;
            _userRepository = userRepository;
        }

        public async Task<IUserSession> AuthenticateAsync(long tenantId, LogonModel model)
        {
            try
            {
                // Validate supplier logon user credentials
                await _authenticationValidator.ValidateLogonAsync(tenantId, model);

                // Get user session, comprising identity and security token
                IUserSession session = new UserSession();
                session.Identity = await _userRepository.ReadUserIdentityAsync(tenantId, model.Email.Trim().ToLower());
                session.Security = new UserSecurity
                {
                    Token = _securityTokenService.GenerateToken(session.Identity)
                };

                // Return user session
                return session;
            }
            catch (UserLockedOutException)
            {
                throw;
            }
        }
    }
}
