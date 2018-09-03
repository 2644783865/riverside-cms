using System;
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
    public class HtmlBlob
    {
        public long HtmlBlobId { get; set; }
        public long ImageBlobId { get; set; }
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
    }

    public class HtmlElementSettings : ElementSettings
    {
        public string Html { get; set; }
        public IEnumerable<HtmlBlob> Blobs { get; set; }
    }

    public class HtmlContentImage
    {
        public long BlobId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
    }

    public class HtmlElementContent
    {
        public string FormattedHtml { get; set; }
        public IDictionary<long, HtmlContentImage> Images { get; set; }
    }

    public interface IHtmlElementService : IElementSettingsService<HtmlElementSettings>, IElementViewService<HtmlElementSettings, HtmlElementContent>, IElementStorageService
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

        public async Task<IElementView<HtmlElementSettings, HtmlElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<HtmlElementSettings, HtmlElementContent>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long elementBlobId, PageImageType imageType)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/{elementId}/blobs/{elementBlobId}/content?imagetype={imageType.ToString().ToLower()}";
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(uri);
                    return new BlobContent
                    {
                        Name = response.Content.Headers.ContentDisposition.FileName,
                        Type = response.Content.Headers.ContentType.MediaType,
                        Stream = await response.Content.ReadAsStreamAsync()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
