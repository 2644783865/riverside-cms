using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Common;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Mortgage.Domain;

namespace Riverside.Cms.Services.Mortgage.Mvc
{
    [MultiTenant()]
    public class MortgagesController : ControllerBase
    {
        private readonly IAmortisationCalculatorElementService _amortisationCalculatorElementService;
        private readonly IBorrowCalculatorElementService _borrowCalculatorElementService;
        private readonly IPayCalculatorElementService _payCalculatorElementService;
        private readonly IRentalCalculatorElementService _rentalCalculatorElementService;
        private readonly IStampDutyCalculatorElementService _stampDutyCalculatorElementService;

        public MortgagesController(IAmortisationCalculatorElementService amortisationCalculatorElementService, IBorrowCalculatorElementService borrowCalculatorElementService, IPayCalculatorElementService payCalculatorElementService, IRentalCalculatorElementService rentalCalculatorElementService, IStampDutyCalculatorElementService stampDutyCalculatorElementService)
        {
            _amortisationCalculatorElementService = amortisationCalculatorElementService;
            _borrowCalculatorElementService = borrowCalculatorElementService;
            _payCalculatorElementService = payCalculatorElementService;
            _rentalCalculatorElementService = rentalCalculatorElementService;
            _stampDutyCalculatorElementService = stampDutyCalculatorElementService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        // AMORTISATION CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/fb7e757d-905c-4ab1-828f-c39baabe55a6/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(AmortisationCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadAmortisationCalculatorElementSettingsAsync(long elementId)
        {
            AmortisationCalculatorElementSettings settings = await _amortisationCalculatorElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/fb7e757d-905c-4ab1-828f-c39baabe55a6/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<AmortisationCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadAmortisationCalculatorElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<AmortisationCalculatorElementSettings, object> view = await _amortisationCalculatorElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // BORROW CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/8373bdac-6f80-4c5e-8e27-1b3a9c92cd7c/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BorrowCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadBorrowCalculatorElementSettingsAsync(long elementId)
        {
            BorrowCalculatorElementSettings settings = await _borrowCalculatorElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/8373bdac-6f80-4c5e-8e27-1b3a9c92cd7c/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<BorrowCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadBorrowCalculatorElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<BorrowCalculatorElementSettings, object> view = await _borrowCalculatorElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // PAY CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/f03aa333-4a52-4716-aa1b-1d0d6a31dc15/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PayCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPayCalculatorElementSettingsAsync(long elementId)
        {
            PayCalculatorElementSettings settings = await _payCalculatorElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/f03aa333-4a52-4716-aa1b-1d0d6a31dc15/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<PayCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPayCalculatorElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<PayCalculatorElementSettings, object> view = await _payCalculatorElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // RENTAL CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/eec21fac-d185-45ee-a8dd-6032679697b1/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RentalCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadRentalCalculatorElementSettingsAsync(long elementId)
        {
            RentalCalculatorElementSettings settings = await _rentalCalculatorElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/eec21fac-d185-45ee-a8dd-6032679697b1/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<RentalCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadRentalCalculatorElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<RentalCalculatorElementSettings, object> view = await _rentalCalculatorElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // STAMP DUTY CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/9c167ed6-1caf-4b2c-8de1-2586d247e28e/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(StampDutyCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadStampDutyCalculatorElementSettingsAsync(long elementId)
        {
            StampDutyCalculatorElementSettings settings = await _stampDutyCalculatorElementService.ReadElementSettingsAsync(TenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/elementtypes/9c167ed6-1caf-4b2c-8de1-2586d247e28e/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<StampDutyCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadStampDutyCalculatorElementViewAsync(long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<StampDutyCalculatorElementSettings, object> view = await _stampDutyCalculatorElementService.ReadElementViewAsync(TenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }
    }
}
