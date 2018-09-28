using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    public class WebsController : Controller
    {
        private readonly IWebService _webService;

        public WebsController(IWebService webService)
        {
            _webService = webService;
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Web), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadWebAsync(long tenantId)
        {
            Web web = await _webService.ReadWebAsync(tenantId);
            if (web == null)
                return NotFound();
            return Ok(web);
        }
    }
}
