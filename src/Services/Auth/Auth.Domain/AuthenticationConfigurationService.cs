using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class AuthenticationConfigurationService : IAuthenticationConfigurationService
    {
        public int GetPasswordFailuresBeforeLockOut(long tenantId)
        {
            return 3; // 3 for all tenants
        }

        public TimeSpan GetLockOutDuration(long tenantId)
        {
            return new TimeSpan(0, 10, 0); // 10 minutes for all tenants
        }
    }
}
