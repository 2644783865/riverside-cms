﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Element.Client
{
    public class PageHeaderElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public bool ShowName { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowCreated { get; set; }
        public bool ShowUpdated { get; set; }
        public bool ShowOccurred { get; set; }
        public bool ShowBreadcrumbs { get; set; }
    }

    public class PageHeaderBreadcrumb
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string PageName { get; set; }
    }

    public class PageHeaderElementContent : IElementContent
    {
        public Page Page { get; set; }
        public Dictionary<long, BlobImage> Images { get; set; }
        public IEnumerable<PageHeaderBreadcrumb> Breadcrumbs { get; set; }
    }

    public interface IPageHeaderElementService : IElementSettingsService<PageHeaderElementSettings>, IElementContentService<PageHeaderElementContent>
    {
    }

    public class PageHeaderElementService : IPageHeaderElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public PageHeaderElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<PageHeaderElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<PageHeaderElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<PageHeaderElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/1cbac30c-5deb-404e-8ea8-aabc20c82aa8/elements/{elementId}/content?pageid={pageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<PageHeaderElementContent>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
