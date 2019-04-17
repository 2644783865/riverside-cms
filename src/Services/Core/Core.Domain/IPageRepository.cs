using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IPageRepository
    {
        Task<Page> ReadPageAsync(long tenantId, long pageId);
        Task UpdatePageAsync(long tenantId, long pageId, Page page);

        Task<IEnumerable<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId);
        Task<IEnumerable<Page>> ListPagesAsync(long tenantId, IEnumerable<long> pageIds);
        Task<PageListResult> ListPagesAsync(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize);
    }
}
