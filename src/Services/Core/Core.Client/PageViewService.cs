using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageViewService : IPageViewService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public PageViewService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        public async Task<PageView> ReadPageViewAsync(long tenantId, long pageId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pageviews/{pageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<PageView>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<List<PageViewZone>> SearchPageViewZonesAsync(long tenantId, long pageId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pageviews/{pageId}/zones";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<List<PageViewZone>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<PageViewZone> ReadPageViewZoneAsync(long tenantId, long pageId, long masterPageZoneId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pageviews/{pageId}/zones/{masterPageZoneId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<PageViewZone>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<List<PageViewZoneElement>> SearchPageViewZoneElementsAsync(long tenantId, long pageId, long masterPageZoneId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pageviews/{pageId}/zones/{masterPageZoneId}/elements";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<List<PageViewZoneElement>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
