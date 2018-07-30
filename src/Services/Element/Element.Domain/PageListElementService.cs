using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
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

    public class PageListPage
    {
    }

    public class PageListElementContent : ElementContent
    {
        public IEnumerable<PageListPage> Pages { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }

    public interface IPageListElementService : IElementSettingsService<PageListElementSettings>, IElementContentService<PageListElementContent>
    {
    }

    public class PageListElementService : IPageListElementService
    {
        private readonly IElementRepository<PageListElementSettings> _elementRepository;
        private readonly IPageService _pageService;

        public PageListElementService(IElementRepository<PageListElementSettings> elementRepository, IPageService pageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
        }

        public Task<PageListElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<PageListElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            PageListElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            PageListElementContent elementContent = new PageListElementContent
            {
                TenantId = elementSettings.TenantId,
                ElementId = elementSettings.ElementId,
                ElementTypeId = elementSettings.ElementTypeId,
            };

            return elementContent;
        }
    }
}
