using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Auth.Domain
{
    public interface IAuthenticationRepository
    {
        Task<AuthenticationState> ReadAuthenticationStateAsync(long tenantId, string email);
        Task UpdateAuthenticationStateAsync(long tenantId, string email, AuthenticationState state);
    }
}
