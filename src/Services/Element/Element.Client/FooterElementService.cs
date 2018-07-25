using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Element.Client
{
    public class FooterElementSettings : ElementSettings
    {
        public bool ShowLoggedOnUserOptions { get; set; }
        public bool ShowLoggedOffUserOptions { get; set; }
        public string Message { get; set; }
    }

    public class FooterElementContent : IElementContent
    {
        public string FormattedMessage { get; set; }
    }

    public interface IFooterElementService : IElementSettingsService<FooterElementSettings>, IElementContentService<FooterElementContent>
    {
    }

    public class FooterElementService : IFooterElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public FooterElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<FooterElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<FooterElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<FooterElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId}/content?pageid={pageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<FooterElementContent>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
