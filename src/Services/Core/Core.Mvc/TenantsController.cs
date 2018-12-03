using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    [MultiTenant()]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Tenant), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadWebAsync(long tenantId)
        {
            Tenant tenant = null;
            if (TenantId == tenantId)
                tenant = await _tenantService.ReadTenantAsync(TenantId);
            if (tenant == null)
                return NotFound();
            return Ok(tenant);
        }
    }
}
