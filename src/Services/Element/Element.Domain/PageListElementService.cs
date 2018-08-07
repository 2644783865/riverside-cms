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
        public PageListImage BackgroundImage { get; set; }
        public PageListImage Image { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class PageListPageLink
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
    }

    public class PageListPager
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int PageCount { get; set; }
    }

    public class PageListElementContent
    {
        public PageListPageLink CurrentPage { get; set; }
        public PageListPageLink MorePage { get; set; }
        public IEnumerable<PageListPage> Pages { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public string DisplayName { get; set; }
        public string MoreMessage { get; set; }
        public string NoPagesMessage { get; set; }
        public PageListPager Pager { get; set; }
    }

    public interface IPageListElementService : IElementSettingsService<PageListElementSettings>, IElementViewService<PageListElementSettings, PageListElementContent>
    {
    }

    public class PageListElementService : IPageListElementService
    {
        private readonly IElementRepository<PageListElementSettings> _elementRepository;
        private readonly IPageService _pageService;
        private readonly IStorageService _storageService;
        private readonly ITagService _tagService;

        public PageListElementService(IElementRepository<PageListElementSettings> elementRepository, IPageService pageService, IStorageService storageService, ITagService tagService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
            _storageService = storageService;
            _tagService = tagService;
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

        private PageListImage GetImage(Page page, IDictionary<long, BlobImage> imagesById)
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

        private async Task<PageListPageLink> GetPageListPageLinkAsync(long tenantId, long pageId)
        {
            Page page = await _pageService.ReadPageAsync(tenantId, pageId);
            return new PageListPageLink
            {
                Home = !page.ParentPageId.HasValue,
                Name = page.Name,
                PageId = pageId
            };
        }

        public async Task<IElementView<PageListElementSettings, PageListElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            PageListElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            int pageIndex = 1;
            if (context.Parameters != null && context.Parameters.ContainsKey("page"))
                Int32.TryParse(context.Parameters["page"], out pageIndex);
            pageIndex = pageIndex - 1;

            IEnumerable<Tag> tags = null;
            if (context.TagIds != null)
                tags = await _tagService.ListTagsAsync(tenantId, context.TagIds);

            PageListPageLink currentPageLink = null;
            if (settings.ShowTags)
                currentPageLink = await GetPageListPageLinkAsync(tenantId, context.PageId);

            PageListPageLink morePageLink = null;
            long pageListPageId = settings.PageId ?? context.PageId;
            bool displayMorePageLink = settings.MoreMessage != null && context.PageId != pageListPageId;
            if (displayMorePageLink)
                morePageLink = await GetPageListPageLinkAsync(tenantId, pageListPageId);

            PageListResult result = await _pageService.ListPagesAsync(tenantId, pageListPageId, settings.Recursive, settings.PageType, context.TagIds, settings.SortBy, settings.SortAsc, pageIndex, settings.PageSize);

            IDictionary<long, BlobImage> blobsById = null;
            if (settings.ShowImage || settings.ShowBackgroundImage)
            {
                IEnumerable<long> blobIds = result.Pages.Where(p => p.ThumbnailImageBlobId.HasValue).Select(p => p.ThumbnailImageBlobId.Value);
                IEnumerable<Blob> blobs = await _storageService.ListBlobsAsync(tenantId, blobIds);
                blobsById = blobs.ToDictionary(b => b.BlobId, b => (BlobImage)b);
            }

            PageListElementContent content = new PageListElementContent
            {
                CurrentPage = currentPageLink,
                MorePage = morePageLink,
                DisplayName = GetContentDisplayName(settings.DisplayName, tags),
                MoreMessage = displayMorePageLink ? settings.MoreMessage : null,
                NoPagesMessage = settings.NoPagesMessage != null && !result.Pages.Any() ? settings.NoPagesMessage : null,
                Pages = result.Pages.Select(p => new PageListPage
                {
                    Name = p.Name,
                    PageId = p.PageId,
                    Home = !p.ParentPageId.HasValue,
                    Description = settings.ShowDescription ? p.Description : null,
                    Created = settings.ShowCreated ? (DateTime?)p.Created : null,
                    Updated = settings.ShowUpdated && !(settings.ShowCreated && (p.Created.Date == p.Updated.Date)) ? (DateTime?)p.Updated : null,
                    Occurred = settings.ShowOccurred && p.Occurred.HasValue ? p.Occurred : null,
                    OccursInFuture = settings.ShowOccurred && p.Occurred.HasValue ? p.Occurred.Value.Date > DateTime.UtcNow.Date : false,
                    BackgroundImage = settings.ShowBackgroundImage ? GetImage(p, blobsById) : null,
                    Image = settings.ShowImage ? GetImage(p, blobsById) : null,
                    Tags = settings.ShowTags ? p.Tags : new List<Tag>()
                }),
                Pager = GetPager(pageIndex, result, settings),
                Tags = tags
            };

            return new ElementView<PageListElementSettings, PageListElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
