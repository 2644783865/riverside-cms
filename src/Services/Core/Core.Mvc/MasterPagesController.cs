using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    [MultiTenant()]
    public class MasterPagesController : ControllerBase
    {
        private readonly IMasterPageService _masterPageService;

        public MasterPagesController(IMasterPageService masterPageService)
        {
            _masterPageService = masterPageService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [Route("api/v1/core/masterpages/{masterPageId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(MasterPage), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadMasterPageAsync(long masterPageId)
        {
            MasterPage masterPage = await _masterPageService.ReadMasterPageAsync(TenantId, masterPageId);
            if (masterPage == null)
                return NotFound();
            return Ok(masterPage);
        }
    }
}
