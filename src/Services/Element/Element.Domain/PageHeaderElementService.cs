﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class PageHeaderElementService : IPageHeaderElementService
    {
        private readonly IElementRepository<PageHeaderElementSettings> _elementRepository;
        private readonly IPageService _pageService;

        public PageHeaderElementService(IElementRepository<PageHeaderElementSettings> elementRepository, IPageService pageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
        }

        public Task<PageHeaderElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private async void PopulatePageHierarchy(Page page)
        {
            Page hierarchyPage = page;
            while (hierarchyPage != null && hierarchyPage.ParentPageId != null)
            {
                hierarchyPage.ParentPage = await _pageService.ReadPageAsync(page.TenantId, hierarchyPage.ParentPageId.Value);
                hierarchyPage = hierarchyPage.ParentPage;
            }
        }

        public async Task<PageHeaderElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            PageHeaderElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            Page page = await _pageService.ReadPageAsync(tenantId, pageId);

            if (elementSettings.ShowBreadcrumbs)
                PopulatePageHierarchy(page);

            return new PageHeaderElementContent
            {
                Page = page
            };
        }
    }
}
