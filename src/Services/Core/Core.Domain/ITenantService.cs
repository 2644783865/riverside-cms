using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface ITenantService
    {
        Task<Tenant> ReadTenantAsync(long tenantId);
    }
}
