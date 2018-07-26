﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Element.Domain;

namespace Element.Api.Controllers
{
    public class ElementsController : Controller
    {
        private readonly ICodeSnippetElementService _codeSnippetElementService;
        private readonly IFooterElementService _footerElementService;
        private readonly IHtmlElementService _htmlElementService;
        private readonly INavigationBarElementService _navigationBarElementService;
        private readonly IPageHeaderElementService _pageHeaderElementService;
        private readonly IShareElementService _shareElementService;

        public ElementsController(ICodeSnippetElementService codeSnippetElementService, IFooterElementService footerElementService, IHtmlElementService htmlElementService, INavigationBarElementService navigationBarElementService, IPageHeaderElementService pageHeaderElementService, IShareElementService shareElementService)
        {
            _codeSnippetElementService = codeSnippetElementService;
            _footerElementService = footerElementService;
            _htmlElementService = htmlElementService;
            _navigationBarElementService = navigationBarElementService;
            _pageHeaderElementService = pageHeaderElementService;
            _shareElementService = shareElementService;
        }

        // CODE SNIPPET

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CodeSnippetElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCodeSnippetElementSettings(long tenantId, long elementId)
        {
            CodeSnippetElementSettings elementSettings = await _codeSnippetElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (elementSettings == null)
                return NotFound();
            return Ok(elementSettings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ElementContent), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadCodeSnippetElementContent(long tenantId, long elementId, [FromQuery]long pageId)
        {
            ElementContent elementContent = await _codeSnippetElementService.ReadElementContentAsync(tenantId, elementId, pageId);
            if (elementContent == null)
                return NotFound();
            return Ok(elementContent);
        }

        // FOOTER

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(FooterElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFooterElementSettings(long tenantId, long elementId)
        {
            FooterElementSettings elementSettings = await _footerElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (elementSettings == null)
                return NotFound();
            return Ok(elementSettings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(FooterElementContent), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadFooterElementContent(long tenantId, long elementId, [FromQuery]long pageId)
        {
            FooterElementContent elementContent = await _footerElementService.ReadElementContentAsync(tenantId, elementId, pageId);
            if (elementContent == null)
                return NotFound();
            return Ok(elementContent);
        }

        // HTML

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(HtmlElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadHtmlElementSettings(long tenantId, long elementId)
        {
            HtmlElementSettings elementSettings = await _htmlElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (elementSettings == null)
                return NotFound();
            return Ok(elementSettings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(HtmlElementContent), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadHtmlElementContent(long tenantId, long elementId, [FromQuery]long pageId)
        {
            HtmlElementContent elementContent = await _htmlElementService.ReadElementContentAsync(tenantId, elementId, pageId);
            if (elementContent == null)
                return NotFound();
            return Ok(elementContent);
        }

        // NAVIGATION BAR

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(NavigationBarElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadNavigationBarElementSettings(long tenantId, long elementId)
        {
            NavigationBarElementSettings elementSettings = await _navigationBarElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (elementSettings == null)
                return NotFound();
            return Ok(elementSettings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(NavigationBarElementContent), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadNavigationBarElementContent(long tenantId, long elementId, [FromQuery]long pageId)
        {
            NavigationBarElementContent elementContent = await _navigationBarElementService.ReadElementContentAsync(tenantId, elementId, pageId);
            if (elementContent == null)
                return NotFound();
            return Ok(elementContent);
        }

        // PAGE HEADER

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageHeaderElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageHeaderElementSettings(long tenantId, long elementId)
        {
            PageHeaderElementSettings elementSettings = await _pageHeaderElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (elementSettings == null)
                return NotFound();
            return Ok(elementSettings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PageHeaderElementContent), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadPageHeaderElementContent(long tenantId, long elementId, [FromQuery]long pageId)
        {
            PageHeaderElementContent elementContent = await _pageHeaderElementService.ReadElementContentAsync(tenantId, elementId, pageId);
            if (elementContent == null)
                return NotFound();
            return Ok(elementContent);
        }

        // SHARE

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ShareElementSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadShareElementSettings(long tenantId, long elementId)
        {
            ShareElementSettings elementSettings = await _shareElementService.ReadElementSettingsAsync(tenantId, elementId);
            if (elementSettings == null)
                return NotFound();
            return Ok(elementSettings);
        }

        [HttpGet]
        [Route("api/v1/element/tenants/{tenantId:int}/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ShareElementContent), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadShareElementContent(long tenantId, long elementId, [FromQuery]long pageId)
        {
            ShareElementContent elementContent = await _shareElementService.ReadElementContentAsync(tenantId, elementId, pageId);
            if (elementContent == null)
                return NotFound();
            return Ok(elementContent);
        }
    }
}
