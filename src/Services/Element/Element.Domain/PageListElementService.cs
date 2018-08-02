using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class PageListElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public string DisplayName { get; set; }
        public SortBy SortBy { get; set; }
        public bool SortAsc { get; set; }
        public bool ShowRelated { get; set; }
        public bool ShowDescription { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowBackgroundImage { get; set; }
        public bool ShowCreated { get; set; }
        public bool ShowUpdated { get; set; }
        public bool ShowOccurred { get; set; }
        public bool ShowComments { get; set; }
        public bool ShowTags { get; set; }
        public bool ShowPager { get; set; }
        public string MoreMessage { get; set; }
        public bool Recursive { get; set; }
        public PageType PageType { get; set; }
        public int PageSize { get; set; }
        public string NoPagesMessage { get; set; }
        public string Preamble { get; set; }
    }

    public class PageListImage
    {
        public long BlobId { get; set; }
        public long PageId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class PageListPage
    {
        public long PageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Home { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Occurred { get; set; }
        public bool OccursInFuture { get; set; }
        public PageListImage Image { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class PageListPager
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int PageCount { get; set; }
    }

    public class PageListElementContent : ElementContent
    {
        public long CurrentPageId { get; set; }
        public string CurrentPageName { get; set; }
        public bool CurrentPageHome { get; set; }
        public long PageListPageId { get; set; }
        public string PageListPageName { get; set; }
        public bool PageListPageHome { get; set; }
        public IEnumerable<PageListPage> Pages { get; set; }
        public string DisplayName { get; set; }
        public string MoreMessage { get; set; }
        public string NoPagesMessage { get; set; }
        public PageListPager Pager { get; set; }
    }

    public interface IPageListElementService : IElementSettingsService<PageListElementSettings>, IElementContentService<PageListElementContent>
    {
    }

    public class PageListElementService : IPageListElementService
    {
        private readonly IElementRepository<PageListElementSettings> _elementRepository;
        private readonly IPageService _pageService;
        private readonly IStorageService _storageService;

        public PageListElementService(IElementRepository<PageListElementSettings> elementRepository, IPageService pageService, IStorageService storageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
            _storageService = storageService;
        }

        public Task<PageListElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private string GetContentDisplayName(string displayName, IEnumerable<Tag> tags)
        {
            if (displayName != null && tags?.Any() == true)
                displayName += " " + string.Join(" ", tags.Select(t => "#" + t.Name));
            return displayName;
        }

        private PageListPager GetPager(int pageIndex, PageListResult result, PageListElementSettings elementSettings)
        {
            if (!elementSettings.ShowPager)
                return null;
            int pageCount = ((result.Total - 1) / elementSettings.PageSize) + 1;
            if (pageCount < 2)
                return null;
            return new PageListPager
            {
                PageCount = pageCount,
                PageIndex = pageIndex,
                PageSize = elementSettings.PageSize,
                Total = result.Total
            };
        }

        private PageListImage GetImage(Page page, Dictionary<long, BlobImage> imagesById)
        {
            if (!page.ThumbnailImageBlobId.HasValue)
                return null;
            BlobImage thumbnailImage = imagesById[page.ThumbnailImageBlobId.Value];
            return new PageListImage
            {
                BlobId = thumbnailImage.BlobId,
                PageId = page.PageId,
                Width = thumbnailImage.Width,
                Height = thumbnailImage.Height
            };
        }

        public async Task<PageListElementContent> ReadElementContentAsync(long tenantId, long elementId, long pageId)
        {
            PageListElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            int pageIndex = 0;
            IEnumerable<Tag> tags = new[] { new Tag { Name = "bmw", TagId = 400 } };
            tags = new List<Tag>();
            IEnumerable<long> tagIds = tags.Select(t => t.TagId);

            Page currentPage = await _pageService.ReadPageAsync(tenantId, pageId);
            long pageListPageId = elementSettings.PageId ?? pageId;
            Page pageListPage = await _pageService.ReadPageAsync(tenantId, pageListPageId);

            PageListResult result = await _pageService.ListPagesAsync(tenantId, pageListPageId, elementSettings.Recursive, elementSettings.PageType, tagIds, elementSettings.SortBy, elementSettings.SortAsc, pageIndex, elementSettings.PageSize);

            Dictionary<long, BlobImage> imagesById = new Dictionary<long, BlobImage>();
            if (elementSettings.ShowImage)
            {
                foreach (Page page in result.Pages)
                {
                    if (page.ThumbnailImageBlobId.HasValue)
                        imagesById.Add(page.ThumbnailImageBlobId.Value, (BlobImage)await _storageService.ReadBlobAsync(tenantId, page.ThumbnailImageBlobId.Value));
                }
            }

            PageListElementContent elementContent = new PageListElementContent
            {
                TenantId = elementSettings.TenantId,
                ElementId = elementSettings.ElementId,
                ElementTypeId = elementSettings.ElementTypeId,
                CurrentPageId = pageId,
                CurrentPageName = currentPage.Name,
                CurrentPageHome = !currentPage.ParentPageId.HasValue,
                PageListPageId = pageListPageId,
                PageListPageName = pageListPage.Name,
                PageListPageHome = !pageListPage.ParentPageId.HasValue,
                DisplayName = GetContentDisplayName(elementSettings.DisplayName, tags),
                MoreMessage = elementSettings.MoreMessage != null && pageId != pageListPageId ? elementSettings.MoreMessage : null,
                NoPagesMessage = elementSettings.NoPagesMessage != null && !result.Pages.Any() ? elementSettings.NoPagesMessage : null,
                Pages = result.Pages.Select(p => new PageListPage
                {
                    Name = p.Name,
                    PageId = p.PageId,
                    Home = !p.ParentPageId.HasValue,
                    Description = elementSettings.ShowDescription ? p.Description : null,
                    Created = elementSettings.ShowCreated ? (DateTime?)p.Created : null,
                    Updated = elementSettings.ShowUpdated && !(elementSettings.ShowCreated && (p.Created.Date == p.Updated.Date)) ? (DateTime?)p.Updated : null,
                    Occurred = elementSettings.ShowOccurred && p.Occurred.HasValue ? p.Occurred : null,
                    OccursInFuture = elementSettings.ShowOccurred && p.Occurred.HasValue ? p.Occurred.Value.Date > DateTime.UtcNow.Date : false,
                    Image = GetImage(p, imagesById),
                    Tags = elementSettings.ShowTags ? p.Tags : new List<Tag>()
                }),
                Pager = GetPager(pageIndex, result, elementSettings)
            };

            return elementContent;
        }
    }
}
