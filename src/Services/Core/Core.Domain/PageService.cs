using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Utilities.Drawing.ImageAnalysis;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageService : IPageService
    {
        private readonly IImageAnalysisService _imageAnalysisService;
        private readonly IMasterPageRepository _masterPageRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IPageValidator _pageValidator;
        private readonly IStorageService _storageService;

        private const string PageImagePath = "pages/images";

        public PageService(IImageAnalysisService imageAnalysisService, IMasterPageRepository masterPageRepository, IPageRepository pageRepository, IPageValidator pageValidator, IStorageService storageService)
        {
            _imageAnalysisService = imageAnalysisService;
            _masterPageRepository = masterPageRepository;
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

        public async Task<IEnumerable<long>> CreatePageImagesAsync(long tenantId, long pageId, IBlobContent content)
        {
            // Validate the request to create page images
            await _pageValidator.ValidateCreatePageImagesAsync(tenantId, pageId, content);

            // Create thumbnail and preview images
            Page page = await _pageRepository.ReadPageAsync(tenantId, pageId);
            MasterPage masterPage = await _masterPageRepository.ReadMasterPageAsync(tenantId, page.MasterPageId);
            ResizeOptions thumbnailImageResizeOptions = new ResizeOptions { Mode = masterPage.ThumbnailImageResizeMode.Value, Width = masterPage.ThumbnailImageWidth.Value, Height = masterPage.ThumbnailImageHeight.Value };
            ResizeOptions previewImageResizeOptions = new ResizeOptions { Mode = masterPage.PreviewImageResizeMode.Value, Width = masterPage.PreviewImageWidth.Value, Height = masterPage.PreviewImageHeight.Value };
            Stream thumbnailImageContentStream = _imageAnalysisService.ResizeImage(content.Stream, thumbnailImageResizeOptions);
            Stream previewImageContentStream = _imageAnalysisService.ResizeImage(content.Stream, previewImageResizeOptions);

            // Create blobs for thumbnail, preview and original image
            long imageBlobId = await _storageService.CreateBlobAsync(tenantId, content);
            long previewImageBlobId = await _storageService.CreateBlobAsync(tenantId, new BlobContent { Name = content.Name, Type = content.Type, Stream = previewImageContentStream });
            long thumbnailImageBlobId = await _storageService.CreateBlobAsync(tenantId, new BlobContent { Name = content.Name, Type = content.Type, Stream = thumbnailImageContentStream });

            // Return result
            return new List<long> { imageBlobId, previewImageBlobId, thumbnailImageBlobId };
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
    }
}
