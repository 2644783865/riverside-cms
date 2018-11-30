using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Auth.Domain
{
    public interface IAuthenticationConfigurationService
    {
        int GetPasswordFailuresBeforeLockOut(long tenantId);
        TimeSpan GetLockOutDuration(long tenantId);
    }
}
