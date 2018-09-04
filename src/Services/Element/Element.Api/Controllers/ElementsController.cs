using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Storage.Client;

namespace Element.Api.Controllers
{
    public class ElementsController : Controller
    {
        private readonly ICodeSnippetElementService _codeSnippetElementService;
        private readonly IFooterElementService _footerElementService;
        private readonly IHtmlElementService _htmlElementService;
        private readonly ILatestThreadsElementService _latestThreadsElementService;
        private readonly INavigationBarElementService _navigationBarElementService;
        private readonly IPageHeaderElementService _pageHeaderElementService;
        private readonly IPageListElementService _pageListElementService;
        private readonly IShareElementService _shareElementService;
        private readonly ISocialBarElementService _socialBarElementService;
        private readonly ITagCloudElementService _tagCloudElementService;

        public ElementsController(ICodeSnippetElementService codeSnippetElementService, IFooterElementService footerElementService, IHtmlElementService htmlElementService, ILatestThreadsElementService latestThreadsElementService, INavigationBarElementService navigationBarElementService, IPageHeaderElementService pageHeaderElementService, IPageListElementService pageListElementService, IShareElementService shareElementService, ISocialBarElementService socialBarElementService, ITagCloudElementService tagCloudElementService)
        {
            _codeSnippetElementService = codeSnippetElementService;
            _footerElementService = footerElementService;
            _htmlElementService = htmlElementService;
            _latestThreadsElementService = latestThreadsElementService;
            _navigationBarElementService = navigationBarElementService;
            _pageHeaderElementService = pageHeaderElementService;
            _pageListElementService = pageListElementService;
            _shareElementService = shareElementService;
            _socialBarElementService = socialBarElementService;
            _tagCloudElementService = tagCloudElementService;
        }

        // CODE SNIPPET

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CodeSnippetElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCodeSnippetElementSettings(long tenantId, long elementId)
        {
            CodeSnippetElementSettings settings = await _codeSnippetElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<CodeSnippetElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCodeSnippetElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<CodeSnippetElementSettings, object> view = await _codeSnippetElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // FOOTER

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(FooterElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFooterElementSettings(long tenantId, long elementId)
        {
            FooterElementSettings settings = await _footerElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<FooterElementSettings, FooterElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFooterElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<FooterElementSettings, FooterElementContent> view = await _footerElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // HTML

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(HtmlElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadHtmlElementSettings(long tenantId, long elementId, [FromQuery]long pageId)
        {
            HtmlElementSettings settings = await _htmlElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}/blobsets/{blobSetId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadHtmlElementBlobContent(long tenantId, long elementId, long blobSetId, [FromQuery]PageImageType imageType)
        {
            BlobContent content = await _htmlElementService.ReadBlobContentAsync(tenantId, elementId, blobSetId, imageType);
            if (content == null)
                return NotFound();
            return File(content.Stream, content.Type, content.Name);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<HtmlElementSettings, HtmlElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadHtmlElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<HtmlElementSettings, HtmlElementContent> view = await _htmlElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // LATEST THREADS

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/f9557287-ba01-48e3-9ab4-e2f4831933d0/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(LatestThreadsElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadLatestThreadsElementSettings(long tenantId, long elementId, [FromQuery]long pageId)
        {
            LatestThreadsElementSettings settings = await _latestThreadsElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/f9557287-ba01-48e3-9ab4-e2f4831933d0/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<LatestThreadsElementSettings, LatestThreadsElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadLatestThreadsElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<LatestThreadsElementSettings, LatestThreadsElementContent> view = await _latestThreadsElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // NAVIGATION BAR

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(NavigationBarElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadNavigationBarElementSettings(long tenantId, long elementId)
        {
            NavigationBarElementSettings settings = await _navigationBarElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<NavigationBarElementSettings, NavigationBarElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadNavigationBarElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<NavigationBarElementSettings, NavigationBarElementContent> view = await _navigationBarElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // PAGE HEADER

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageHeaderElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageHeaderElementSettings(long tenantId, long elementId)
        {
            PageHeaderElementSettings settings = await _pageHeaderElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<PageHeaderElementSettings, PageHeaderElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageHeaderElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<PageHeaderElementSettings, PageHeaderElementContent> view = await _pageHeaderElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // PAGE LIST

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/61f55535-9f3e-4ef5-96a2-bc84d648842a/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageListElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageListElementSettings(long tenantId, long elementId)
        {
            PageListElementSettings settings = await _pageListElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/61f55535-9f3e-4ef5-96a2-bc84d648842a/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<PageListElementSettings, PageListElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageListElementView(long tenantId, long elementId, [FromQuery]long pageId, [FromQuery]string tagIds, [FromQuery]string page)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(",").Select(long.Parse) : null;
            IDictionary<string, string> parameters = page != null ? new Dictionary<string, string> { { "page", page } } : null;
            PageContext context = new PageContext { PageId = pageId, Parameters = parameters, TagIds = tagIdCollection };
            IElementView<PageListElementSettings, PageListElementContent> view = await _pageListElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // SHARE

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ShareElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadShareElementSettings(long tenantId, long elementId)
        {
            ShareElementSettings settings = await _shareElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<ShareElementSettings, ShareElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadShareElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<ShareElementSettings, ShareElementContent> view = await _shareElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // SOCIAL BAR

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/4e6b936d-e8a1-4ff2-9576-9f9b78f82895/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(SocialBarElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadSocialBarElementSettings(long tenantId, long elementId)
        {
            SocialBarElementSettings settings = await _socialBarElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/4e6b936d-e8a1-4ff2-9576-9f9b78f82895/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<SocialBarElementSettings, SocialBarElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadSocialBarElementView(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<SocialBarElementSettings, SocialBarElementContent> view = await _socialBarElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // TAG CLOUD

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/b910c231-7dbd-4cad-92ef-775981e895b4/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TagCloudElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTagCloudElementSettings(long tenantId, long elementId)
        {
            TagCloudElementSettings settings = await _tagCloudElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/b910c231-7dbd-4cad-92ef-775981e895b4/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<TagCloudElementSettings, TagCloudElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTagCloudElementView(long tenantId, long elementId, [FromQuery]long pageId, [FromQuery]string tagIds)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(",").Select(long.Parse) : null;
            PageContext context = new PageContext { PageId = pageId, TagIds = tagIdCollection };
            IElementView<TagCloudElementSettings, TagCloudElementContent> view = await _tagCloudElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }
    }
}
