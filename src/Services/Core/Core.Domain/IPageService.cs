using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IPageService
    {
        Task<IEnumerable<long>> CreatePageImagesAsync(long tenantId, long pageId, IBlobContent content);

        Task<Page> ReadPageAsync(long tenantId, long pageId);
        Task<BlobContent> ReadPageImageAsync(long tenantId, long pageId, PageImageType pageImageType);
        Task UpdatePageAsync(long tenantId, long pageId, Page page);

        Task<IEnumerable<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId);
        Task<IEnumerable<Page>> ListPagesAsync(long tenantId, IEnumerable<long> pageIds);
        Task<PageListResult> ListPagesAsync(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize);
    }
}
