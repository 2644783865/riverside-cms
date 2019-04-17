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

        private IEnumerable<PageZoneElement> GetPageZoneElements(Page page, long masterPageZoneId)
        {
            PageZone pageZone = page.PageZones.Where(z => z.MasterPageZoneId == masterPageZoneId).FirstOrDefault();
            if (pageZone == null)
                return Enumerable.Empty<PageZoneElement>();
            else
                return pageZone.PageZoneElements;
        }

        private IEnumerable<MasterPageZoneElement> GetMasterPageZoneElements(MasterPage masterPage, long masterPageZoneId)
        {
            MasterPageZone masterPageZone = masterPage.MasterPageZones.Where(z => z.MasterPageZoneId == masterPageZoneId).FirstOrDefault();
            if (masterPageZone == null)
                return Enumerable.Empty<MasterPageZoneElement>();
            else
                return masterPageZone.MasterPageZoneElements;
        }

        private IEnumerable<PageViewZoneElement> SearchPageViewZoneElements(MasterPage masterPage, Page page, MasterPageZone masterPageZone)
        {
            switch (masterPageZone.AdminType)
            {
                case AdminType.Editable:
                    IEnumerable<PageZoneElement> editablePageZoneElements = GetPageZoneElements(page, masterPageZone.MasterPageZoneId);
                    IEnumerable<MasterPageZoneElement> editableMasterPageZoneElements = GetMasterPageZoneElements(masterPage, masterPageZone.MasterPageZoneId);
                    return EnumeratePageViewZoneElements(editablePageZoneElements, editableMasterPageZoneElements);

                case AdminType.Configurable:
                    IEnumerable<PageZoneElement> pageZoneElements = GetPageZoneElements(page, masterPageZone.MasterPageZoneId);
                    return EnumeratePageViewZoneElements(pageZoneElements);

                default: // AdminType.Static
                    IEnumerable<MasterPageZoneElement> masterPageZoneElements = GetMasterPageZoneElements(masterPage, masterPageZone.MasterPageZoneId);
                    return EnumeratePageViewZoneElements(masterPageZoneElements);
            }
        }

        private IEnumerable<PageViewZone> EnumeratePageViewZones(MasterPage masterPage, Page page)
        {
            foreach (MasterPageZone masterPageZone in masterPage.MasterPageZones)
            {
                PageViewZone pageViewZone = new PageViewZone
                {
                    MasterPageZoneId = masterPageZone.MasterPageZoneId,
                    BeginRender = masterPageZone.BeginRender,
                    EndRender = masterPageZone.EndRender,
                    PageViewZoneElements = SearchPageViewZoneElements(masterPage, page, masterPageZone)
                };
                yield return pageViewZone;
            }
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
                PageViewZones = EnumeratePageViewZones(masterPage, page),
                GoogleSiteVerification = web?.GoogleSiteVerification ?? string.Empty
            };
            return pageView;
        }
    }
}
