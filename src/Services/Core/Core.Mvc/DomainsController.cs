using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    [MultiTenant()]
    public class DomainsController : ControllerBase
    {
        private readonly IDomainService _domainService;

        public DomainsController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [Route("api/v1/core/domains")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(WebDomain), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadDomainByUrlAsync([FromQuery]string url, [FromQuery]string redirectUrl)
        {
            WebDomain domain = null;
            if (url != null)
                domain = await _domainService.ReadDomainByUrlAsync(url);
            else if (redirectUrl != null)
                domain = await _domainService.ReadDomainByRedirectUrlAsync(TenantId, redirectUrl);
            if (domain == null)
                return NotFound();
            return Ok(domain);
        }
    }
}
