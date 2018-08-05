using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Services.Storage.Client;
using RiversideCms.Mvc.Models;
using RiversideCms.Mvc.Services;

namespace RiversideCms.Mvc.Controllers
{
    public class PagesController : Controller
    {
        private readonly IElementServiceFactory _elementServiceFactory;
        private readonly IPageService _pageService;
        private readonly IPageViewService _pageViewService;
        private readonly ITagService _tagService;

        private const long TenantId = 6;

        public PagesController(IElementServiceFactory elementServiceFactory, IPageService pageService, IPageViewService pageViewService, ITagService tagService)
        {
            _elementServiceFactory = elementServiceFactory;
            _pageService = pageService;
            _pageViewService = pageViewService;
            _tagService = tagService;
        }

        private async Task<ElementRender> GetElementRender(long tenantId, Guid elementTypeId, long elementId, long pageId, IEnumerable<long> tagIds)
        {
            IElementView elementView = await _elementServiceFactory.GetElementViewAsync(tenantId, elementTypeId, elementId, pageId, tagIds);
            if (elementView == null)
                return new ElementRender { PartialViewName = "~/Views/Elements/NotFound.cshtml" };

            return new ElementRender
            {
                PartialViewName = $"~/Views/Elements/{elementTypeId}.cshtml",
                ElementView = elementView
            };
        }

        private async Task<IEnumerable<long>> GetTagIds(long tenantId, string tagNames)
        {
            if (string.IsNullOrWhiteSpace(tagNames))
                return null;
            IEnumerable<string> tagNameCollection = tagNames.Split("+").Select(t => t.Trim()).Distinct().Where(t => t != string.Empty);
            IEnumerable<Tag> tags = await _tagService.ListTagsAsync(tenantId, tagNameCollection);
            return tags.Select(t => t.TagId);
        }

        [HttpGet]
        public async Task<IActionResult> ReadTagged(long pageId, string tags)
        {
            long tenantId = TenantId;

            IEnumerable<long> tagIds = await GetTagIds(tenantId, tags);

            PageView pageView = await _pageViewService.ReadPageViewAsync(tenantId, pageId);
            pageView.PageViewZones = await _pageViewService.SearchPageViewZonesAsync(tenantId, pageId);
            foreach (PageViewZone pageViewZone in pageView.PageViewZones)
                pageViewZone.PageViewZoneElements = await _pageViewService.SearchPageViewZoneElementsAsync(tenantId, pageId, pageViewZone.MasterPageZoneId);

            Dictionary<long, ElementRender> elements = new Dictionary<long, ElementRender>();
            foreach (PageViewZone pageViewZone in pageView.PageViewZones)
            {
                foreach (PageViewZoneElement pageViewZoneElement in pageViewZone.PageViewZoneElements)
                {
                    if (!elements.ContainsKey(pageViewZoneElement.ElementId))
                        elements.Add(pageViewZoneElement.ElementId, await GetElementRender(tenantId, pageViewZoneElement.ElementTypeId, pageViewZoneElement.ElementId, pageId, tagIds));
                }
            }

            PageRender pageRender = new PageRender
            {
                View = pageView,
                Elements = elements
            };

            return View("Read", pageRender);
        }

        [HttpGet]
        public Task<IActionResult> Read(long pageId)
        {
            return ReadTagged(pageId, null);
        }

        [HttpGet]
        public async Task<IActionResult> ReadImage(long pageId, PageImageType pageImageType)
        {
            long tenantId = TenantId;

            BlobContent blobContent = await _pageService.ReadPageImageAsync(tenantId, pageId, pageImageType);

            return File(blobContent.Stream, blobContent.Type);
        }
    }
}
