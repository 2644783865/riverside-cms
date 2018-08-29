using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Client
{
    public class SocialBarElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public string Preamble { get; set; }
        public string TwitterUsername { get; set; }
        public string FacebookUsername { get; set; }
        public string LinkedInCompanyUsername { get; set; }
        public string LinkedInPersonalUsername { get; set; }
        public string InstagramUsername { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string TelephoneNumber1 { get; set; }
        public string TelephoneNumber2 { get; set; }
        public string YouTubeChannelId { get; set; }
    }

    public class SocialBarElementContent
    {
        public string TwitterUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string LinkedInPersonalUrl { get; set; }
        public string LinkedInCompanyUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string YouTubeChannelUrl { get; set; }
    }

    public interface ISocialBarElementService : IElementSettingsService<SocialBarElementSettings>, IElementViewService<SocialBarElementSettings, SocialBarElementContent>
    {
    }

    public class SocialBarElementService : ISocialBarElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public SocialBarElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<SocialBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/4e6b936d-e8a1-4ff2-9576-9f9b78f82895/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<SocialBarElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<SocialBarElementSettings, SocialBarElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/4e6b936d-e8a1-4ff2-9576-9f9b78f82895/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<SocialBarElementSettings, SocialBarElementContent>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
