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
    public class PagesController : Controller
    {
        private readonly IPageService _pageService;

        public const int DefaultPageSize = 10;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages/{pageId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Page), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPage(long tenantId, long pageId)
        {
            Page page = await _pageService.ReadPageAsync(tenantId, pageId);
            if (page == null)
                return NotFound();
            return Ok(page);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages/{pageId:int}/images/{pageImageType}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadPageImage(long tenantId, long pageId, PageImageType pageImageType)
        {
            BlobContent blobContent = await _pageService.ReadPageImageAsync(tenantId, pageId, pageImageType);
            if (blobContent == null)
                return NotFound();
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages/{pageId:int}/hierarchy")]
        [ProducesResponseType(typeof(IEnumerable<Page>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListPagesInHierarchy(long tenantId, long pageId)
        {
            IEnumerable<Page> pages = await _pageService.ListPagesInHierarchyAsync(tenantId, pageId);
            return Ok(pages);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages")]
        [ProducesResponseType(typeof(PageListResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListPages(long tenantId, [FromQuery]long? parentPageId, [FromQuery]bool? recursive, [FromQuery]PageType? pageType, [FromQuery]string tagIds, [FromQuery]SortBy? sortBy, [FromQuery]bool? sortAsc, [FromQuery]int? pageIndex, [FromQuery]int? pageSize)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(",").Select(long.Parse) : null;
            PageListResult pages = await _pageService.ListPages(tenantId, parentPageId, recursive ?? false, pageType ?? PageType.Document, tagIdCollection, sortBy ?? SortBy.Created, sortAsc ?? false, pageIndex ?? 0, pageSize ?? DefaultPageSize);
            return Ok(pages);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages/{pageId:int}/zones")]
        [ProducesResponseType(typeof(IEnumerable<PageZone>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchPageZones(long tenantId, long pageId)
        {
            IEnumerable<PageZone> pageZones = await _pageService.SearchPageZonesAsync(tenantId, pageId);
            return Ok(pageZones);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages/{pageId:int}/zones/{pageZoneId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageZone), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageZone(long tenantId, long pageId, long pageZoneId)
        {
            PageZone pageZone = await _pageService.ReadPageZoneAsync(tenantId, pageId, pageZoneId);
            if (pageZone == null)
                return NotFound();
            return Ok(pageZone);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages/{pageId:int}/zones/{pageZoneId:int}/elements")]
        [ProducesResponseType(typeof(IEnumerable<PageZoneElement>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchPageZoneElements(long tenantId, long pageId, long pageZoneId)
        {
            IEnumerable<PageZoneElement> pageZoneElements = await _pageService.SearchPageZoneElementsAsync(tenantId, pageId, pageZoneId);
            return Ok(pageZoneElements);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/pages/{pageId:int}/zones/{pageZoneId:int}/elements/{pageZoneElementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageZoneElement), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageZoneElement(long tenantId, long pageId, long pageZoneId, long pageZoneElementId)
        {
            PageZoneElement pageZoneElement = await _pageService.ReadPageZoneElementAsync(tenantId, pageId, pageZoneId, pageZoneElementId);
            if (pageZoneElement == null)
                return NotFound();
            return Ok(pageZoneElement);
        }
    }
}
