using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IPageService
    {
        Task<Page> ReadPageAsync(long tenantId, long pageId);
        Task<BlobContent> ReadPageImageAsync(long tenantId, long pageId, PageImageType pageImageType);

        Task<IEnumerable<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId);
        Task<PageListResult> ListPages(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize);

        Task<IEnumerable<PageZone>> SearchPageZonesAsync(long tenantId, long pageId);
        Task<PageZone> ReadPageZoneAsync(long tenantId, long pageId, long pageZoneId);

        Task<IEnumerable<PageZoneElement>> SearchPageZoneElementsAsync(long tenantId, long pageId, long pageZoneId);
        Task<PageZoneElement> ReadPageZoneElementAsync(long tenantId, long pageId, long pageZoneId, long pageZoneElementId);
    }
}
