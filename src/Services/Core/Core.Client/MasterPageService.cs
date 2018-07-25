using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Core.Client
{
    public class MasterPageService : IMasterPageService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public MasterPageService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        public async Task<MasterPage> ReadMasterPageAsync(long tenantId, long masterPageId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/masterpages/{masterPageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<MasterPage>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<List<MasterPageZone>> SearchMasterPageZonesAsync(long tenantId, long masterPageId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/masterpages/{masterPageId}/zones";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<List<MasterPageZone>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<MasterPageZone> ReadMasterPageZoneAsync(long tenantId, long masterPageId, long masterPageZoneId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/masterpages/{masterPageId}/zones/{masterPageZoneId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<MasterPageZone>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<List<MasterPageZoneElement>> SearchMasterPageZoneElementsAsync(long tenantId, long masterPageId, long masterPageZoneId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/masterpages/{masterPageId}/zones/{masterPageZoneId}/elements";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<List<MasterPageZoneElement>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<MasterPageZoneElement> ReadMasterPageZoneElementAsync(long tenantId, long masterPageId, long masterPageZoneId, long masterPageZoneElementId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/masterpages/{masterPageId}/zones/{masterPageZoneId}/elements/{masterPageZoneElementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<MasterPageZoneElement>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
