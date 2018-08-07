﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class ShareElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public bool ShareOnDigg { get; set; }
        public bool ShareOnFacebook { get; set; }
        public bool ShareOnGoogle { get; set; }
        public bool ShareOnLinkedIn { get; set; }
        public bool ShareOnPinterest { get; set; }
        public bool ShareOnReddit { get; set; }
        public bool ShareOnStumbleUpon { get; set; }
        public bool ShareOnTumblr { get; set; }
        public bool ShareOnTwitter { get; set; }
    }

    public class ShareElementContent : ElementContent
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Via { get; set; }
        public string Hashtags { get; set; }
        public string Image { get; set; }
        public string IsVideo { get; set; }
    }

    public interface IShareElementService : IElementSettingsService<ShareElementSettings>, IElementViewService<ShareElementSettings, ShareElementContent>
    {
    }

    public class ShareElementService : IShareElementService
    {
        private readonly IElementRepository<ShareElementSettings> _elementRepository;
        private readonly IPageService _pageService;

        public ShareElementService(IElementRepository<ShareElementSettings> elementRepository, IPageService pageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
        }

        public Task<ShareElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<ShareElementSettings, ShareElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            ShareElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            ShareElementContent content = new ShareElementContent
            {
                TenantId = tenantId,
                ElementId = elementId,
                ElementTypeId = settings.ElementTypeId
            };

            Page page = await _pageService.ReadPageAsync(tenantId, context.PageId);

            content.Description = page.Description ?? string.Empty;
            content.Hashtags = string.Empty;
            content.Image = string.Empty;
            content.IsVideo = string.Empty;
            content.Title = page.Name;

            return new ElementView<ShareElementSettings, ShareElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
