using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Element.Domain
{
    public class NavigationBarTab
    {
        public long PageId { get; set; }
        public long TabId { get; set; }
        public string Name { get; set; }
        public IEnumerable<NavigationBarTab> Tabs { get; set; }
    }

    public class NavigationBarContentTab
    {
        public bool Active { get; set; }
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string PageName { get; set; }
        public IEnumerable<NavigationBarContentTab> Tabs { get; set; }
    }

    public class NavigationBarElementSettings : ElementSettings
    {
        public string Brand { get; set; }
        public bool ShowLoggedOnUserOptions { get; set; }
        public bool ShowLoggedOffUserOptions { get; set; }
        public IEnumerable<NavigationBarTab> Tabs { get; set; }
    }

    public class NavigationBarElementContent
    {
        public IEnumerable<NavigationBarContentTab> Tabs { get; set; }
    }

    public interface INavigationBarElementService : IElementSettingsService<NavigationBarElementSettings>, IElementViewService<NavigationBarElementSettings, NavigationBarElementContent>
    {
    }

    public class NavigationBarElementService : INavigationBarElementService
    {
        private readonly IElementRepository<NavigationBarElementSettings> _elementRepository;
        private readonly IPageService _pageService;

        public NavigationBarElementService(IElementRepository<NavigationBarElementSettings> elementRepository, IPageService pageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
        }

        public Task<NavigationBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private bool TabIsActive(Page tabPage, IEnumerable<Page> currentPageHierarchy)
        {
            foreach (Page page in currentPageHierarchy)
            {
                if (page.PageId == tabPage.PageId)
                    return true;
            }
            return false;
        }

        private IEnumerable<NavigationBarContentTab> GetContentTabs(IDictionary<long, Page> pagesById, IEnumerable<Page> currentPageHierarchy, IEnumerable<NavigationBarTab> tabs)
        {
            foreach (NavigationBarTab tab in tabs)
            {
                if (pagesById.ContainsKey(tab.PageId))
                {
                    Page tabPage = pagesById[tab.PageId];
                    bool home = tabPage.ParentPageId == null;
                    bool active = !home && TabIsActive(tabPage, currentPageHierarchy);
                    NavigationBarContentTab contentTab = new NavigationBarContentTab
                    {
                        Active = active,
                        Name = tab.Name == string.Empty ? tabPage.Name : tab.Name,
                        PageId = tab.PageId,
                        PageName = tabPage.Name,
                        Home = home
                    };
                    contentTab.Tabs = GetContentTabs(pagesById, currentPageHierarchy, tab.Tabs);
                    yield return contentTab;
                }
            }
        }

        private async Task<IEnumerable<NavigationBarContentTab>> GetContentTabs(NavigationBarElementSettings settings, long pageId)
        {
            // Get details of pages that are referenced by navigation tabs
            IEnumerable<long> pageIds = Enumerable.Concat(settings.Tabs.Select(t => t.PageId), settings.Tabs.SelectMany(t => t.Tabs).Select(t => t.PageId)).Distinct();
            IEnumerable<Page> pages = await _pageService.ListPagesAsync(settings.TenantId, pageIds);
            IDictionary<long, Page> pagesById = pages.ToDictionary(p => p.PageId, p => p);

            // Get the current page hierarchy
            IEnumerable<Page> currentPageHierarchy = await _pageService.ListPagesInHierarchyAsync(settings.TenantId, pageId);

            // Get content tabs
            return GetContentTabs(pagesById, currentPageHierarchy, settings.Tabs);
        }

        public async Task<IElementView<NavigationBarElementSettings, NavigationBarElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            NavigationBarElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            NavigationBarElementContent content = new NavigationBarElementContent
            {
                Tabs = await GetContentTabs(settings, context.PageId)
            };

            return new ElementView<NavigationBarElementSettings, NavigationBarElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
