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

        [HttpGet]
        [Route("api/v1/core/masterpages/{masterPageId:int}/zones")]
        [ProducesResponseType(typeof(IEnumerable<MasterPageZone>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchMasterPageZonesAsync(long masterPageId)
        {
            IEnumerable<MasterPageZone> masterPageZones = await _masterPageService.SearchMasterPageZonesAsync(TenantId, masterPageId);
            return Ok(masterPageZones);
        }

        [HttpGet]
        [Route("api/v1/core/masterpages/{masterPageId:int}/zones/{masterPageZoneId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(MasterPageZone), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadMasterPageZoneAsync(long masterPageId, long masterPageZoneId)
        {
            MasterPageZone masterPageZone = await _masterPageService.ReadMasterPageZoneAsync(TenantId, masterPageId, masterPageZoneId);
            if (masterPageZone == null)
                return NotFound();
            return Ok(masterPageZone);
        }

        [HttpGet]
        [Route("api/v1/core/masterpages/{masterPageId:int}/zones/{masterPageZoneId:int}/elements")]
        [ProducesResponseType(typeof(IEnumerable<MasterPageZoneElement>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchMasterPageZoneElementsAsync(long masterPageId, long masterPageZoneId)
        {
            IEnumerable<MasterPageZoneElement> masterPageZoneElements = await _masterPageService.SearchMasterPageZoneElementsAsync(TenantId, masterPageId, masterPageZoneId);
            return Ok(masterPageZoneElements);
        }

        [HttpGet]
        [Route("api/v1/core/masterpages/{masterPageId:int}/zones/{masterPageZoneId:int}/elements/{masterPageZoneElementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(MasterPageZoneElement), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadMasterPageZoneElementAsync(long masterPageId, long masterPageZoneId, long masterPageZoneElementId)
        {
            MasterPageZoneElement masterPageZoneElement = await _masterPageService.ReadMasterPageZoneElementAsync(TenantId, masterPageId, masterPageZoneId, masterPageZoneElementId);
            if (masterPageZoneElement == null)
                return NotFound();
            return Ok(masterPageZoneElement);
        }
    }
}
