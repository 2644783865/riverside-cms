﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Riverside.Cms.Utilities.RestSharpExtensions;

namespace Riverside.Cms.Services.Element.Client
{
    public class FooterElementSettings : ElementSettings
    {
        public bool ShowLoggedOnUserOptions { get; set; }
        public bool ShowLoggedOffUserOptions { get; set; }
        public string Message { get; set; }
    }

    public class FooterElementView
    {
        public FooterElementSettings Settings { get; set; }
    }

    public interface IFooterElementService
    {
        Task<FooterElementSettings> ReadElementAsync(long tenantId, long elementId);
        Task<FooterElementView> GetElementViewAsync(long tenantId, long elementId, long pageId);
    }

    public class FooterElementService : IFooterElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public FooterElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        private void CheckResponseStatus<T>(IRestResponse<T> response) where T : new()
        {
            if (response.ErrorException != null)
                throw new ElementClientException($"Element API failed with response status {response.ResponseStatus}", response.ErrorException);
        }

        public async Task<FooterElementSettings> ReadElementAsync(long tenantId, long elementId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.ElementApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId}", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("elementId", elementId);
                IRestResponse<FooterElementSettings> response = await client.ExecuteAsync<FooterElementSettings>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (ElementClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<FooterElementView> GetElementViewAsync(long tenantId, long elementId, long pageId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.ElementApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/elementtypes/f1c2b384-4909-47c8-ada7-cd3cc7f32620/elements/{elementId}/view", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("elementId", elementId);
                request.AddQueryParameter("pageId", pageId.ToString());
                IRestResponse<FooterElementView> response = await client.ExecuteAsync<FooterElementView>(request);
                CheckResponseStatus(response);
                return response.Data;
            }
            catch (ElementClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
