using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    public class DomainsController : Controller
    {
        private readonly IDomainService _domainService;

        public DomainsController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet]
        [Route("api/v1/core/domains")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(WebDomain), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadDomainByUrlAsync([FromQuery]string url)
        {
            WebDomain domain = await _domainService.ReadDomainByUrlAsync(url);
            if (domain == null)
                return NotFound();
            return Ok(domain);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/domains")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(WebDomain), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadDomainByRedirectUrlAsync(long tenantId, [FromQuery]string redirectUrl)
        {
            WebDomain domain = await _domainService.ReadDomainByRedirectUrlAsync(tenantId, redirectUrl);
            if (domain == null)
                return NotFound();
            return Ok(domain);
        }
    }
}
