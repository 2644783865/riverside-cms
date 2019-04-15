using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageService : IPageService
    {
        private readonly IPageRepository _pageRepository;
        private readonly IPageValidator _pageValidator;
        private readonly IStorageService _storageService;

        private const string PageImagePath = "pages/images";

        public PageService(IPageRepository pageRepository, IPageValidator pageValidator, IStorageService storageService)
        {
            _pageRepository = pageRepository;
            _pageValidator = pageValidator;
            _storageService = storageService;
        }

        public Task<IEnumerable<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId)
        {
            return _pageRepository.ListPagesInHierarchyAsync(tenantId, pageId);
        }

        public Task<IEnumerable<Page>> ListPagesAsync(long tenantId, IEnumerable<long> pageIds)
        {
            return _pageRepository.ListPagesAsync(tenantId, pageIds);
        }

        public Task<PageListResult> ListPagesAsync(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize)
        {
            return _pageRepository.ListPagesAsync(tenantId, parentPageId, recursive, pageType, tagIds, sortBy, sortAsc, pageIndex, pageSize);
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

        public async Task UpdatePageAsync(long tenantId, long pageId, Page page)
        {
            // Override properties that are not allowed to be changed
            Page currentPage = await _pageRepository.ReadPageAsync(tenantId, pageId);
            page.ParentPageId = currentPage.ParentPageId;
            page.MasterPageId = currentPage.MasterPageId;
            page.Created = currentPage.Created;
            page.Occurred = currentPage.Occurred;
            page.ImageBlobId = currentPage.ImageBlobId;
            page.PreviewImageBlobId = currentPage.PreviewImageBlobId;
            page.ThumbnailImageBlobId = currentPage.ThumbnailImageBlobId;

            // Ensure properties supplied, which can be changed, are in the correct format
            page.Name = (page.Name ?? string.Empty).Trim();
            page.Description = (page.Description ?? string.Empty).Trim();
            page.Title = (page.Title ?? string.Empty).Trim();
            page.Updated = DateTime.UtcNow;

            // Perform validation (including checking that all or none of the image upload properties are specified)
            _pageValidator.ValidateUpdatePage(tenantId, pageId, page);

            // Do the update
            await _pageRepository.UpdatePageAsync(tenantId, pageId, page);
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
