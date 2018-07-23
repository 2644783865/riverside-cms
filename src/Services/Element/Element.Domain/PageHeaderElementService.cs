using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class PageHeaderElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public bool ShowName { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowCreated { get; set; }
        public bool ShowUpdated { get; set; }
        public bool ShowOccurred { get; set; }
        public bool ShowBreadcrumbs { get; set; }
    }

    public class PageHeaderBreadcrumb
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string PageName { get; set; }
    }

    public class PageHeaderElementContent : IElementContent
    {
        public Page Page { get; set; }
        public IDictionary<long, BlobImage> Images { get; set; }
        public IEnumerable<PageHeaderBreadcrumb> Breadcrumbs { get; set; }
    }

    public interface IPageHeaderElementService : IElementSettingsService<PageHeaderElementSettings>, IElementContentService<PageHeaderElementContent>
    {
    }

    public class PageHeaderElementService : IPageHeaderElementService
    {
        private readonly IElementRepository<PageHeaderElementSettings> _elementRepository;
        private readonly IPageService _pageService;
        private readonly IStorageService _storageService;

        public PageHeaderElementService(IElementRepository<PageHeaderElementSettings> elementRepository, IPageService pageService, IStorageService storageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
            _storageService = storageService;
        }

        public Task<PageHeaderElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<PageHeaderElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            PageHeaderElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            Page page = await _pageService.ReadPageAsync(tenantId, pageId);

            BlobImage image = page.ImageBlobId.HasValue ? (BlobImage) await _storageService.ReadBlobAsync(tenantId, page.ImageBlobId.Value) : null;
            BlobImage previewImage = page.PreviewImageBlobId.HasValue ? (BlobImage) await _storageService.ReadBlobAsync(tenantId, page.PreviewImageBlobId.Value) : null;
            BlobImage thumbnailImage = page.ThumbnailImageBlobId.HasValue ? (BlobImage) await _storageService.ReadBlobAsync(tenantId, page.ThumbnailImageBlobId.Value) : null;

            IDictionary<long, BlobImage> images = new Dictionary<long, BlobImage>();
            if (image != null)
                images.Add(image.BlobId, image);
            if (previewImage != null)
                images.Add(previewImage.BlobId, previewImage);
            if (thumbnailImage != null)
                images.Add(thumbnailImage.BlobId, thumbnailImage);

            IEnumerable<PageHeaderBreadcrumb> breadcrumbs = null;
            if (elementSettings.ShowBreadcrumbs)
            {
                IEnumerable<Page> pages = await _pageService.ListPagesInHierarchyAsync(tenantId, pageId);
                breadcrumbs = pages.Reverse().Select(p => new PageHeaderBreadcrumb
                {
                    Home = p.ParentPageId == null,
                    Name = p.ParentPageId == null ? "Home" : p.Name,
                    PageId = p.PageId,
                    PageName = p.Name
                });
            }

            return new PageHeaderElementContent
            {
                Page = page,
                Images = images,
                Breadcrumbs = breadcrumbs
            };
        }
    }
}
