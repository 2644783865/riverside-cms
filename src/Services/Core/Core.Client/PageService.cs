﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageService : IPageService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public PageService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        public async Task<Page> ReadPageAsync(long tenantId, long pageId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pages/{pageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<Page>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<BlobContent> ReadPageImageAsync(long tenantId, long pageId, PageImageType pageImageType)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pages/{pageId}/images/{pageImageType.ToString().ToLower()}";
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
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<List<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pages/{pageId}/hierarchy";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<List<Page>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<IEnumerable<Page>> ListPagesAsync(long tenantId, IEnumerable<long> pageIds)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pages" +
                    (pageIds != null && pageIds.Count() > 0 ? $"?pageids={string.Join(",", pageIds)}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<IEnumerable<Page>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<PageListResult> ListPagesAsync(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/pages?recursive={recursive.ToString().ToLower()}&pagetype={(int)pageType}&sortby={(int)sortBy}&sortasc={sortAsc.ToString().ToLower()}&pageindex={pageIndex}&pagesize={pageSize}" +
                    (parentPageId.HasValue ? $"&parentpageid={parentPageId.Value}" : string.Empty) +
                    (tagIds != null && tagIds.Count() > 0 ? $"&tagids={string.Join(",", tagIds)}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<PageListResult>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
