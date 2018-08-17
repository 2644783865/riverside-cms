using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;

namespace Core.API.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public const int DefaultPageSize = 10;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/users")]
        [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListForumThreads(long tenantId, [FromQuery]string userIds)
        {
            IEnumerable<long> userIdCollection = !string.IsNullOrWhiteSpace(userIds) ? userIds.Split(",").Select(long.Parse) : null;
            IEnumerable<User> users = await _userService.ListUsersAsync(tenantId, userIdCollection);
            return Ok(users);
        }
    }
}
