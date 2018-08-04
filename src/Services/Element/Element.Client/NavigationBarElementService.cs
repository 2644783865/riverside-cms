﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Element.Client
{
    public class NavigationBarTab
    {
        public long PageId { get; set; }
        public long TabId { get; set; }
        public string Name { get; set; }
    }

    public class NavigationBarContentTab
    {
        public bool Active { get; set; }
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string PageName { get; set; }
    }

    public class NavigationBarElementSettings : ElementSettings
    {
        public string Brand { get; set; }
        public bool ShowLoggedOnUserOptions { get; set; }
        public bool ShowLoggedOffUserOptions { get; set; }
        public IEnumerable<NavigationBarTab> Tabs { get; set; }
    }

    public class NavigationBarElementContent : ElementContent
    {
        public IEnumerable<NavigationBarContentTab> Tabs { get; set; }
    }

    public interface INavigationBarElementService : IElementSettingsService<NavigationBarElementSettings>, IElementContentService<NavigationBarElementContent>
    {
    }

    public class NavigationBarElementService : INavigationBarElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public NavigationBarElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<NavigationBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<NavigationBarElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<NavigationBarElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId, IEnumerable<long> tagIds)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId}/content?pageid={pageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<NavigationBarElementContent>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
