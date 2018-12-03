using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Element.Mvc
{
    [MultiTenant()]
    public class ElementsController : ControllerBase
    {
        private readonly IAlbumElementService _albumElementService;
        private readonly ICarouselElementService _carouselElementService;
        private readonly ICodeSnippetElementService _codeSnippetElementService;
        private readonly IFooterElementService _footerElementService;
        private readonly IFormElementService _formElementService;
        private readonly IForumElementService _forumElementService;
        private readonly IHtmlElementService _htmlElementService;
        private readonly ILatestThreadsElementService _latestThreadsElementService;
        private readonly INavigationBarElementService _navigationBarElementService;
        private readonly IPageHeaderElementService _pageHeaderElementService;
        private readonly IPageListElementService _pageListElementService;
        private readonly IShareElementService _shareElementService;
        private readonly ISocialBarElementService _socialBarElementService;
        private readonly ITableElementService _tableElementService;
        private readonly ITagCloudElementService _tagCloudElementService;
        private readonly ITestimonialElementService _testimonialElementService;

        public ElementsController(IAlbumElementService albumElementService, ICarouselElementService carouselElementService, ICodeSnippetElementService codeSnippetElementService, IFooterElementService footerElementService, IFormElementService formElementService, IForumElementService forumElementService, IHtmlElementService htmlElementService, ILatestThreadsElementService latestThreadsElementService, INavigationBarElementService navigationBarElementService, IPageHeaderElementService pageHeaderElementService, IPageListElementService pageListElementService, IShareElementService shareElementService, ISocialBarElementService socialBarElementService, ITableElementService tableElementService, ITagCloudElementService tagCloudElementService, ITestimonialElementService testimonialElementService)
        {
            _albumElementService = albumElementService;
            _carouselElementService = carouselElementService;
            _codeSnippetElementService = codeSnippetElementService;
            _footerElementService = footerElementService;
            _formElementService = formElementService;
            _forumElementService = forumElementService;
            _htmlElementService = htmlElementService;
            _latestThreadsElementService = latestThreadsElementService;
            _navigationBarElementService = navigationBarElementService;
            _pageHeaderElementService = pageHeaderElementService;
            _pageListElementService = pageListElementService;
            _shareElementService = shareElementService;
            _socialBarElementService = socialBarElementService;
            _tableElementService = tableElementService;
            _tagCloudElementService = tagCloudElementService;
            _testimonialElementService = testimonialElementService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        // ALBUM

        [HttpGet]
        [Route("api/v1/element/elementtypes/b539d2a4-52ae-40d5-b366-e42447b93d15/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(AlbumElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadAlbumElementSettingsAsync(long elementId, [FromQuery]long pageId)
        {
            AlbumElementSettings settings = await _albumElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/b539d2a4-52ae-40d5-b366-e42447b93d15/elements/{elementId:int}/blobsets/{blobSetId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadAlbumElementBlobContentAsync(long elementId, long blobSetId, [FromQuery]string blobLabel)
        {
            BlobContent content = await _albumElementService.ReadBlobContentAsync(TenantId, elementId, blobSetId, blobLabel);
            if (content == null)
                return NotFound();
            return File(content.Stream, content.Type, content.Name);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/b539d2a4-52ae-40d5-b366-e42447b93d15/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<AlbumElementSettings, AlbumElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadAlbumElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<AlbumElementSettings, AlbumElementContent> view = await _albumElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // CAROUSEL

        [HttpGet]
        [Route("api/v1/element/elementtypes/aacb11a0-5532-47cb-aab9-939cee3d5175/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CarouselElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCarouselElementSettingsAsync(long elementId, [FromQuery]long pageId)
        {
            CarouselElementSettings settings = await _carouselElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/aacb11a0-5532-47cb-aab9-939cee3d5175/elements/{elementId:int}/blobsets/{blobSetId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadCarouselElementBlobContentAsync(long elementId, long blobSetId, [FromQuery]string blobLabel)
        {
            BlobContent content = await _carouselElementService.ReadBlobContentAsync(TenantId, elementId, blobSetId, blobLabel);
            if (content == null)
                return NotFound();
            return File(content.Stream, content.Type, content.Name);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/aacb11a0-5532-47cb-aab9-939cee3d5175/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<CarouselElementSettings, CarouselElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCarouselElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<CarouselElementSettings, CarouselElementContent> view = await _carouselElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // CODE SNIPPET

        [HttpGet]
        [Route("api/v1/element/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CodeSnippetElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCodeSnippetElementSettingsAsync(long elementId)
        {
            CodeSnippetElementSettings settings = await _codeSnippetElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<CodeSnippetElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCodeSnippetElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<CodeSnippetElementSettings, object> view = await _codeSnippetElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // FOOTER

        [HttpGet]
        [Route("api/v1/element/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(FooterElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFooterElementSettingsAsync(long elementId)
        {
            FooterElementSettings settings = await _footerElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<FooterElementSettings, FooterElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFooterElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<FooterElementSettings, FooterElementContent> view = await _footerElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // FORM

        [HttpGet]
        [Route("api/v1/element/elementtypes/eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(FormElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFormElementSettingsAsync(long elementId)
        {
            FormElementSettings settings = await _formElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<FormElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFormElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<FormElementSettings, object> view = await _formElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        [HttpPost]
        [Route("api/v1/element/elementtypes/eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc/elements/{elementId:int}/action")]
        public async Task<IActionResult> PerformFormElementActionAsync(long elementId, [FromBody]FormElementActionRequest request, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            FormElementActionResponse response = await _formElementService.PerformElementActionAsync(TenantId, elementId, request, context);
            return Ok(response);
        }

        // FORUM

        [HttpGet]
        [Route("api/v1/element/elementtypes/484192d1-5a4f-496f-981b-7e0120453949/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ForumElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadForumElementSettingsAsync(long elementId)
        {
            ForumElementSettings settings = await _forumElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/484192d1-5a4f-496f-981b-7e0120453949/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<ForumElementSettings, ForumElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadForumElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<ForumElementSettings, ForumElementContent> view = await _forumElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // HTML

        [HttpGet]
        [Route("api/v1/element/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(HtmlElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadHtmlElementSettingsAsync(long elementId, [FromQuery]long pageId)
        {
            HtmlElementSettings settings = await _htmlElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}/blobsets/{blobSetId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadHtmlElementBlobContentAsync(long elementId, long blobSetId, [FromQuery]string blobLabel)
        {
            BlobContent content = await _htmlElementService.ReadBlobContentAsync(TenantId, elementId, blobSetId, blobLabel);
            if (content == null)
                return NotFound();
            return File(content.Stream, content.Type, content.Name);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<HtmlElementSettings, HtmlElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadHtmlElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<HtmlElementSettings, HtmlElementContent> view = await _htmlElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // LATEST THREADS

        [HttpGet]
        [Route("api/v1/element/elementtypes/f9557287-ba01-48e3-9ab4-e2f4831933d0/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(LatestThreadsElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadLatestThreadsElementSettingsAsync(long elementId, [FromQuery]long pageId)
        {
            LatestThreadsElementSettings settings = await _latestThreadsElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/f9557287-ba01-48e3-9ab4-e2f4831933d0/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<LatestThreadsElementSettings, LatestThreadsElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadLatestThreadsElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<LatestThreadsElementSettings, LatestThreadsElementContent> view = await _latestThreadsElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // NAVIGATION BAR

        [HttpGet]
        [Route("api/v1/element/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(NavigationBarElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadNavigationBarElementSettingsAsync(long elementId)
        {
            NavigationBarElementSettings settings = await _navigationBarElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<NavigationBarElementSettings, NavigationBarElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadNavigationBarElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<NavigationBarElementSettings, NavigationBarElementContent> view = await _navigationBarElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // PAGE HEADER

        [HttpGet]
        [Route("api/v1/element/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageHeaderElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageHeaderElementSettingsAsync(long elementId)
        {
            PageHeaderElementSettings settings = await _pageHeaderElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<PageHeaderElementSettings, PageHeaderElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageHeaderElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<PageHeaderElementSettings, PageHeaderElementContent> view = await _pageHeaderElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // PAGE LIST

        [HttpGet]
        [Route("api/v1/element/elementtypes/61f55535-9f3e-4ef5-96a2-bc84d648842a/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageListElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageListElementSettingsAsync(long elementId)
        {
            PageListElementSettings settings = await _pageListElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/61f55535-9f3e-4ef5-96a2-bc84d648842a/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<PageListElementSettings, PageListElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageListElementViewAsync(long elementId, [FromQuery]long pageId, [FromQuery]string tagIds, [FromQuery]string page)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
            IDictionary<string, string> parameters = page != null ? new Dictionary<string, string> { { "page", page } } : null;
            PageContext context = new PageContext { PageId = pageId, Parameters = parameters, TagIds = tagIdCollection };
            IElementView<PageListElementSettings, PageListElementContent> view = await _pageListElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // SHARE

        [HttpGet]
        [Route("api/v1/element/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ShareElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadShareElementSettingsAsync(long elementId)
        {
            ShareElementSettings settings = await _shareElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<ShareElementSettings, ShareElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadShareElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<ShareElementSettings, ShareElementContent> view = await _shareElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // SOCIAL BAR

        [HttpGet]
        [Route("api/v1/element/elementtypes/4e6b936d-e8a1-4ff2-9576-9f9b78f82895/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(SocialBarElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadSocialBarElementSettingsAsync(long elementId)
        {
            SocialBarElementSettings settings = await _socialBarElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/4e6b936d-e8a1-4ff2-9576-9f9b78f82895/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<SocialBarElementSettings, SocialBarElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadSocialBarElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<SocialBarElementSettings, SocialBarElementContent> view = await _socialBarElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // TABLE

        [HttpGet]
        [Route("api/v1/element/elementtypes/252ca19c-d085-4e0d-b70b-da3e1098f51b/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TableElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTableElementSettingsAsync(long elementId)
        {
            TableElementSettings settings = await _tableElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/252ca19c-d085-4e0d-b70b-da3e1098f51b/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<TableElementSettings, TableElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTableElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<TableElementSettings, TableElementContent> view = await _tableElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // TAG CLOUD

        [HttpGet]
        [Route("api/v1/element/elementtypes/b910c231-7dbd-4cad-92ef-775981e895b4/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TagCloudElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTagCloudElementSettingsAsync(long elementId)
        {
            TagCloudElementSettings settings = await _tagCloudElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/b910c231-7dbd-4cad-92ef-775981e895b4/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<TagCloudElementSettings, TagCloudElementContent>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTagCloudElementViewAsync(long elementId, [FromQuery]long pageId, [FromQuery]string tagIds)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(',').Select(long.Parse) : null;
            PageContext context = new PageContext { PageId = pageId, TagIds = tagIdCollection };
            IElementView<TagCloudElementSettings, TagCloudElementContent> view = await _tagCloudElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // TESTIMONIAL

        [HttpGet]
        [Route("api/v1/element/elementtypes/eb479ac9-8c79-4fae-817a-e77fd3dbf05b/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(TestimonialElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTestimonialElementSettingsAsync(long elementId)
        {
            TestimonialElementSettings settings = await _testimonialElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/element/elementtypes/eb479ac9-8c79-4fae-817a-e77fd3dbf05b/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<TestimonialElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTestimonialElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<TestimonialElementSettings, object> view = await _testimonialElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }
    }
}
