using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    [MultiTenant()]
    public class ForumsController : ControllerBase
    {
        private readonly IForumService _forumService;

        public const int DefaultPageSize = 10;

        public ForumsController(IForumService forumService)
        {
            _forumService = forumService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [Route("api/v1/core/threads")]
        [ProducesResponseType(typeof(IEnumerable<ForumThread>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListForumThreadsAsync([FromQuery]long? parentPageId, [FromQuery]bool? recursive, [FromQuery]string tagIds, [FromQuery]int? pageSize)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
            IEnumerable<ForumThread> threads = await _forumService.ListLatestThreadsAsync(TenantId, parentPageId, recursive ?? false, tagIdCollection, pageSize ?? DefaultPageSize);
            return Ok(threads);
        }
    }
}
