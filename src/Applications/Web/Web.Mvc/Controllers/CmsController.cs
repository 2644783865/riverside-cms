﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Services.Storage.Client;
using Riverside.Cms.Applications.Web.Mvc.Models;
using Riverside.Cms.Applications.Web.Mvc.Services;

namespace Riverside.Cms.Applications.Web.Mvc.Controllers
{
    public class CmsController : Controller
    {
        private readonly IDomainService _domainService;
        private readonly IElementServiceFactory _elementServiceFactory;
        private readonly IPageService _pageService;
        private readonly IPageViewService _pageViewService;
        private readonly ITagService _tagService;
        private readonly IUserService _userService;

        public CmsController(IDomainService domainService, IElementServiceFactory elementServiceFactory, IPageService pageService, IPageViewService pageViewService, ITagService tagService, IUserService userService)
        {
            _domainService = domainService;
            _elementServiceFactory = elementServiceFactory;
            _pageService = pageService;
            _pageViewService = pageViewService;
            _tagService = tagService;
            _userService = userService;
        }

        /// <summary>
        /// Returns root URL of current request. For example, the URI "http://localhost:7823/article/1" has root URI "http://localhost:7823".
        /// </summary>
        /// <returns>Root URL of current request.</returns>
        public string GetRootUrl()
        {
            return string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host);
        }

        private async Task<long> GetTenantIdAsync()
        {
            string url = GetRootUrl();
            WebDomain domain = await _domainService.ReadDomainByUrlAsync(url);
            return domain.TenantId;
        }

        private async Task<ElementRender> GetElementRenderAsync(long tenantId, Guid elementTypeId, long elementId, PageContext context)
        {
            IElementView elementView = await _elementServiceFactory.GetElementViewAsync(tenantId, elementTypeId, elementId, context);
            if (elementView == null)
                return new ElementRender { PartialViewName = "~/Views/Elements/NotFound.cshtml" };

            return new ElementRender
            {
                PartialViewName = $"~/Views/Elements/{elementTypeId}.cshtml",
                ElementView = elementView
            };
        }

        private async Task<IEnumerable<long>> GetTagIdsAsync(long tenantId, string tagNames)
        {
            if (string.IsNullOrWhiteSpace(tagNames))
                return null;
            IEnumerable<string> tagNameCollection = tagNames.Split("+").Select(t => t.Trim()).Distinct().Where(t => t != string.Empty);
            IEnumerable<Tag> tags = await _tagService.ListTagsAsync(tenantId, tagNameCollection);
            return tags.Select(t => t.TagId);
        }

        private async Task<PageContext> GetPageContextAsync(long tenantId, long pageId, string tags)
        {
            return new PageContext
            {
                PageId = pageId,
                Parameters = HttpContext.Request.Query.ToDictionary(q => q.Key, q => q.Value.First()),
                TagIds = await GetTagIdsAsync(tenantId, tags)
            };
        }

        private async Task<IActionResult> ReadPageTaggedAsync(long tenantId, long pageId, string tags)
        {
            PageContext context = await GetPageContextAsync(tenantId, pageId, tags);

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
                        elements.Add(pageViewZoneElement.ElementId, await GetElementRenderAsync(tenantId, pageViewZoneElement.ElementTypeId, pageViewZoneElement.ElementId, context));
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
        public async Task<IActionResult> ReadPageTaggedAsync(long pageId, string tags)
        {
            long tenantId = await GetTenantIdAsync();
            return await ReadPageTaggedAsync(tenantId, pageId, tags);
        }

        [HttpGet]
        public Task<IActionResult> ReadPageAsync(long pageId)
        {
            return ReadPageTaggedAsync(pageId, null);
        }

        [HttpGet]
        public async Task<IActionResult> ReadHomeTaggedAsync(string tags)
        {
            long tenantId = await GetTenantIdAsync();
            PageListResult result = await _pageService.ListPagesAsync(tenantId, null, false, PageType.Folder, null, SortBy.Created, true, 0, 1);
            return await ReadPageTaggedAsync(tenantId, result.Pages.First().PageId, tags);
        }

        [HttpGet]
        public Task<IActionResult> ReadHomeAsync()
        {
            return ReadHomeTaggedAsync(null);
        }

        [HttpGet]
        public async Task<IActionResult> ReadPageImageAsync(long pageId, PageImageType pageImageType)
        {
            long tenantId = await GetTenantIdAsync();

            BlobContent blobContent = await _pageService.ReadPageImageAsync(tenantId, pageId, pageImageType);

            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpGet]
        public async Task<IActionResult> ReadElementBlobAsync(Guid elementTypeId, long elementId, long blobSetId, string blobLabel)
        {
            long tenantId = await GetTenantIdAsync();

            BlobContent blobContent = await _elementServiceFactory.GetElementBlobContentAsync(tenantId, elementTypeId, elementId, blobSetId, blobLabel);

            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpGet]
        public async Task<IActionResult> ReadUserBlobAsync(long userId, UserImageType userImageType)
        {
            long tenantId = await GetTenantIdAsync();

            BlobContent blobContent = await _userService.ReadUserImageAsync(tenantId, userId, userImageType);

            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpPost]
        public async Task<IActionResult> PerformElementActionAsync(Guid elementTypeId, long elementId, long pageId, [ModelBinder(BinderType = typeof(JsonModelBinder))]string json)
        {
            PageContext context = new PageContext
            {
                PageId = pageId
            };
            long tenantId = await GetTenantIdAsync();
            object response = await _elementServiceFactory.PerformElementActionAsync(tenantId, elementTypeId, elementId, json, context);
            return Json(response);
        }
    }
}
