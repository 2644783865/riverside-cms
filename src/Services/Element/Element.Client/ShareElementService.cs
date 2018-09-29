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

    public class ShareElementContent
    {
        public string Title { get; set; }
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
        private readonly IOptions<ElementApiOptions> _options;

        public ShareElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<ShareElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ShareElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<ShareElementSettings, ShareElementContent>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/cf0d7834-54fb-4a6e-86db-0f238f8b1ac1/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<ShareElementSettings, ShareElementContent>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
