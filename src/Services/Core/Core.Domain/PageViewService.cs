using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageViewService : IPageViewService
    {
        private readonly IMasterPageRepository _masterPageRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IWebRepository _webRepository;

        public PageViewService(IMasterPageRepository masterPageRepository, IPageRepository pageRepository, IWebRepository webRepository)
        {
            _masterPageRepository = masterPageRepository;
            _pageRepository = pageRepository;
            _webRepository = webRepository;
        }

        private async Task<string> GetPageTitle(Web web, Page page)
        {
            // If a title is specified, then it must be used as the title
            if (page.Title != null && page.Title != string.Empty)
                return page.Title;

            // If page is home page, then use website name
            if (page.ParentPageId == null)
                return web.Name;

            // Otherwise, title is pipe separated list of page names from the current page's hierarchy (current page title first, home page title last) 
            IEnumerable<Page> pages = await _pageRepository.ListPagesInHierarchyAsync(page.TenantId, page.PageId);
            return string.Join(" | ", pages.Select(p => p.Name));
        }

        private IEnumerable<PageViewZoneElement> EnumeratePageViewZoneElements(IEnumerable<MasterPageZoneElement> masterPageZoneElements)
        {
            foreach (MasterPageZoneElement masterPageZoneElement in masterPageZoneElements)
            {
                PageViewZoneElement pageViewZoneElement = new PageViewZoneElement
                {
                    ElementTypeId = masterPageZoneElement.ElementTypeId,
                    ElementId = masterPageZoneElement.ElementId,
                    BeginRender = masterPageZoneElement.BeginRender,
                    EndRender = masterPageZoneElement.EndRender
                };
                yield return pageViewZoneElement;
            }
        }

        private IEnumerable<PageViewZoneElement> EnumeratePageViewZoneElements(IEnumerable<PageZoneElement> pageZoneElements)
        {
            foreach (PageZoneElement pageZoneElement in pageZoneElements)
            {
                PageViewZoneElement pageViewZoneElement = new PageViewZoneElement
                {
                    ElementTypeId = pageZoneElement.ElementTypeId,
                    ElementId = pageZoneElement.ElementId
                };
                yield return pageViewZoneElement;
            }
        }

        private IEnumerable<PageViewZoneElement> EnumeratePageViewZoneElements(IEnumerable<PageZoneElement> pageZoneElements, IEnumerable<MasterPageZoneElement> masterPageZoneElements)
        {
            foreach (MasterPageZoneElement masterPageZoneElement in masterPageZoneElements)
            {
                PageZoneElement pageZoneElement = pageZoneElements.Where(e => e.MasterPageZoneElementId == masterPageZoneElement.MasterPageZoneElementId).FirstOrDefault();
                if (pageZoneElement == null)
                    continue;
                PageViewZoneElement pageViewZoneElement = new PageViewZoneElement
                {
                    ElementTypeId = pageZoneElement.ElementTypeId,
                    ElementId = pageZoneElement.ElementId,
                    BeginRender = masterPageZoneElement.BeginRender,
                    EndRender = masterPageZoneElement.EndRender
                };
                yield return pageViewZoneElement;
            }
        }

        private async Task<long?> GetPageZoneId(long tenantId, long pageId, long masterPageZoneId)
        {
            IEnumerable<PageZone> pageZones = await _pageRepository.SearchPageZonesAsync(tenantId, pageId);
            PageZone pageZone = pageZones.Where(z => z.MasterPageZoneId == masterPageZoneId).FirstOrDefault();
            if (pageZone == null)
                return null;
            return pageZone.PageZoneId;
        }

        private async Task<IEnumerable<PageViewZoneElement>> SearchPageViewZoneElementsAsync(Page page, MasterPageZone masterPageZone)
        {
            switch (masterPageZone.AdminType)
            {
                case AdminType.Editable:
                    long? editablePageZoneId = await GetPageZoneId(page.TenantId, page.PageId, masterPageZone.MasterPageZoneId);
                    if (editablePageZoneId == null)
                        return Enumerable.Empty<PageViewZoneElement>();
                    IEnumerable<PageZoneElement> editablePageZoneElements = await _pageRepository.SearchPageZoneElementsAsync(page.TenantId, page.PageId, editablePageZoneId.Value);
                    IEnumerable<MasterPageZoneElement> editableMasterPageZoneElements = await _masterPageRepository.SearchMasterPageZoneElementsAsync(page.TenantId, page.MasterPageId, masterPageZone.MasterPageZoneId);
                    return EnumeratePageViewZoneElements(editablePageZoneElements, editableMasterPageZoneElements);

                case AdminType.Configurable:
                    long? pageZoneId = await GetPageZoneId(page.TenantId, page.PageId, masterPageZone.MasterPageZoneId);
                    if (pageZoneId == null)
                        return Enumerable.Empty<PageViewZoneElement>();
                    IEnumerable<PageZoneElement> pageZoneElements = await _pageRepository.SearchPageZoneElementsAsync(page.TenantId, page.PageId, pageZoneId.Value);
                    return EnumeratePageViewZoneElements(pageZoneElements);

                default: // AdminType.Static
                    IEnumerable<MasterPageZoneElement> masterPageZoneElements = await _masterPageRepository.SearchMasterPageZoneElementsAsync(page.TenantId, page.MasterPageId, masterPageZone.MasterPageZoneId);
                    return EnumeratePageViewZoneElements(masterPageZoneElements);
            }
        }

        private async Task<PageViewZone> GetPageViewZoneFromMasterPageZoneAsync(Page page, MasterPageZone masterPageZone)
        {
            PageViewZone pageViewZone = new PageViewZone
            {
                TenantId = masterPageZone.TenantId,
                MasterPageId = masterPageZone.MasterPageId,
                MasterPageZoneId = masterPageZone.MasterPageZoneId,
                PageId = page.PageId,
                BeginRender = masterPageZone.BeginRender,
                EndRender = masterPageZone.EndRender,
                PageViewZoneElements = await SearchPageViewZoneElementsAsync(page, masterPageZone)
            };
            return pageViewZone;
        }

        private IEnumerable<PageViewZone> EnumeratePageViewZones(Page page, IEnumerable<MasterPageZone> masterPageZones)
        {
            foreach (MasterPageZone masterPageZone in masterPageZones)
            {
                yield return GetPageViewZoneFromMasterPageZoneAsync(page, masterPageZone).Result;
            }
        }

        private async Task<IEnumerable<PageViewZone>> SearchPageViewZonesAsync(MasterPage masterPage, Page page)
        {
            IEnumerable<MasterPageZone> masterPageZones = await _masterPageRepository.SearchMasterPageZonesAsync(masterPage.TenantId, masterPage.MasterPageId);
            return EnumeratePageViewZones(page, masterPageZones);
        }

        public async Task<PageView> ReadPageViewAsync(long tenantId, long pageId)
        {
            Page page = await _pageRepository.ReadPageAsync(tenantId, pageId);
            if (page == null)
                return null;
            MasterPage masterPage = await _masterPageRepository.ReadMasterPageAsync(tenantId, page.MasterPageId);
            Web web = null;
            if (page.ParentPageId == null)
                web = await _webRepository.ReadWebAsync(tenantId);
            PageView pageView = new PageView
            {
                TenantId = tenantId,
                MasterPageId = masterPage.MasterPageId,
                PreviewImageBlobId = page.PreviewImageBlobId,
                PageId = pageId,
                Title = await GetPageTitle(web, page),
                Description = page.Description ?? string.Empty,
                Keywords = string.Join(", ", page.Tags.Select(t => t.Name)),
                BeginRender = masterPage.BeginRender,
                EndRender = masterPage.EndRender,
                PageViewZones = await SearchPageViewZonesAsync(masterPage, page),
                GoogleSiteVerification = web?.GoogleSiteVerification ?? string.Empty
            };
            return pageView;
        }
    }
}
