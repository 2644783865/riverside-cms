using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Riverside.Cms.Utilities.Net.RestSharpExtensions;

namespace Riverside.Cms.Services.Element.Client
{
    public class NavigationBarTab
    {
        public long PageId { get; set; }
        public long TabId { get; set; }
        public string Name { get; set; }
    }

    public class NavigationBarContentTab
    {
        public bool Active { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string PageName { get; set; }
    }

    public class NavigationBarElementSettings : ElementSettings
    {
        public string Brand { get; set; }
        public bool ShowLoggedOnUserOptions { get; set; }
        public bool ShowLoggedOffUserOptions { get; set; }
        public IEnumerable<NavigationBarTab> Tabs { get; set; }
    }

    public class NavigationBarElementContent : IElementContent
    {
        public IEnumerable<NavigationBarContentTab> Tabs { get; set; }
    }

    public interface INavigationBarElementService : IElementSettingsService<NavigationBarElementSettings>, IElementContentService<NavigationBarElementContent>
    {
    }

    public class NavigationBarElementService : INavigationBarElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public NavigationBarElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        private void CheckResponseStatus<T>(IRestResponse<T> response) where T : new()
        {
            if (response.ErrorException != null)
                throw new ElementClientException($"Element API failed with response status {response.ResponseStatus}", response.ErrorException);
        }

        public async Task<NavigationBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.ElementApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId}", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("elementId", elementId);
                IRestResponse<NavigationBarElementSettings> response = await client.ExecuteAsync<NavigationBarElementSettings>(request);
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

        public async Task<NavigationBarElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            try
            {
                RestClient client = new RestClient(_options.Value.ElementApiBaseUrl);
                RestRequest request = new RestRequest("tenants/{tenantId}/elementtypes/a94c34c0-1a4c-4c91-a669-2f830cf1ea5f/elements/{elementId}/content", Method.GET);
                request.AddUrlSegment("tenantId", tenantId);
                request.AddUrlSegment("elementId", elementId);
                request.AddQueryParameter("pageId", pageId.ToString());
                IRestResponse<NavigationBarElementContent> response = await client.ExecuteAsync<NavigationBarElementContent>(request);
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
