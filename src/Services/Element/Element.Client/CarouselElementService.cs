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
    public class CarouselSlide
    {
        public long BlobSetId { get; set; }
        public long ImageBlobId { get; set; }
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ButtonPageId { get; set; }
        public string ButtonText { get; set; }
    }

    public class CarouselElementSettings : ElementSettings
    {
        public IEnumerable<CarouselSlide> Slides { get; set; }
    }

    public class CarouselButton
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string ButtonText { get; set; }
    }

    public class CarouselContentSlide
    {
        public long BlobSetId { get; set; }
        public int PreviewWidth { get; set; }
        public int PreviewHeight { get; set; }
        public CarouselButton Button { get; set; }
    }

    public class CarouselElementContent
    {
        public IDictionary<long, CarouselContentSlide> Slides { get; set; }
    }

    public interface ICarouselElementService : IElementSettingsService<CarouselElementSettings>, IElementViewService<CarouselElementSettings, CarouselElementContent>, IElementStorageService
    {
    }

    public class CarouselElementService : ICarouselElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public CarouselElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<CarouselElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/aacb11a0-5532-47cb-aab9-939cee3d5175/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<CarouselElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<CarouselElementSettings, CarouselElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/aacb11a0-5532-47cb-aab9-939cee3d5175/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<CarouselElementSettings, CarouselElementContent>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long blobSetId, string blobLabel)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/aacb11a0-5532-47cb-aab9-939cee3d5175/elements/{elementId}/blobsets/{blobSetId}/content?bloblabel={blobLabel}";
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
