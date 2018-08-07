using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public async Task<Blob> ReadBlobAsync(long tenantId, long blobId)
        {
            try
            {
                string uri = $"{_options.Value.StorageApiBaseUrl}tenants/{tenantId}/blobs/{blobId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    BlobTypeModel blobTypeModel = JsonConvert.DeserializeObject<BlobTypeModel>(json);
                    switch (blobTypeModel.BlobType)
                    {
                        case BlobType.Image:
                            return JsonConvert.DeserializeObject<BlobImage>(json);

                        default:
                            return JsonConvert.DeserializeObject<Blob>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new StorageClientException("Storage API failed", ex);
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long blobId, string path)
        {
            try
            {
                string uri = $"{_options.Value.StorageApiBaseUrl}tenants/{tenantId}/blobs/{blobId}/content?path={WebUtility.UrlEncode(path)}";
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
                throw new StorageClientException("Storage API failed", ex);
            }
        }

        private Blob GetBlobFromModel(BlobModel model)
        {
            Blob blob = null;
            if (model == null)
                return blob;
            if (model.BlobType == BlobType.Image)
                blob = new BlobImage { Width = model.Width.Value, Height = model.Height.Value, BlobType = BlobType.Image };
            else
                blob = new Blob { BlobType = BlobType.Document };
            blob.TenantId = model.TenantId;
            blob.BlobId = model.BlobId;
            blob.Size = model.Size;
            blob.ContentType = model.ContentType;
            blob.Path = model.Path;
            blob.Name = model.Name;
            blob.Created = model.Created;
            blob.Updated = model.Updated;
            return blob;
        }

        public async Task<IEnumerable<Blob>> ListBlobsAsync(long tenantId, IEnumerable<long> blobIds)
        {
            try
            {
                string uri = $"{_options.Value.StorageApiBaseUrl}tenants/{tenantId}/blobs/" +
                    (blobIds != null && blobIds.Count() > 0 ? $"?blobids={string.Join(",", blobIds)}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    IEnumerable<BlobModel> blobModels = JsonConvert.DeserializeObject<IEnumerable<BlobModel>>(json);
                    return blobModels.Select(b => GetBlobFromModel(b));
                }
            }
            catch (Exception ex)
            {
                throw new StorageClientException("Storage API failed", ex);
            }
        }
    }
}
