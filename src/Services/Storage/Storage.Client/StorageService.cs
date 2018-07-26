﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
    }
}
