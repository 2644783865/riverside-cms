using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Riverside.Cms.Utilities.Net.RestSharpExtensions;

namespace Riverside.Cms.Services.Storage.Client
{
    public class StorageService : IStorageService
    {
        private readonly IOptions<StorageApiOptions> _options;

        public StorageService(IOptions<StorageApiOptions> options)
        {
            _options = options;
        }

        private void CheckResponseStatus<T>(IRestResponse<T> response) where T : new()
        {
            if (response.ErrorException != null)
                throw new StorageClientException($"Storage API failed with response status {response.ResponseStatus}", response.ErrorException);
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
                RestClient client = new RestClient(_options.Value.StorageApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/blobs/{blobId}", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("blobId", blobId);
                IRestResponse<BlobImage> response = await client.ExecuteAsync<BlobImage>(request);
                CheckResponseStatus(response);
                BlobImage blobImage = response.Data;
                if (blobImage.Width == 0 && blobImage.Height == 0)
                    return GetBlob(blobImage);
                return blobImage;
            }
            catch (StorageClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new StorageClientException("Storage API failed", ex);
            }
        }
    }
}
