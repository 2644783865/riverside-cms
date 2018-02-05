﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using RiversideCms.Mvc.Models;

namespace RiversideCms.Mvc.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageViewService _pageViewService;

        private readonly IFooterElementService _footerElementService;
        private readonly IPageHeaderElementService _pageHeaderElementService;

        private const long TenantId = 6;

        public PagesController(IFooterElementService footerElementService, IPageHeaderElementService pageHeaderElementService, IPageViewService pageViewService)
        {
            _footerElementService = footerElementService;
            _pageHeaderElementService = pageHeaderElementService;
            _pageViewService = pageViewService;
        }

        private async Task<ElementRender> GetElementRender(long elementId, long pageId)
        {
            ElementRender elementRender = null;

            Guid elementTypeId = new Guid("1cbac30c-5deb-404e-8ea8-aabc20c82aa8");
            elementId = 162;

            switch (elementTypeId.ToString())
            {
                case "f1c2b384-4909-47c8-ada7-cd3cc7f32620":
                    FooterElementView footerElementView = await _footerElementService.GetElementViewAsync(TenantId, elementId, pageId);
                    ElementView<FooterElementSettings, object> footerElementRenderView = new ElementView<FooterElementSettings, object>()
                    {
                        Settings = footerElementView.Settings
                    };
                    elementRender = new ElementRender
                    {
                        PartialViewName = "~/Views/Elements/Footer.cshtml",
                        ElementView = footerElementRenderView
                    };
                    break;

                case "1cbac30c-5deb-404e-8ea8-aabc20c82aa8":
                    PageHeaderElementView pageHeaderElementView = await _pageHeaderElementService.GetElementViewAsync(TenantId, elementId, pageId);
                    ElementView<PageHeaderElementSettings, PageHeaderElementContent> pageHeaderElementRenderView = new ElementView<PageHeaderElementSettings, PageHeaderElementContent>()
                    {
                        Settings = pageHeaderElementView.Settings,
                        Content = pageHeaderElementView.Content
                    };
                    elementRender = new ElementRender
                    {
                        PartialViewName = "~/Views/Elements/PageHeader.cshtml",
                        ElementView = pageHeaderElementRenderView
                    };
                    break;
            }

            return elementRender;
        }

        [HttpGet]
        public async Task<IActionResult> Read(long pageId)
        {
            PageView pageView = await _pageViewService.ReadPageViewAsync(TenantId, pageId);
            pageView.PageViewZones = await _pageViewService.SearchPageViewZonesAsync(TenantId, pageId);
            foreach (PageViewZone pageViewZone in pageView.PageViewZones)
                pageViewZone.PageViewZoneElements = await _pageViewService.SearchPageViewZoneElementsAsync(TenantId, pageId, pageViewZone.MasterPageZoneId);

            Dictionary<long, ElementRender> elements = new Dictionary<long, ElementRender>();
            foreach (PageViewZone pageViewZone in pageView.PageViewZones)
            {
                foreach (PageViewZoneElement pageViewZoneElement in pageViewZone.PageViewZoneElements)
                {
                    if (!elements.ContainsKey(pageViewZoneElement.ElementId))
                        elements.Add(pageViewZoneElement.ElementId, await GetElementRender(pageViewZoneElement.ElementId, pageId));
                }
            }

            PageRender pageRender = new PageRender
            {
                View = pageView,
                Elements = elements
            };

            return View("Read", pageRender);
        }
    }
}
