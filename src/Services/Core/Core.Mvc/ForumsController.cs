using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    public class ForumsController : Controller
    {
        private readonly IForumService _forumService;

        public const int DefaultPageSize = 10;

        public ForumsController(IForumService forumService)
        {
            _forumService = forumService;
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/threads")]
        [ProducesResponseType(typeof(IEnumerable<ForumThread>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListForumThreadsAsync(long tenantId, [FromQuery]long? parentPageId, [FromQuery]bool? recursive, [FromQuery]string tagIds, [FromQuery]int? pageSize)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
            IEnumerable<ForumThread> threads = await _forumService.ListLatestThreadsAsync(tenantId, parentPageId, recursive ?? false, tagIdCollection, pageSize ?? DefaultPageSize);
            return Ok(threads);
        }
    }
}
