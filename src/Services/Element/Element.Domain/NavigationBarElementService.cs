using System.Collections.Generic;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
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
        public bool Home { get; set; }
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

        private async Task<Page> GetCurrentPage(long tenantId, long pageId)
        {
            Page currentPage = await _pageService.ReadPageAsync(tenantId, pageId);
            Page hierarchyPage = currentPage;
            while (hierarchyPage != null && hierarchyPage.ParentPageId != null)
            {
                hierarchyPage.ParentPage = await _pageService.ReadPageAsync(tenantId, hierarchyPage.ParentPageId.Value);
                hierarchyPage = hierarchyPage.ParentPage;
            }
            return currentPage;
        }

        private bool TabIsActive(Page tabPage, Page currentPage)
        {
            bool active = false;
            while (!active && currentPage != null)
            {
                active = currentPage.PageId == tabPage.PageId;
                currentPage = currentPage.ParentPage;
            }
            return active;
        }

        private async Task<List<NavigationBarContentTab>> GetContentTabs(NavigationBarElementSettings elementSettings, long pageId)
        {
            List<NavigationBarContentTab> tabs = new List<NavigationBarContentTab>();
            Page currentPage = null;
            foreach (NavigationBarTab tab in elementSettings.Tabs)
            {
                Page tabPage = await _pageService.ReadPageAsync(elementSettings.TenantId, tab.PageId);
                if (tabPage != null)
                {
                    if (currentPage == null)
                        currentPage = await GetCurrentPage(elementSettings.TenantId, pageId);
                    bool home = tabPage.ParentPageId == null;
                    bool active = !home && TabIsActive(tabPage, currentPage);
                    tabs.Add(new NavigationBarContentTab
                    {
                        Active = active,
                        Name = tab.Name == string.Empty ? tabPage.Name : tab.Name,
                        PageId = tab.PageId,
                        PageName = tabPage.Name,
                        Home = home
                    });
                }
            }
            return tabs;
        }

        public async Task<NavigationBarElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            NavigationBarElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            return new NavigationBarElementContent
            {
                Tabs = await GetContentTabs(elementSettings, pageId)
            };
        }
    }
}
