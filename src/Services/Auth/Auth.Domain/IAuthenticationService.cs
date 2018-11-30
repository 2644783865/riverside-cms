using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Auth.Domain
{
    public interface IAuthenticationService
    {
        void Logoff();
        Task LogonAsync(long tenantId, LogonModel logonModel);
    }
}
