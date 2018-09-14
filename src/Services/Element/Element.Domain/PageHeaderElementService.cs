using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Storage.Domain;

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
        public long BlobId { get; set; }
        public long PageId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class PageHeaderElementContent
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

    public interface IPageHeaderElementService : IElementSettingsService<PageHeaderElementSettings>, IElementViewService<PageHeaderElementSettings, PageHeaderElementContent>
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

        public async Task<IElementView<PageHeaderElementSettings, PageHeaderElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            PageHeaderElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            Page page = await _pageService.ReadPageAsync(tenantId, context.PageId);

            PageHeaderElementContent content = new PageHeaderElementContent();

            content.Created = settings.ShowCreated ? (DateTime?)page.Created : null;
            content.Updated = settings.ShowUpdated && !(settings.ShowCreated && (page.Created.Date == page.Updated.Date)) ? (DateTime?)page.Updated : null;
            content.Occurred = settings.ShowOccurred && page.Occurred.HasValue ? page.Occurred : null;
            content.OccursInFuture = content.Occurred.HasValue && (page.Occurred.Value.Date > DateTime.UtcNow.Date);
            content.Name = settings.ShowName ? page.Name : null;
            content.Description = settings.ShowDescription ? page.Description : null;

            if (settings.ShowImage && page.ThumbnailImageBlobId.HasValue)
            {
                BlobImage thumbnailImage = (BlobImage)await _storageService.ReadBlobAsync(tenantId, page.ThumbnailImageBlobId.Value);
                content.Image = new PageHeaderImage
                {
                    BlobId = thumbnailImage.BlobId,
                    PageId = page.PageId,
                    Height = thumbnailImage.Height,
                    Width = thumbnailImage.Width
                };
            }

            if (settings.ShowBreadcrumbs)
            {
                IEnumerable<Page> pages = await _pageService.ListPagesInHierarchyAsync(tenantId, context.PageId);
                content.Breadcrumbs = pages.Reverse().Select(p => new PageHeaderBreadcrumb
                {
                    Home = p.ParentPageId == null,
                    Name = p.Name,
                    PageId = p.PageId,
                    PageName = p.Name
                });
            }

            return new ElementView<PageHeaderElementSettings, PageHeaderElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
