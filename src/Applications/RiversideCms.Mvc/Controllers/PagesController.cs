﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Client;
using RiversideCms.Mvc.Models;

namespace RiversideCms.Mvc.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageViewService _pageViewService;

        private const long TenantId = 6;

        public PagesController(IPageViewService pageViewService)
        {
            _pageViewService = pageViewService;
        }

        [HttpGet]
        public async Task<IActionResult> Read(long id)
        {
            PageView pageView = await _pageViewService.ReadPageViewAsync(TenantId, id);
            pageView.PageViewZones = await _pageViewService.SearchPageViewZonesAsync(TenantId, id);
            foreach (PageViewZone pageViewZone in pageView.PageViewZones)
                pageViewZone.PageViewZoneElements = await _pageViewService.SearchPageViewZoneElementsAsync(TenantId, id, pageViewZone.MasterPageZoneId);
            return View("Read", pageView);
        }
    }
}
