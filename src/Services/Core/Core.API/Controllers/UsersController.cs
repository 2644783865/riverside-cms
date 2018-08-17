using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Storage.Client;

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
        [Route("api/v1/core/tenants/{tenantId:int}/users/{userId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadUser(long tenantId, long userId)
        {
            User user = await _userService.ReadUserAsync(tenantId, userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/users/{userId:int}/images/{userImageType}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadUserImage(long tenantId, long userId, UserImageType userImageType)
        {
            BlobContent blobContent = await _userService.ReadUserImageAsync(tenantId, userId, userImageType);
            if (blobContent == null)
                return NotFound();
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/users")]
        [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListUsers(long tenantId, [FromQuery]string userIds)
        {
            IEnumerable<long> userIdCollection = !string.IsNullOrWhiteSpace(userIds) ? userIds.Split(",").Select(long.Parse) : null;
            IEnumerable<User> users = await _userService.ListUsersAsync(tenantId, userIdCollection);
            return Ok(users);
        }
    }
}
