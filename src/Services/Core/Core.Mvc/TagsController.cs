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
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [Route("api/v1/core/tags/{tagId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Tag), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTagAsync(long tagId)
        {
            Tag tag = await _tagService.ReadTagAsync(TenantId, tagId);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }

        [HttpGet]
        [Route("api/v1/core/tags")]
        [ProducesResponseType(typeof(IEnumerable<Tag>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListTagsAsync([FromQuery]string tagIds, [FromQuery]string tagNames)
        {
            IEnumerable<Tag> tags = null;
            if (tagIds != null)
            {
                IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
                tags = await _tagService.ListTagsAsync(TenantId, tagIdCollection);
            }
            else if (tagNames != null)
            {
                IEnumerable<string> tagNameCollection = tagNames.Split(',').Select(t => t.Trim()).Distinct().Where(t => t != string.Empty);
                tags = await _tagService.ListTagsAsync(TenantId, tagNameCollection);
            }
            else
            {

            }
            return Ok(tags);
        }

        [HttpGet]
        [Route("api/v1/core/tags/counts")]
        [ProducesResponseType(typeof(IEnumerable<TagCount>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListTagCountsAsync([FromQuery]long? parentPageId, [FromQuery]bool? recursive)
        {
            IEnumerable<TagCount> tags = await _tagService.ListTagCountsAsync(TenantId, parentPageId, recursive ?? false);
            return Ok(tags);
        }

        [HttpGet]
        [Route("api/v1/core/tags/related/counts")]
        [ProducesResponseType(typeof(IEnumerable<TagCount>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListRelatedTagCountsAsync([FromQuery]string tagIds, [FromQuery]long? parentPageId, [FromQuery]bool? recursive)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
            IEnumerable<TagCount> tags = await _tagService.ListRelatedTagCountsAsync(TenantId, tagIdCollection, parentPageId, recursive ?? false);
            return Ok(tags);
        }
    }
}
