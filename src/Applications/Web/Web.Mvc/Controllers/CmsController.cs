using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Applications.Web.Mvc.Models;
using Riverside.Cms.Applications.Web.Mvc.Services;
using Microsoft.AspNetCore.Http;
using Riverside.Cms.Services.Core.Common;

namespace Riverside.Cms.Applications.Web.Mvc.Controllers
{
    [MultiTenant(true)]
    public class CmsController : Controller
    {
        private readonly IElementServiceFactory _elementServiceFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPageService _pageService;
        private readonly IPageViewService _pageViewService;
        private readonly ISeoService _seoService;
        private readonly ITagService _tagService;
        private readonly IUserService _userService;

        public CmsController(IElementServiceFactory elementServiceFactory, IHttpContextAccessor httpContextAccessor, IPageService pageService, IPageViewService pageViewService, ISeoService seoService, ITagService tagService, IUserService userService)
        {
            _elementServiceFactory = elementServiceFactory;
            _httpContextAccessor = httpContextAccessor;
            _pageService = pageService;
            _pageViewService = pageViewService;
            _seoService = seoService;
            _tagService = tagService;
            _userService = userService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];
        private string RootUrl => string.Format("{0}://{1}", _httpContextAccessor.HttpContext.Request.Scheme, _httpContextAccessor.HttpContext.Request.Host);

        private async Task<ElementPartialView> GetElementPartialViewAsync(long tenantId, Guid elementTypeId, long elementId, IPageContext context)
        {
            IElementViewModel model = await _elementServiceFactory.GetElementViewModelAsync(tenantId, elementTypeId, elementId, context);
            if (model == null)
                return new ElementPartialView { Name = "~/Views/Elements/NotFound.cshtml" };

            return new ElementPartialView
            {
                Name = $"~/Views/Elements/{elementTypeId}.cshtml",
                Model = model
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

        private async Task<IPageContext> GetPageContextAsync(long tenantId, long pageId, string tags)
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
            IPageContext context = await GetPageContextAsync(tenantId, pageId, tags);

            PageView pageView = await _pageViewService.ReadPageViewAsync(tenantId, pageId);

            Dictionary<long, ElementPartialView> elements = new Dictionary<long, ElementPartialView>();
            foreach (PageViewZone pageViewZone in pageView.PageViewZones)
            {
                foreach (PageViewZoneElement pageViewZoneElement in pageViewZone.PageViewZoneElements)
                {
                    if (!elements.ContainsKey(pageViewZoneElement.ElementId))
                        elements.Add(pageViewZoneElement.ElementId, await GetElementPartialViewAsync(tenantId, pageViewZoneElement.ElementTypeId, pageViewZoneElement.ElementId, context));
                }
            }

            PageViewModel viewModel = new PageViewModel
            {
                View = pageView,
                Elements = elements
            };

            return View("Read", viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> ReadPageTaggedAsync(long pageId, string tags)
        {
            return await ReadPageTaggedAsync(TenantId, pageId, tags);
        }

        [HttpGet]
        public Task<IActionResult> ReadPageAsync(long pageId)
        {
            return ReadPageTaggedAsync(pageId, null);
        }

        [HttpGet]
        public async Task<IActionResult> ReadHomeTaggedAsync(string tags)
        {
            PageListResult result = await _pageService.ListPagesAsync(TenantId, null, false, PageType.Folder, null, SortBy.Created, true, 0, 1);
            return await ReadPageTaggedAsync(TenantId, result.Pages.First().PageId, tags);
        }

        [HttpGet]
        public Task<IActionResult> ReadHomeAsync()
        {
            return ReadHomeTaggedAsync(null);
        }

        [HttpGet]
        public async Task<IActionResult> ReadPageImageAsync(long pageId, PageImageType pageImageType)
        {
            BlobContent blobContent = await _pageService.ReadPageImageAsync(TenantId, pageId, pageImageType);
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpGet]
        public async Task<IActionResult> ReadElementBlobAsync(Guid elementTypeId, long elementId, long blobSetId, string blobLabel)
        {
            BlobContent blobContent = await _elementServiceFactory.GetElementBlobContentAsync(TenantId, elementTypeId, elementId, blobSetId, blobLabel);
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpGet]
        public async Task<IActionResult> ReadUserBlobAsync(long userId, UserImageType userImageType)
        {
            BlobContent blobContent = await _userService.ReadUserImageAsync(TenantId, userId, userImageType);
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpPost]
        public async Task<IActionResult> PerformElementActionAsync(Guid elementTypeId, long elementId, long pageId, [ModelBinder(BinderType = typeof(JsonModelBinder))]string json)
        {
            PageContext context = new PageContext
            {
                PageId = pageId
            };
            object response = await _elementServiceFactory.PerformElementActionAsync(TenantId, elementTypeId, elementId, json, context);
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> RobotsAsync()
        {
            string robots = await _seoService.GetRobotsExclusionStandardAsync(RootUrl);
            return Content(robots);
        }

        [HttpGet]
        public async Task<IActionResult> SitemapAsync()
        {
            string sitemap = await _seoService.GetSitemapAsync(TenantId, RootUrl);
            return Content(sitemap, "application/xml", System.Text.Encoding.UTF8);
        }
    }
}
