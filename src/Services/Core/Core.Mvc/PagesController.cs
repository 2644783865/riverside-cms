using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Core.Mvc
{
    [Authorize]
    [MultiTenant()]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;

        public const int DefaultPageSize = 10;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/pages/{pageId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Page), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageAsync(long pageId)
        {
            Page page = await _pageService.ReadPageAsync(TenantId, pageId);
            if (page == null)
                return NotFound();
            return Ok(page);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/pages/{pageId:int}/images/{pageImageType}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadPageImageAsync(long pageId, PageImageType pageImageType)
        {
            BlobContent blobContent = await _pageService.ReadPageImageAsync(TenantId, pageId, pageImageType);
            if (blobContent == null)
                return NotFound();
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Policy = "UpdatePages")]
        [Route("api/v1/core/pages/{pageId:int}")]
        public async Task<IActionResult> UpdatePageAsync(long pageId, [FromBody]Page page)
        {
            try
            {
                await _pageService.UpdatePageAsync(TenantId, pageId, page);
                return Ok();
            }
            catch (ValidationErrorException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/pages/{pageId:int}/hierarchy")]
        [ProducesResponseType(typeof(IEnumerable<Page>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListPagesInHierarchyAsync(long pageId)
        {
            IEnumerable<Page> pages = await _pageService.ListPagesInHierarchyAsync(TenantId, pageId);
            return Ok(pages);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/pages")]
        [ProducesResponseType(typeof(PageListResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Page>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListPagesAsync([FromQuery]long? parentPageId, [FromQuery]bool? recursive, [FromQuery]PageType? pageType, [FromQuery]string tagIds, [FromQuery]SortBy? sortBy, [FromQuery]bool? sortAsc, [FromQuery]int? pageIndex, [FromQuery]int? pageSize, [FromQuery]string pageIds)
        {
            object result = null;
            if (pageIds != null)
            {
                IEnumerable<long> pageIdCollection = !string.IsNullOrWhiteSpace(pageIds) ? pageIds.Split(',').Select(long.Parse) : null;
                result = await _pageService.ListPagesAsync(TenantId, pageIdCollection);
            }
            else
            {
                IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
                result = await _pageService.ListPagesAsync(TenantId, parentPageId, recursive ?? false, pageType ?? PageType.Document, tagIdCollection, sortBy ?? SortBy.Created, sortAsc ?? false, pageIndex ?? 0, pageSize ?? DefaultPageSize);
            }
            return Ok(result);
        }
    }
}
