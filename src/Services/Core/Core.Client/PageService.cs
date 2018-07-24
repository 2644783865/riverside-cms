﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Riverside.Cms.Services.Storage.Client;
using Riverside.Cms.Utilities.Net.RestSharpExtensions;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageService : IPageService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public PageService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        private void CheckResponseStatus<T>(IRestResponse<T> response) where T : new()
        {
            if (response.ErrorException != null)
                throw new CoreClientException($"Core API failed with response status {response.ResponseStatus}", response.ErrorException);
        }

        public async Task<Page> ReadPageAsync(long tenantId, long pageId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.CoreApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/pages/{pageId}", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("pageId", pageId);
                IRestResponse<Page> response = await client.ExecuteAsync<Page>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (CoreClientException)
            {
                throw;
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
                RestClient client = new RestClient(_options.Value.CoreApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/pages/{pageId}/hierarchy", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("pageId", pageId);
                IRestResponse<List<Page>> response = await client.ExecuteAsync<List<Page>>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (CoreClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<List<PageZone>> SearchPageZonesAsync(long tenantId, long pageId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.CoreApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/pages/{pageId}/zones", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("pageId", pageId);
                IRestResponse<List<PageZone>> response = await client.ExecuteAsync<List<PageZone>>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (CoreClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<PageZone> ReadPageZoneAsync(long tenantId, long pageId, long pageZoneId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.CoreApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/pages/{pageId}/zones/{pageZoneId}", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("pageId", pageId);
                request.AddUrlSegment("pageZoneId", pageZoneId);
                IRestResponse<PageZone> response = await client.ExecuteAsync<PageZone>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (CoreClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<List<PageZoneElement>> SearchPageZoneElementsAsync(long tenantId, long pageId, long pageZoneId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.CoreApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/pages/{pageId}/zones/{pageZoneId}/elements", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("pageId", pageId);
                request.AddUrlSegment("pageZoneId", pageZoneId);
                IRestResponse<List<PageZoneElement>> response = await client.ExecuteAsync<List<PageZoneElement>>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (CoreClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<PageZoneElement> ReadPageZoneElementAsync(long tenantId, long pageId, long pageZoneId, long pageZoneElementId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.CoreApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/pages/{pageId}/zones/{pageZoneId}/elements/{pageZoneElementId}", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("pageId", pageId);
                request.AddUrlSegment("pageZoneId", pageZoneId);
                request.AddUrlSegment("pageZoneElementId", pageZoneElementId);
                IRestResponse<PageZoneElement> response = await client.ExecuteAsync<PageZoneElement>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (CoreClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
