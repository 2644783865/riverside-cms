using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    public class TenantsController : Controller
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Tenant), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadWebAsync(long tenantId)
        {
            Tenant tenant = await _tenantService.ReadTenantAsync(tenantId);
            if (tenant == null)
                return NotFound();
            return Ok(tenant);
        }
    }
}
