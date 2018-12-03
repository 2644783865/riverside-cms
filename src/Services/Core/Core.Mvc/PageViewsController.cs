using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    [MultiTenant()]
    public class PageViewsController : ControllerBase
    {
        private readonly IPageViewService _pageViewService;

        public PageViewsController(IPageViewService pageViewService)
        {
            _pageViewService = pageViewService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [Route("api/v1/core/pageviews/{pageId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageView), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageViewAsync(long pageId)
        {
            PageView pageView = await _pageViewService.ReadPageViewAsync(TenantId, pageId);
            if (pageView == null)
                return NotFound();
            return Ok(pageView);
        }
    }
}
