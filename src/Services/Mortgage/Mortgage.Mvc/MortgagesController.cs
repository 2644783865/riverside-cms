using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Mortgage.Domain;

namespace Riverside.Cms.Services.Mortgage.Mvc
{
    public class MortgagesController : Controller
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

        // AMORTISATION CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/fb7e757d-905c-4ab1-828f-c39baabe55a6/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(AmortisationCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadAmortisationCalculatorElementSettingsAsync(long tenantId, long elementId)
        {
            AmortisationCalculatorElementSettings settings = await _amortisationCalculatorElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/fb7e757d-905c-4ab1-828f-c39baabe55a6/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<AmortisationCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadAmortisationCalculatorElementViewAsync(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<AmortisationCalculatorElementSettings, object> view = await _amortisationCalculatorElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // BORROW CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/8373bdac-6f80-4c5e-8e27-1b3a9c92cd7c/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BorrowCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadBorrowCalculatorElementSettingsAsync(long tenantId, long elementId)
        {
            BorrowCalculatorElementSettings settings = await _borrowCalculatorElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/8373bdac-6f80-4c5e-8e27-1b3a9c92cd7c/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<BorrowCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadBorrowCalculatorElementViewAsync(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<BorrowCalculatorElementSettings, object> view = await _borrowCalculatorElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // PAY CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/f03aa333-4a52-4716-aa1b-1d0d6a31dc15/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PayCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPayCalculatorElementSettingsAsync(long tenantId, long elementId)
        {
            PayCalculatorElementSettings settings = await _payCalculatorElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/f03aa333-4a52-4716-aa1b-1d0d6a31dc15/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<PayCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPayCalculatorElementViewAsync(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<PayCalculatorElementSettings, object> view = await _payCalculatorElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // RENTAL CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/eec21fac-d185-45ee-a8dd-6032679697b1/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RentalCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadRentalCalculatorElementSettingsAsync(long tenantId, long elementId)
        {
            RentalCalculatorElementSettings settings = await _rentalCalculatorElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/eec21fac-d185-45ee-a8dd-6032679697b1/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<RentalCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadRentalCalculatorElementViewAsync(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<RentalCalculatorElementSettings, object> view = await _rentalCalculatorElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }

        // STAMP DUTY CALCULATOR

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/9c167ed6-1caf-4b2c-8de1-2586d247e28e/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(StampDutyCalculatorElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadStampDutyCalculatorElementSettingsAsync(long tenantId, long elementId)
        {
            StampDutyCalculatorElementSettings settings = await _stampDutyCalculatorElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return NotFound();
            return Ok(settings);
        }

        [HttpGet]
        [Route("api/v1/mortgage/tenants/{tenantId:int}/elementtypes/9c167ed6-1caf-4b2c-8de1-2586d247e28e/elements/{elementId:int}/view")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IElementView<StampDutyCalculatorElementSettings, object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadStampDutyCalculatorElementViewAsync(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageContext context = new PageContext { PageId = pageId };
            IElementView<StampDutyCalculatorElementSettings, object> view = await _stampDutyCalculatorElementService.ReadElementViewAsync(tenantId, elementId, context);
            if (view == null)
                return NotFound();
            return Ok(view);
        }
    }
}
