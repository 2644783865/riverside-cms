using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Auth.Domain;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Auth.Mvc
{
    [Authorize]
    [MultiTenant()]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpPost]
        [AllowAnonymous]
        [Route("api/v1/account/authenticate")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AuthenticateAsync([FromBody]LogonModel model)
        {
            try
            {
                return Ok(await _authenticationService.AuthenticateAsync(TenantId, model));
            }
            catch (ValidationErrorException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpGet]
        [Authorize(Policy = "UpdatePageElements")]
        [Route("api/v1/account/test")]
        public IActionResult Test()
        {
            return Ok("Test result returned");
        }
    }
}
