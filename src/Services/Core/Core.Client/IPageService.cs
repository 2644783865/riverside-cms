using System.Collections.Generic;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Core.Client
{
    public interface IPageService
    {
        Task<Page> ReadPageAsync(long tenantId, long pageId);
        Task<BlobContent> ReadPageImageAsync(long tenantId, long pageId, PageImageType pageImageType);

        Task<List<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId);
        Task<IEnumerable<Page>> ListPagesAsync(long tenantId, IEnumerable<long> pageIds);
        Task<PageListResult> ListPagesAsync(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize);
    }
}
