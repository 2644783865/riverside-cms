using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/tags/{tagId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Tag), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTagAsync(long tenantId, long tagId)
        {
            Tag tag = await _tagService.ReadTagAsync(tenantId, tagId);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/tags")]
        [ProducesResponseType(typeof(IEnumerable<Tag>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListTagsAsync(long tenantId, [FromQuery]string tagIds, [FromQuery]string tagNames)
        {
            IEnumerable<Tag> tags = null;
            if (tagIds != null)
            {
                IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
                tags = await _tagService.ListTagsAsync(tenantId, tagIdCollection);
            }
            else if (tagNames != null)
            {
                IEnumerable<string> tagNameCollection = tagNames.Split(',').Select(t => t.Trim()).Distinct().Where(t => t != string.Empty);
                tags = await _tagService.ListTagsAsync(tenantId, tagNameCollection);
            }
            else
            {

            }
            return Ok(tags);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/tags/counts")]
        [ProducesResponseType(typeof(IEnumerable<TagCount>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListTagCountsAsync(long tenantId, [FromQuery]long? parentPageId, [FromQuery]bool? recursive)
        {
            IEnumerable<TagCount> tags = await _tagService.ListTagCountsAsync(tenantId, parentPageId, recursive ?? false);
            return Ok(tags);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/tags/related/counts")]
        [ProducesResponseType(typeof(IEnumerable<TagCount>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListRelatedTagCountsAsync(long tenantId, [FromQuery]string tagIds, [FromQuery]long? parentPageId, [FromQuery]bool? recursive)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
            IEnumerable<TagCount> tags = await _tagService.ListRelatedTagCountsAsync(tenantId, tagIdCollection, parentPageId, recursive ?? false);
            return Ok(tags);
        }
    }
}
