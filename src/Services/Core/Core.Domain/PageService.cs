using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageService : IPageService
    {
        private readonly IPageRepository _pageRepository;
        private readonly IStorageService _storageService;

        private const string PageImagePath = "pages/images";

        public PageService(IPageRepository pageRepository, IStorageService storageService)
        {
            _pageRepository = pageRepository;
            _storageService = storageService;
        }

        public async Task<IEnumerable<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId)
        {
            return await _pageRepository.ListPagesInHierarchyAsync(tenantId, pageId);
        }

        public async Task<PageListResult> ListPagesAsync(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize)
        {
            return await _pageRepository.ListPagesAsync(tenantId, parentPageId, recursive, pageType, tagIds, sortBy, sortAsc, pageIndex, pageSize);
        }

        public Task<Page> ReadPageAsync(long tenantId, long pageId)
        {
            return _pageRepository.ReadPageAsync(tenantId, pageId);
        }

        private long? GetBlobId(Page page, PageImageType pageImageType)
        {
            switch (pageImageType)
            {
                case PageImageType.Original:
                    return page.ImageBlobId;

                case PageImageType.Preview:
                    return page.PreviewImageBlobId;

                case PageImageType.Thumbnail:
                    return page.ThumbnailImageBlobId;

                default:
                    return null;
            }
        }

        public async Task<BlobContent> ReadPageImageAsync(long tenantId, long pageId, PageImageType pageImageType)
        {
            Page page = await _pageRepository.ReadPageAsync(tenantId, pageId);
            if (page == null)
                return null;
            long? blobId = GetBlobId(page, pageImageType);
            if (blobId == null)
                return null;
            return await _storageService.ReadBlobContentAsync(tenantId, blobId.Value, PageImagePath);
        }

        public Task<IEnumerable<PageZone>> SearchPageZonesAsync(long tenantId, long pageId)
        {
            return _pageRepository.SearchPageZonesAsync(tenantId, pageId);
        }

        public Task<PageZone> ReadPageZoneAsync(long tenantId, long pageId, long pageZoneId)
        {
            return _pageRepository.ReadPageZoneAsync(tenantId, pageId, pageZoneId);
        }

        public Task<IEnumerable<PageZoneElement>> SearchPageZoneElementsAsync(long tenantId, long pageId, long pageZoneId)
        {
            return _pageRepository.SearchPageZoneElementsAsync(tenantId, pageId, pageZoneId);
        }

        public Task<PageZoneElement> ReadPageZoneElementAsync(long tenantId, long pageId, long pageZoneId, long pageZoneElementId)
        {
            return _pageRepository.ReadPageZoneElementAsync(tenantId, pageId, pageZoneId, pageZoneElementId);
        }
    }
}
