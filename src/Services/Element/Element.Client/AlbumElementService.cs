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
    public class AlbumPhoto
    {
        public long BlobSetId { get; set; }
        public long ImageBlobId { get; set; }
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AlbumElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public IEnumerable<AlbumPhoto> Photos { get; set; }
    }

    public class AlbumContentPhoto
    {
        public long BlobSetId { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int PreviewWidth { get; set; }
        public int PreviewHeight { get; set; }
    }

    public class AlbumElementContent
    {
        public IDictionary<long, AlbumContentPhoto> Photos { get; set; }
    }

    public interface IAlbumElementService : IElementSettingsService<AlbumElementSettings>, IElementViewService<AlbumElementSettings, AlbumElementContent>, IElementStorageService
    {
    }

    public class AlbumElementService : IAlbumElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public AlbumElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<AlbumElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/b539d2a4-52ae-40d5-b366-e42447b93d15/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<AlbumElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<AlbumElementSettings, AlbumElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/b539d2a4-52ae-40d5-b366-e42447b93d15/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<AlbumElementSettings, AlbumElementContent>>(json);
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
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/b539d2a4-52ae-40d5-b366-e42447b93d15/elements/{elementId}/blobsets/{blobSetId}/content?bloblabel={blobLabel}";
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
