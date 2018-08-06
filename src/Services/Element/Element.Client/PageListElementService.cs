using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Client
{
    public class PageListElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public string DisplayName { get; set; }
        public SortBy SortBy { get; set; }
        public bool SortAsc { get; set; }
        public bool ShowRelated { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowBackgroundImage { get; set; }
        public bool ShowCreated { get; set; }
        public bool ShowUpdated { get; set; }
        public bool ShowOccurred { get; set; }
        public bool ShowComments { get; set; }
        public bool ShowTags { get; set; }
        public bool ShowPager { get; set; }
        public string MoreMessage { get; set; }
        public bool Recursive { get; set; }
        public PageType PageType { get; set; }
        public int PageSize { get; set; }
        public string NoPagesMessage { get; set; }
        public string Preamble { get; set; }
    }

    public class PageListImage
    {
        public long BlobId { get; set; }
        public long PageId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class PageListPage
    {
        public long PageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Home { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Occurred { get; set; }
        public bool OccursInFuture { get; set; }
        public PageListImage BackgroundImage { get; set; }
        public PageListImage Image { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class PageListPageLink
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
    }

    public class PageListPager
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int PageCount { get; set; }
    }

    public class PageListElementContent : ElementContent
    {
        public PageListPageLink CurrentPage { get; set; }
        public PageListPageLink MorePage { get; set; }
        public IEnumerable<PageListPage> Pages { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public string DisplayName { get; set; }
        public string MoreMessage { get; set; }
        public string NoPagesMessage { get; set; }
        public PageListPager Pager { get; set; }
    }

    public interface IPageListElementService : IElementSettingsService<PageListElementSettings>, IElementContentService<PageListElementContent>
    {
    }

    public class PageListElementService : IPageListElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public PageListElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<PageListElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/61f55535-9f3e-4ef5-96a2-bc84d648842a/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<PageListElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<PageListElementContent> ReadElementContentAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/61f55535-9f3e-4ef5-96a2-bc84d648842a/elements/{elementId}/content?pageid={context.PageId}" +
                    (context.TagIds != null && context.TagIds.Count() > 0 ? $"&tagids={string.Join(",", context.TagIds)}" : string.Empty) +
                    (context.Parameters.ContainsKey("page") ? $"&page={context.Parameters["page"]}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<PageListElementContent>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
