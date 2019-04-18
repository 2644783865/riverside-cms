using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Mvc
{
    [Authorize]
    [MultiTenant()]
    public class ElementsController : ControllerBase
    {
        private readonly IElementService _elementService;

        public ElementsController(IElementService elementService)
        {
            _elementService = elementService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/elementtypes/{elementTypeId:guid}/elements/{elementId:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ElementDefinition), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadElementDefinitionAsync(Guid elementTypeId, long elementId)
        {
            ElementDefinition elementDefinition = await _elementService.ReadElementDefinitionAsync(TenantId, elementTypeId, elementId);
            if (elementDefinition == null)
                return NotFound();
            return Ok(elementDefinition);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/elementtypes/{elementTypeId:guid}/elements")]
        [ProducesResponseType(typeof(IEnumerable<ElementDefinition>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListElementDefinitionsAsync(Guid elementTypeId, [FromQuery]string elementIds)
        {
            IEnumerable<long> elementIdCollection = !string.IsNullOrWhiteSpace(elementIds) ? elementIds.Split(',').Select(long.Parse) : null;
            IEnumerable<ElementDefinition> elementDefinitions = await _elementService.ListElementDefinitionsAsync(TenantId, elementTypeId, elementIdCollection);
            return Ok(elementDefinitions);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/core/elementtypes")]
        [ProducesResponseType(typeof(IEnumerable<ElementType>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListElementTypesAsync([FromQuery]string elementTypeIds)
        {
            IEnumerable<Guid> elementTypeIdCollection = !string.IsNullOrWhiteSpace(elementTypeIds) ? elementTypeIds.Split(',').Select(Guid.Parse) : null;
            return Ok(elementTypeIdCollection == null ? await _elementService.ListElementTypesAsync() : await _elementService.ListElementTypesAsync(elementTypeIdCollection));
        }
    }
}
