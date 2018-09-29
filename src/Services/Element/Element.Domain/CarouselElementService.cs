using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Element.Domain
{
    public class CarouselSlide
    {
        public long BlobSetId { get; set; }
        public long ImageBlobId { get; set; }
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ButtonPageId { get; set; }
        public string ButtonText { get; set; }
    }

    public class CarouselElementSettings : ElementSettings
    {
        public IEnumerable<CarouselSlide> Slides { get; set; }
    }

    public class CarouselButton
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
        public string ButtonText { get; set; }
    }

    public class CarouselContentSlide
    {
        public long BlobSetId { get; set; }
        public int PreviewWidth { get; set; }
        public int PreviewHeight { get; set; }
        public CarouselButton Button { get; set; }
    }

    public class CarouselElementContent
    {
        public IDictionary<long, CarouselContentSlide> Slides { get; set; }
    }

    public interface ICarouselElementService : IElementSettingsService<CarouselElementSettings>, IElementViewService<CarouselElementSettings, CarouselElementContent>, IElementStorageService
    {
    }

    public class CarouselElementService : ICarouselElementService
    {
        private readonly IElementRepository<CarouselElementSettings> _elementRepository;
        private readonly IPageService _pageService;
        private readonly IStorageService _storageService;

        private const string CarouselImagePath = "elements/carousels/{0}";

        private const string OriginalBlobLabel = "original";
        private const string ThumbnailBlobLabel = "thumbnail";
        private const string PreviewBlobLabel = "preview";

        public CarouselElementService(IElementRepository<CarouselElementSettings> elementRepository, IPageService pageService, IStorageService storageService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
            _storageService = storageService;
        }

        public Task<CarouselElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<CarouselElementSettings, CarouselElementContent>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            // Get element settings
            CarouselElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            // Get details of preview images
            IEnumerable<long> previewImageBlobIds = settings.Slides.Select(s => s.PreviewImageBlobId);
            IEnumerable<Blob> previewImageBlobs = await _storageService.ListBlobsAsync(tenantId, previewImageBlobIds);
            IDictionary<long, BlobImage> previewImageBlobsById = previewImageBlobs.ToDictionary(b => b.BlobId, b => (BlobImage)b);

            // Get details of pages linked to from carousel slide buttons
            IEnumerable<long> buttonPageIds = settings.Slides.Where(s => s.ButtonPageId.HasValue).Select(s => s.ButtonPageId.Value).Distinct();
            IEnumerable<Page> buttonPages = await _pageService.ListPagesAsync(tenantId, buttonPageIds);
            IDictionary<long, string> buttonTextByPageId = settings.Slides.Where(s => s.ButtonPageId.HasValue && s.ButtonText != null).ToDictionary(s => s.ButtonPageId.Value, s => s.ButtonText);
            IDictionary<long, CarouselButton> buttonsByPageId = buttonPages.ToDictionary(p => p.PageId, p => new CarouselButton { Home = !p.ParentPageId.HasValue, Name = p.Name, PageId = p.PageId, ButtonText = buttonTextByPageId.ContainsKey(p.PageId) ? buttonTextByPageId[p.PageId] : null });

            // Construct list of slides for carousel content
            IDictionary<long, CarouselContentSlide> slides = settings
                .Slides
                .Where(s => previewImageBlobsById.ContainsKey(s.PreviewImageBlobId))
                .Select(s => new CarouselContentSlide
                {
                    BlobSetId = s.BlobSetId,
                    PreviewWidth = previewImageBlobsById[s.PreviewImageBlobId].Width,
                    PreviewHeight = previewImageBlobsById[s.PreviewImageBlobId].Height,
                    Button = s.ButtonPageId.HasValue && buttonsByPageId.ContainsKey(s.ButtonPageId.Value) ? buttonsByPageId[s.ButtonPageId.Value] : null
                })
                .ToDictionary(s => s.BlobSetId, s => s);

            // Construct element content
            CarouselElementContent content = new CarouselElementContent
            {
                Slides = slides
            };

            // Return element view
            return new ElementView<CarouselElementSettings, CarouselElementContent>
            {
                Settings = settings,
                Content = content
            };
        }

        private long? GetBlobId(CarouselSlide slide, string blobLabel)
        {
            switch (blobLabel)
            {
                case OriginalBlobLabel:
                    return slide.ImageBlobId;

                case PreviewBlobLabel:
                    return slide.PreviewImageBlobId;

                case ThumbnailBlobLabel:
                    return slide.ThumbnailImageBlobId;

                default:
                    return slide.PreviewImageBlobId;
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long blobSetId, string blobLabel)
        {
            CarouselElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            CarouselSlide slide = settings.Slides.Where(s => s.BlobSetId == blobSetId).FirstOrDefault();
            if (slide == null)
                return null;

            long? blobId = GetBlobId(slide, blobLabel);
            if (blobId == null)
                return null;

            return await _storageService.ReadBlobContentAsync(tenantId, blobId.Value, string.Format(CarouselImagePath, elementId));
        }
    }
}
