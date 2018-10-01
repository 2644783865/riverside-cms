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
    public class ForumElementSettings : ElementSettings
    {
        public long? OwnerTenantId { get; set; }
        public long? OwnerUserId { get; set; }
        public bool OwnerOnlyThreads { get; set; }
    }

    public class ForumPager
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int PageCount { get; set; }
    }

    public class ForumElementContent
    {
        public ForumPager Pager { get; set; }
        public IEnumerable<ForumThread> Threads { get; set; }
    }

    public interface IForumElementService : IElementSettingsService<ForumElementSettings>, IElementViewService<ForumElementSettings, ForumElementContent>
    {
    }

    public class ForumElementService : IForumElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public ForumElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<ForumElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/484192d1-5a4f-496f-981b-7e0120453949/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ForumElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<ForumElementSettings, ForumElementContent>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/484192d1-5a4f-496f-981b-7e0120453949/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<ForumElementSettings, ForumElementContent>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
