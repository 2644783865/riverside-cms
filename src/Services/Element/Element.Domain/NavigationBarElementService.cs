using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Element.Domain
{
    public class NavigationBarTab
    {
        public long TenantId { get; set; }
        public long ElementId { get; set; }
        public long NavigationBarTabId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public long PageId { get; set; }
    }

    public class NavigationBarElementSettings : ElementSettings
    {
        public string NavBarName { get; set; }
        public bool ShowLoggedOnUserOptions { get; set; }
        public bool ShowLoggedOffUserOptions { get; set; }
        public IEnumerable<NavigationBarTab> Tabs { get; set; }
    }

    public class NavigationBarElementContent : IElementContent
    {
        public long? ActiveNavigationBarTabId { get; set; }
    }

    public interface INavigationBarElementService : IElementSettingsService<NavigationBarElementSettings>, IElementContentService<NavigationBarElementContent>
    {
    }

    public class NavigationBarElementService : INavigationBarElementService
    {
        private readonly IElementRepository<NavigationBarElementSettings> _elementRepository;

        public NavigationBarElementService(IElementRepository<NavigationBarElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<NavigationBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<NavigationBarElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            NavigationBarElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            return new NavigationBarElementContent
            {
                ActiveNavigationBarTabId = null
            };
        }
    }
}
