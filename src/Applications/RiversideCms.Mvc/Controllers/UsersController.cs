using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Storage.Client;

namespace RiversideCms.Mvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        private const long TenantId = 6;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> ReadImage(long userId, UserImageType userImageType)
        {
            long tenantId = TenantId;

            BlobContent blobContent = await _userService.ReadUserImageAsync(tenantId, userId, userImageType);

            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }
    }
}
