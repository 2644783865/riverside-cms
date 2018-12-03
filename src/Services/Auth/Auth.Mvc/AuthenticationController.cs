using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Auth.Domain;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Auth.Mvc
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("api/v1/authentication/logon")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult PostAsync([FromBody] LogonModel model)
        {
            try
            {
                return Ok();
            }
            catch (ValidationErrorException ex)
            {
                return BadRequest(ex.Errors);
            }
        }
    }
}
