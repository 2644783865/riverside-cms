using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Client
{
    public class LatestThreadsElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public string DisplayName { get; set; }
        public string NoThreadsMessage { get; set; }
        public string Preamble { get; set; }
        public int PageSize { get; set; }
        public bool Recursive { get; set; }
    }

    public class LatestThreadsImage
    {
        public long BlobId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class LatestThreadsUser
    {
        public long UserId { get; set; }
        public string Alias { get; set; }
        public LatestThreadsImage Image { get; set; }
    }

    public class LatestThreadsThread
    {
        public long PageId { get; set; }
        public long TenantId { get; set; }
        public long ForumId { get; set; }
        public long ThreadId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool Notify { get; set; }
        public int Views { get; set; }
        public int Replies { get; set; }
        public long? LastPostId { get; set; }
        public DateTime LastPostCreated { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public LatestThreadsUser CreatedByUser { get; set; }
        public LatestThreadsUser LastPostByUser { get; set; }
    }

    public class LatestThreadsElementContent
    {
        public IEnumerable<LatestThreadsThread> Threads { get; set; }
    }

    public interface ILatestThreadsElementService : IElementSettingsService<LatestThreadsElementSettings>, IElementViewService<LatestThreadsElementSettings, LatestThreadsElementContent>
    {
    }

    public class LatestThreadsElementService : ILatestThreadsElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public LatestThreadsElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<LatestThreadsElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/f9557287-ba01-48e3-9ab4-e2f4831933d0/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<LatestThreadsElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<LatestThreadsElementSettings, LatestThreadsElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/f9557287-ba01-48e3-9ab4-e2f4831933d0/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<LatestThreadsElementSettings, LatestThreadsElementContent>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
