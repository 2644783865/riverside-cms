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

    public class PageHeaderImage
    {
        public long TenantId { get; set; }
        public long BlobId { get; set; }
        public long PageId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class PageHeaderElementContent : IElementContent
    {
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Occurred { get; set; }
        public bool OccursInFuture { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public PageHeaderImage Image { get; set; }

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

            PageHeaderElementContent elementContent = new PageHeaderElementContent();

            elementContent.Created = elementSettings.ShowCreated ? (DateTime?)page.Created : null;
            elementContent.Updated = elementSettings.ShowUpdated && !(elementSettings.ShowCreated && (page.Created.Date == page.Updated.Date)) ? (DateTime?)page.Updated : null;
            elementContent.Occurred = elementSettings.ShowOccurred && page.Occurred.HasValue ? page.Occurred : null;
            elementContent.OccursInFuture = elementContent.Occurred.HasValue && (page.Occurred.Value.Date > DateTime.UtcNow.Date);
            elementContent.Name = elementSettings.ShowName ? page.Name : null;
            elementContent.Description = elementSettings.ShowDescription ? page.Description : null;

            if (elementSettings.ShowImage && page.ThumbnailImageBlobId.HasValue)
            {
                BlobImage thumbnailImage = (BlobImage) await _storageService.ReadBlobAsync(tenantId, page.ThumbnailImageBlobId.Value);
                elementContent.Image = new PageHeaderImage
                {
                    TenantId = tenantId,
                    BlobId = thumbnailImage.BlobId,
                    PageId = page.PageId,
                    Height = thumbnailImage.Height,
                    Width = thumbnailImage.Width
                };
            }

            if (elementSettings.ShowBreadcrumbs)
            {
                IEnumerable<Page> pages = await _pageService.ListPagesInHierarchyAsync(tenantId, pageId);
                elementContent.Breadcrumbs = pages.Reverse().Select(p => new PageHeaderBreadcrumb
                {
                    Home = p.ParentPageId == null,
                    Name = p.Name,
                    PageId = p.PageId,
                    PageName = p.Name
                });
            }

            return elementContent;
        }
    }
}
