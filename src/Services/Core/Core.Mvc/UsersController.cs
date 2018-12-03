using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    [MultiTenant()]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public const int DefaultPageSize = 10;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [Route("api/v1/core/users/{userId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadUserAsync(long userId)
        {
            User user = await _userService.ReadUserAsync(TenantId, userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet]
        [Route("api/v1/core/users/{userId:int}/images/{userImageType}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadUserImageAsync(long userId, UserImageType userImageType)
        {
            BlobContent blobContent = await _userService.ReadUserImageAsync(TenantId, userId, userImageType);
            if (blobContent == null)
                return NotFound();
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpGet]
        [Route("api/v1/core/users")]
        [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListUsersAsync([FromQuery]string userIds)
        {
            IEnumerable<long> userIdCollection = !string.IsNullOrWhiteSpace(userIds) ? userIds.Split(',').Select(long.Parse) : null;
            IEnumerable<User> users = await _userService.ListUsersAsync(TenantId, userIdCollection);
            return Ok(users);
        }
    }
}
