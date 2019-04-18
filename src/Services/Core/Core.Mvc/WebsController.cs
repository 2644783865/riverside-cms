using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Core.Mvc
{
    [Authorize]
    [MultiTenant()]
    public class WebsController : ControllerBase
    {
        private readonly IWebService _webService;

        public WebsController(IWebService webService)
        {
            _webService = webService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/web")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Web), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadWebAsync()
        {
            Web web = await _webService.ReadWebAsync(TenantId);
            if (web == null)
                return NotFound();
            return Ok(web);
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Policy = "UpdateWeb")]
        [Route("api/v1/core/web")]
        public async Task<IActionResult> UpdateWebAsync([FromBody]Web web)
        {
            try
            {
                await _webService.UpdateWebAsync(TenantId, web);
                return Ok();
            }
            catch (ValidationErrorException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }
    }
}
