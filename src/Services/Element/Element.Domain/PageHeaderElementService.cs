using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class PageHeaderElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public bool ShowName { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowCreated { get; set; }
        public bool ShowUpdated { get; set; }
        public bool ShowOccurred { get; set; }
        public bool ShowBreadcrumbs { get; set; }
    }

    public class PageHeaderBreadcrumb
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string PageName { get; set; }
    }

    public class PageHeaderElementContent : IElementContent
    {
        public Page Page { get; set; }
        public IEnumerable<PageHeaderBreadcrumb> Breadcrumbs { get; set; }
    }

    public interface IPageHeaderElementService : IElementSettingsService<PageHeaderElementSettings>, IElementContentService<PageHeaderElementContent>
    {
    }

    public class PageHeaderElementService : IPageHeaderElementService
    {
        private readonly IElementRepository<PageHeaderElementSettings> _elementRepository;
        private readonly IPageService _pageService;

        public PageHeaderElementService(IElementRepository<PageHeaderElementSettings> elementRepository, IPageService pageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
        }

        public Task<PageHeaderElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<PageHeaderElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            PageHeaderElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            Page page = await _pageService.ReadPageAsync(tenantId, pageId);

            IEnumerable<PageHeaderBreadcrumb> breadcrumbs = null;
            if (elementSettings.ShowBreadcrumbs)
            {
                IEnumerable<Page> pages = await _pageService.ListPagesInHierarchyAsync(tenantId, pageId);
                breadcrumbs = pages.Reverse().Select(p => new PageHeaderBreadcrumb
                {
                    Home = p.ParentPageId == null,
                    Name = p.ParentPageId == null ? "Home" : p.Name,
                    PageId = p.PageId,
                    PageName = p.Name
                });
            }

            return new PageHeaderElementContent
            {
                Page = page,
                Breadcrumbs = breadcrumbs
            };
        }
    }
}
