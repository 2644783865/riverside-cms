using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Storage.Client
{
    public class StorageService : IStorageService
    {
        private readonly IOptions<StorageApiOptions> _options;

        public StorageService(IOptions<StorageApiOptions> options)
        {
            _options = options;
        }

        private Blob GetBlob(BlobImage blobImage)
        {
            return new Blob
            {
                BlobId = blobImage.BlobId,
                ContentType = blobImage.ContentType,
                Created = blobImage.Created,
                Name = blobImage.Name,
                Path = blobImage.Path,
                Size = blobImage.Size,
                TenantId = blobImage.TenantId,
                Updated = blobImage.Updated
            };
        }

        public async Task<Blob> ReadBlobAsync(long tenantId, long blobId)
        {
            try
            {
                string uri = $"{_options.Value.StorageApiBaseUrl}tenants/{tenantId}/blobs/{blobId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    BlobImage blobImage = JsonConvert.DeserializeObject<BlobImage>(json);
                    if (blobImage.Width == 0 && blobImage.Height == 0)
                        return GetBlob(blobImage);
                    return blobImage;
                }
            }
            catch (Exception ex)
            {
                throw new StorageClientException("Storage API failed", ex);
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long blobId)
        {
            try
            {
                string uri = $"{_options.Value.StorageApiBaseUrl}tenants/{tenantId}/blobs/{blobId}/content";
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(uri);
                    return new BlobContent
                    {
                        Type = response.Content.Headers.ContentType.MediaType,
                        Stream = await response.Content.ReadAsStreamAsync()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new StorageClientException("Storage API failed", ex);
            }
        }
    }
}
