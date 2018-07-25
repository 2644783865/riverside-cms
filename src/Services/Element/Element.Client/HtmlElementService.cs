using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Element.Client
{
    public class HtmlElementSettings : ElementSettings
    {
        public string Html { get; set; }
    }

    public class HtmlElementContent : IElementContent
    {
        public string FormattedHtml { get; set; }
    }

    public interface IHtmlElementService : IElementSettingsService<HtmlElementSettings>, IElementContentService<HtmlElementContent>
    {
    }

    public class HtmlElementService : IHtmlElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public HtmlElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<HtmlElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<HtmlElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<HtmlElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId}/content?pageid={pageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<HtmlElementContent>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
